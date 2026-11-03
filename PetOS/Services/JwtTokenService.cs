using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PetOS.Dto.Auth;
using PetOS.Models;

namespace PetOS.Services;

public class JwtTokenService
{
    private readonly SymmetricSecurityKey _signingKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtTokenService(IConfiguration configuration)
    {
        var secret = configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(secret) ||
            Encoding.UTF8.GetByteCount(secret) < 32)
        {
            throw new InvalidOperationException(
                "Configure Jwt:Key com pelo menos 32 bytes."
            );
        }

        _signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secret)
        );

        _issuer = configuration["Jwt:Issuer"] ?? "PetOS";
        _audience = configuration["Jwt:Audience"] ?? "PetOS-API";

        _expirationMinutes =
            int.TryParse(
                configuration["Jwt:ExpirationMinutes"],
                out var minutes
            ) && minutes >= 1 && minutes <= 1440
                ? minutes
                : 60;
    }

    public AuthResponseDto CreateToken(AppUser user)
    {
        var expires = DateTime.UtcNow.AddMinutes(
            _expirationMinutes
        );

        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()
            ),

            new Claim(
                JwtRegisteredClaimNames.Email,
                user.Email
            ),

            new Claim(
                ClaimTypes.Name,
                user.Name
            ),

            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()
            )
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(
                _signingKey,
                SecurityAlgorithms.HmacSha256
            )
        );

        return new AuthResponseDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            "Bearer",
            expires
        );
    }
}