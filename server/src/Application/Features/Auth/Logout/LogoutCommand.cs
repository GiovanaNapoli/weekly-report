using Application.Common;
using MediatR;

namespace Application.Features.Auth.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<ResponseBase>;
