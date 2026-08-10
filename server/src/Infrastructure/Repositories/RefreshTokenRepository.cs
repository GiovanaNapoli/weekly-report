using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default)
        => await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);
}
