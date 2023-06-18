
namespace AppLogic.Common.Interfaces;

public interface IBaseRepository<E> where E : BaseEntity
{
    Task<E?> GetByIdAsync(int id );
    Task<SqlResult?> InsertAsync(E entity );
    Task<bool> DeleteByIdAsync(int id );
    Task<SqlResult> UpdateAsync(E entity );
}