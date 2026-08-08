namespace Application.Interfaces.Repositories;

public interface IRepositoryBase<T> where T : class
{
    // Create
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

    // Read
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);

    // Update
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);

    // Delete
    // void Delete(T entity);
    // void DeleteRange(IEnumerable<T> entities);
    // void DeleteAsync(T entity, CancellationToken ct = default);
    // void DeleteRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

}
