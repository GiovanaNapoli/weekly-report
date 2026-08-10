using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IUserRepository : IRepositoryBase<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
}
