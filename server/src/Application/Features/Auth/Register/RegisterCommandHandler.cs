using Application.Common;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ResponseBase>
{
    private readonly IUserRepository _userRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _uow;

    public RegisterCommandHandler(IUserRepository userRepository, IIdentityService identityService, IUnitOfWork uow)
    {
        _userRepository = userRepository;
        _identityService = identityService;
        _uow = uow;
    }

    public async Task<ResponseBase> Handle(RegisterCommand request, CancellationToken ct)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email, ct))
            return ResponseBase.Failure("E-mail já está em uso.");

        await _uow.BeginTransactionAsync(ct);

        try
        {
            var user = new User
            {
                Name = request.Name,
                Email = request.Email
            };

            await _userRepository.AddAsync(user, ct);

            var (success, error) = await _identityService.RegisterAsync(request.Email, request.Password, user.Id);
            if (!success)
            {
                await _uow.RollbackAsync(ct);
                return ResponseBase.Failure(error!);
            }

            await _uow.CommitAsync(ct);
            return ResponseBase.Success(["Usuário criado com sucesso."]);
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            throw;
        }
    }
}
