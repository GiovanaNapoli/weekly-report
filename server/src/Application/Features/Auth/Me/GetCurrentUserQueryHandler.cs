using Application.Common;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Auth.Me;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, ResponseBase<MeResponse>>
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ResponseBase<MeResponse>> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
            return ResponseBase<MeResponse>.Failure("Usuário não encontrado.");

        return ResponseBase<MeResponse>.Success(new MeResponse
        {
            Id = user.Id.ToString(),
            Email = user.Email
        });
    }
}
