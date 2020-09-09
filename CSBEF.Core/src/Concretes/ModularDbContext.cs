using System;
using System.Collections.Generic;
using System.Linq;
using CSBEF.Helpers;
using CSBEF.Models.Interfaces;
using CSBEF.src.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CSBEF.Concretes
{
    /// <summary>
    /// TODO: To be translated into English
    /// API uygulamasında kullanılacak Entity Framework DbContext instance nesnesidir.
    /// Oluşturulan bu nesne modüler olarak birleştirilerek tek bir instance olarak tüm modüller tarafından kullanılmaktadır.
    /// </summary>
    public class ModularDbContext : DbContext
    {
        /// <summary>
        /// TODO: To be translated into English
        /// Oluşturulan DbContext için gereken ön ayarlar ctor kısmında yapılmaktadır.
        /// Bu yapılan ayarlamalar için gereken ayarlar, "GlobalConfiguration.ApiStartOptions" içerisinden elde edilmektedir.
        /// </summary>
        /// <param name="options">Dışarıdan gelen "DbContextOptions" tipinde model (bu gelen argumanlar kullanılmamaktadır)</param>
        public ModularDbContext(DbContextOptions options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = GlobalConfiguration.ApiStartOptions.DbContextQueryTrackingBehavior;
            ChangeTracker.LazyLoadingEnabled = GlobalConfiguration.ApiStartOptions.DbContextLazyLoadingEnabled;
            ChangeTracker.AutoDetectChangesEnabled = GlobalConfiguration.ApiStartOptions.DbContextAutoDetectChangesEnabled;
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Bu metot içerisinde, modüllerdeki poco modeller DbContext içerisine eklenmektedir.
        /// Buradaki işlemler diğer metotlarda sırasıyla tetiklenerek gerçekleştirilmektedir.
        /// Bu metot içerisinde ele alınan poco modelleri "IEntityModelBase" kalıtımı ile belirlenmektedir.
        /// </summary>
        /// <param name="modelBuilder">DbContext için gereken ModelBuilder injection'ıdır.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ThrowIfNull();

            // TODO: To be translated into English
            // Tespit edilen poco modellerin tipleri, bu liste içerisine alınmaktadır ve üzerinde çeşitli işlemler gerçekleştirilecektir.
            List<Type> typeToRegisters = new List<Type>();

            // TODO: To be translated into English
            // İçeriye aktarılmış modül olup olmadığını kontrol ediyoruz. Eğer varsa, her modül içerisindeki poco modeller "typeToRegisters"
            // listesine eklenmektedir.
            if (GlobalConfiguration.Modules.Any())
            {
                foreach (var module in GlobalConfiguration.Modules)
                {
                    var getTypes = module.Assembly.DefinedTypes.Select(s => s.AsType());
                    if (getTypes.Any())
                    {
                        typeToRegisters.AddRange(module.Assembly.DefinedTypes.Where(w => typeof(IEntityModelBase).IsAssignableFrom(w)).Select(s => s.AsType()));
                    }
                }
            }

            // TODO: To be translated into English
            // Listeye alınan poco modellerin DbContext içerisine eklenmesi işini yapacak olan metot tetikleniyor.
            RegisterEntities(modelBuilder, typeToRegisters);

            // TODO: To be translated into English
            // DbContext içerisine eklenen Entity modeller ile tablo adı ayarlamalarını yapan metot tetikleniyor.
            RegiserConvention(modelBuilder);

            // TODO: To be translated into English
            // Base içerisindeki "OnModelCreating" metodu tetikleniyor.
            base.OnModelCreating(modelBuilder);

            // TODO: To be translated into English
            // Poco modellerinin mapping işlemlerini yapacak metot tetikleniyor.
            RegisterCustomMappings(modelBuilder, typeToRegisters);
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Modüllerdeki "IEntityModelBase" kalıtımı alan tipleri DbContext içerisine entity model olarak tanıtan metottur.
        /// "typeToRegisters" ile modüllerdeki tüm tipler gelmektedir.
        /// Bu metot içerisinde bu tiplerden "IEntityModelBase" kalıtımı alınanlar belirlenmekte ve bu tipler entity model olarak DbContext'e tanıtılmaktadır.
        /// </summary>
        /// <param name="modelBuilder">DbContext için gereken "ModelBuilder" nesnesi</param>
        /// <param name="typeToRegisters">Tüm modüllerin tüm tiplerini içeren liste</param>
        private static void RegisterEntities(ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters)
        {
            // TODO: To be translated into English
            // Gelen "typeToRegisters" içerisindeki tiplerden, "IEntityModelBase" kalıtımı almış olanlar bu listede biriktirilecektir.
            IEnumerable<Type> entityTypes = null;

            // TODO: To be translated into English
            // Gelen "typeToRegisters" içerisindeki "IEntityModelBase" kalıtım almış modeller "entityTypes" listesine ekleniyor.
            entityTypes = typeToRegisters.Where(x => typeof(IEntityModelBase).IsAssignableFrom(x));
            if (entityTypes.Any())
            {
                foreach (var type in entityTypes)
                {
                    modelBuilder.Entity(type);
                }
            }
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Bu metot, poco modelleriyle veri tabanı tablolarını eşleştirmektedir.
        /// Normal şartlarda CSBEF yapısı gereği tüm tabloların modül adıyla başlaması gerekmektedir.
        /// Örneğin; UserManagement modülü içerisindeki User tablosunun adı "UserManagement_User" şeklinde olması beklenir.
        /// Bu metot içerisinde de "User" poco modeli ile "UserManagement_User" tablosu eşleştirilir.
        /// Ancak, CSBEF ile hazırlanan API uygulamalarında genellikle "Project" adında main bir modül bulunmaktadır.
        /// Bu main modül, API uygulamasının üretilme amacı olan proje için gerekenleri barındırmaktadır ve tek modül halindedir.
        /// Ayrıca, daha önce hazırlanmış bir projenin, CSBEF alt yapısına çekilmesi de sıklıkla karşılaşılan bir durumdur.
        /// Böyle bir durumda hali hazırdaki tablolarla işlem yapmak gerekmektedir ve bu tabloların ön ekleri yoktur.
        /// Gerek bu durum için, gerekse de her projede "Project" adında main bir modül bulunduğu için, "Project" isimli modüllerin poco modellerine
        /// ön ek kuralı uygulanmamaktadır. Böylece sadece "Project" isimli modülün tablolarında ön ek aranmaz ve direk poco adıyla eşleştirilir.
        /// 
        /// Bunu da örneklemek gerekirse;
        /// "UserManagement" modülü için "User" tablosunun adı "UserManagement_User" olmalıyken, "Project" isimli modülün içerisindeki "Staff" tablosunun adı
        /// "Staff" olarak kalabilmektedir.
        /// 
        /// Bu ön ek esnekte kuralı uygulanacak olan main modül adı, CSBEF "ApiStartOptionsModel" içerisinden değiştirilebilir.
        /// </summary>
        /// <param name="modelBuilder">DbContext için gereken "ModelBuilder" nesnesi</param>
        private static void RegiserConvention(ModelBuilder modelBuilder)
        {
            #region Variables

            string[] nameParts;
            string tableName;

            #endregion Variables

            #region Action Body

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (entity.ClrType.Namespace != null)
                {
                    nameParts = entity.ClrType.Namespace.Split('.');
                    if (nameParts.Length > 2 && nameParts[2].ToLower(GlobalConfiguration.ApiStartOptions.DefaultCultureInfo) == GlobalConfiguration.ApiStartOptions.DefaultMainModuleName)
                    {
                        tableName = entity.ClrType.Name;
                    }
                    else
                    {
                        tableName = string.Concat(nameParts[2], "_", entity.ClrType.Name);
                    }
                    modelBuilder.Entity(entity.Name).ToTable(tableName);
                }
            }

            #endregion Action Body
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Entity modellerin map tanımlarını gerçekleştiren metottur.
        /// Modüllerden elde edilen tipler içerisindeki "IEntityMapper" interface'inden kalıtım almış olan sınıfların "Mapper" metotları tetiklenmektedir.
        /// </summary>
        /// <param name="modelBuilder">DbContext için gereken "ModelBuilder" nesnesi</param>
        /// <param name="typeToRegisters">Modüllerden elde edilmiş olan tipler</param>
        private static void RegisterCustomMappings(ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters)
        {
            IEnumerable<Type> customModelBuilderTypes = null;
            IEntityMapper builder = null;

            customModelBuilderTypes = typeToRegisters.Where(x => typeof(IEntityMapper).IsAssignableFrom(x));
            foreach (var builderType in customModelBuilderTypes)
            {
                if (builderType != null && builderType != typeof(IEntityMapper))
                {
                    builder = (IEntityMapper)Activator.CreateInstance(builderType);
                    builder.Mapper(modelBuilder);
                }
            }
        }
    }
}