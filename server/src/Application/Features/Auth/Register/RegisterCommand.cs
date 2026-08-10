using Application.Common;
using MediatR;

namespace Application.Features.Auth.Register;

public record RegisterCommand(string Name, string Email, string Password) : IRequest<ResponseBase>;
