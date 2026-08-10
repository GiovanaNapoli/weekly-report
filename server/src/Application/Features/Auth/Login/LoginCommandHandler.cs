using Application.Common;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ResponseBase<AuthResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LoginCommandHandler(
        IIdentityService identityService,
        IJwtService jwtService,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _identityService = identityService;
        _jwtService = jwtService;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<ResponseBase<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var isValidPassword = await _identityService.CheckPasswordAsync(request.Email, request.Password);
        if (!isValidPassword)
            return ResponseBase<AuthResponse>.Failure("E-mail ou senha inválidos.");

        var user = await _userRepository.GetByEmailAsync(request.Email, ct);
        if (user is null)
            return ResponseBase<AuthResponse>.Failure("E-mail ou senha inválidos.");

        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, []);
        var refreshToken = _jwtService.GenerateRefreshToken();

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = TokenHasher.Hash(refreshToken),
            ExpiresAt = _jwtService.GetRefreshTokenExpiresAt()
        }, ct);

        var result = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = _jwtService.GetAccessTokenExpiresAt()
        };

        return ResponseBase<AuthResponse>.Success(result);
    }
}
