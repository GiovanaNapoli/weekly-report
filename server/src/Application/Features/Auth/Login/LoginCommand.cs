using Application.Common;
using MediatR;

namespace Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<ResponseBase<AuthResponse>>;
