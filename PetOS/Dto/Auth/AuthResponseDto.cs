namespace PetOS.Dto.Auth;

public record AuthResponseDto(
    string AccessToken,
    string TokenType,
    DateTime ExpiresAtUtc
);