using Application.Common;
using MediatR;

namespace Application.Features.Auth.Refresh;

public record RefreshTokenCommand(string RefreshToken) : IRequest<ResponseBase<AuthResponse>>;
