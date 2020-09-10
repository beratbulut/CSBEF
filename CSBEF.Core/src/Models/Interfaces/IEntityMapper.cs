using Microsoft.EntityFrameworkCore;

namespace CSBEF.Models.Interfaces
{
    /// <summary>
    /// TODO: To be translated into English
    /// Entity modellerinin map'lenmesi için kullanılan modellerin kalıtım aldıkları interface'dir.
    /// CSBEF, bu interface'den kalıtım alan tüm sınıfları otomatik olarak ele alır ve map'lemeyi yapar.
    /// </summary>
    public interface IEntityMapper
    {
        /// <summary>
        /// TODO: To be translated into English
        /// Map'leme işlemi için CSBEF tarafından otomatik olarak tetiklenen metottur.
        /// </summary>
        /// <param name="args">İlgili DbContext içerisindeki ModelBuilder injection'ı.</param>
        void Mapper(ModelBuilder modelBuilder);
    }
}