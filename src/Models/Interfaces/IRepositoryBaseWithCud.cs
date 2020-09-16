using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CSBEF.Models.Interfaces
{
    public interface IRepositoryBaseWithCud<T> : IRepositoryBase<T>
        where T : class, IEntityModelBase
    {
        int Save();
        Task<int> SaveAsync(CancellationToken cancellationToken = default);
        int Add(T entity, bool useSave = true);
        Task<int> AddAsync(T entity, bool useSave = true, CancellationToken cancellationToken = default);
        int AddRange(IEnumerable<T> entities, bool useSave = true);
        Task<int> AddRangeAsync(IEnumerable<T> entities, bool useSave = true, CancellationToken cancellationToken = default);
        int Update(T entity, bool useSave = true);
        Task<int> UpdateAsync(T entity, bool useSave = true, CancellationToken cancellationToken = default);
        int UpdateRange(IEnumerable<T> entities, bool useSave = true);
        Task<int> UpdateRangeAsync(IEnumerable<T> entities, bool useSave = true, CancellationToken cancellationToken = default);
        int SoftDelete(T entity, bool useSave = true);
        Task<int> SoftDeleteAsync(T entity, bool useSave = true, CancellationToken cancellationToken = default);
        int SoftDeleteRange(IEnumerable<T> entities, bool useSave = true);
        Task<int> SoftDeleteRangeAsync(IEnumerable<T> entities, bool useSave = true, CancellationToken cancellationToken = default);
        int HardDelete(T entity, bool useSave = true);
        Task<int> HardDeleteAsync(T entity, bool useSave = true, CancellationToken cancellationToken = default);
        int HardDeleteRange(IEnumerable<T> entities, bool useSave = true);
        Task<int> HardDeleteRangeAsync(IEnumerable<T> entities, bool useSave = true, CancellationToken cancellationToken = default);
    }
}