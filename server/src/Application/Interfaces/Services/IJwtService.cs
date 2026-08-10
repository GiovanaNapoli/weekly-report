namespace Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles);
    string GenerateRefreshToken();
    DateTime GetAccessTokenExpiresAt();
    DateTime GetRefreshTokenExpiresAt();
}
