using Microsoft.AspNetCore.Identity;
using PetOS.Dto.Auth;
using PetOS.Models;
using PetOS.Repositories.Interfaces;
using PetOS.Services.Interfaces;

namespace PetOS.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher<AppUser> _passwordHasher;
    private readonly JwtTokenService _tokens;

    public AuthService(
        IUserRepository users,
        IPasswordHasher<AppUser> passwordHasher,
        JwtTokenService tokens)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _tokens = tokens;
    }

    public async Task<long?> RegisterAsync(RegisterRequestDto request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _users.EmailExistsAsync(email))
        {
            return null;
        }

        var user = new AppUser
        {
            Name = request.Name.Trim(),
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.Password
        );

        await _users.AddAsync(user);

        return user.Id;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _users.FindByEmailAsync(email);

        if (user == null)
        {
            return null;
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password
        );

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return _tokens.CreateToken(user);
    }
}