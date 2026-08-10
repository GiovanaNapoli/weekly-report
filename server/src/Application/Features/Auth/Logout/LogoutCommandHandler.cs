using Application.Common;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Auth.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ResponseBase>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<ResponseBase> Handle(LogoutCommand request, CancellationToken ct)
    {
        var tokenHash = TokenHasher.Hash(request.RefreshToken);
        var existingToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, ct);

        if (existingToken is null || !existingToken.IsActive)
            return ResponseBase.Failure("Refresh token inválido.");

        existingToken.IsActive = false;
        _refreshTokenRepository.Update(existingToken);

        return ResponseBase.Success(["Logout realizado com sucesso."]);
    }
}
