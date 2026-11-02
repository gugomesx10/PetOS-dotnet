using PetOS.Dto.Auth;

namespace PetOS.Services.Interfaces;

public interface IAuthService
{
    Task<long?> RegisterAsync(RegisterRequestDto request);

    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);
}