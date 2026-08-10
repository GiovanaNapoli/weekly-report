using Application.Common;
using MediatR;

namespace Application.Features.Auth.Me;

public record GetCurrentUserQuery(Guid UserId) : IRequest<ResponseBase<MeResponse>>;
