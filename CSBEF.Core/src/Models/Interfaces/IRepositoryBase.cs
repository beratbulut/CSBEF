using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CSBEF.Models.Interfaces;

namespace CSBEF.Core.Models.Interfaces
{
    /// <summary>
    /// TODO: To be translated into English
    /// Repository Pattern için eklenmiş generic repository base sınıfıdır.
    /// Sınıf içerisinde "Save" metotları yer almaktadır.
    /// Bu metotlar eğer TransactionHelper veya UnitOfWork kullanılmıyorsa çalıştırılmalıdır.
    /// Fakat bu iki destekten biri yada ikisi kullanılıyorsa, bu metotlar burada kullanılmamalıdır.
    /// 
    /// Silme işlemi için "Delete" metotları yer almaktadır. Fakat bu metotların kullanılmasını tavsiye etmemekteyiz.
    /// Çünkü özellikle de günümüzde veriler fiziki olarak silinmesi istenmediğinden, bu metotlar kullanılarak fiziki silme işlemi yapmayıp,
    /// Status değerini değiştirerek silinme işareti bırakılmalıdır.
    /// </summary>
    /// <typeparam name="TEntity">Kullanılacak entity model (IEntityModelBase interface'inden kalıtım almış olmalıdır).</typeparam>
    public interface IRepositoryBase<TEntity> : IDisposable
        where TEntity : class, IEntityModelBase
    {
        IQueryable<TEntity> GetAll();
        Task<ICollection<TEntity>> GetAllAsyn();
        TEntity Add(TEntity t);
        Task<TEntity> AddAsync(TEntity t);
        TEntity Find(Expression<Func<TEntity, bool>> match);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match);
        ICollection<TEntity> FindAll(Expression<Func<TEntity, bool>> match);
        Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match);
        void Delete(TEntity entity);
        Task DeleteAsync(TEntity entity);
        TEntity Update(TEntity t);
        Task<TEntity> UpdateAsync(TEntity t);
        int Count();
        int Count(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        void Save();
        Task<int> SaveAsync();
    }
}