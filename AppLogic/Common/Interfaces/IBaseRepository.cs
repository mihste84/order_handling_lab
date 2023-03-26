
namespace AppLogic.Common.Interfaces;

public interface IBaseRepository<E> where E : BaseEntity
{
    Task<E?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int?> InsertAsync(E entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(E entity, CancellationToken cancellationToken = default);
}