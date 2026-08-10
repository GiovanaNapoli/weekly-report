using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenExpirationMinutes;
    private readonly int _refreshTokenExpirationDays;

    public JwtService(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:SecretKey"]!;
        _issuer = configuration["Jwt:Issuer"]!;
        _audience = configuration["Jwt:Audience"]!;
        _accessTokenExpirationMinutes = int.Parse(configuration["Jwt:AccessTokenExpirationMinutes"]!);
        _refreshTokenExpirationDays = int.Parse(configuration["Jwt:RefreshTokenExpirationDays"]!);
    }

    public string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: GetAccessTokenExpiresAt(),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public DateTime GetAccessTokenExpiresAt()
        => DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes);

    public DateTime GetRefreshTokenExpiresAt()
        => DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);
}
