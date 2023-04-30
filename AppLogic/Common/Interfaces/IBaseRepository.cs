
namespace AppLogic.Common.Interfaces;

public interface IBaseRepository<E> where E : BaseEntity
{
    Task<E?> GetByIdAsync(int id );
    Task<int?> InsertAsync(E entity );
    Task<bool> DeleteByIdAsync(int id );
    Task<bool> UpdateAsync(E entity );
}