using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IRefreshTokenRepository : IRepositoryBase<RefreshToken>
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
}
