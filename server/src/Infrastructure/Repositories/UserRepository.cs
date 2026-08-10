using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        => await _context.Users.AnyAsync(u => u.Email == email, ct);
}
