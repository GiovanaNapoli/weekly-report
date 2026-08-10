using Application.Common;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using MediatR;

namespace Application.Features.Auth.Refresh;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ResponseBase<AuthResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtService jwtService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<ResponseBase<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var tokenHash = TokenHasher.Hash(request.RefreshToken);
        var existingToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, ct);

        if (existingToken is null || !existingToken.IsActive || existingToken.ExpiresAt <= DateTime.UtcNow)
            return ResponseBase<AuthResponse>.Failure("Refresh token inválido ou expirado.");

        var user = await _userRepository.GetByIdAsync(existingToken.UserId, ct);
        if (user is null)
            return ResponseBase<AuthResponse>.Failure("Refresh token inválido ou expirado.");

        existingToken.IsActive = false;
        _refreshTokenRepository.Update(existingToken);

        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, []);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        await _refreshTokenRepository.AddAsync(new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            TokenHash = TokenHasher.Hash(newRefreshToken),
            ExpiresAt = _jwtService.GetRefreshTokenExpiresAt()
        }, ct);

        var result = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = _jwtService.GetAccessTokenExpiresAt()
        };

        return ResponseBase<AuthResponse>.Success(result);
    }
}
