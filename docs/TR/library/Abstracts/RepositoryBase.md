# RepositoryBase.cs
Bu sınıf Generic Repository Pattern için kullanılan base sınıftır.
Sizler için bu sınıfı düşünebildiğimiz tüm ihtiyaçları giderebilecek şekilde tasarladık. Yaptığımız incelemelerde genel olarak kullanılan benzer base sınıflar da bu şekilde kullanılmaktadır.

## Kod Yapısı
```
using AutoMapper;
using CSBEF.Core.Concretes;
using CSBEF.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CSBEF.Core.Abstracts
{
    public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
        where TEntity : class, IEntityModelBase
    {
        #region Dependencies

        protected ModularDbContext _context;
        private IMapper _mapper;

        #endregion Dependencies

        #region Construction

        public RepositoryBase(ModularDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #endregion Construction

        #region Actions

        public virtual IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().AsNoTracking();
        }

        public virtual async Task<ICollection<TEntity>> GetAllAsyn()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public virtual TEntity Add(TEntity t)
        {
            var entity = _context.Set<TEntity>().Add(t).Entity;
            return entity;
        }

        public virtual TEntity Find(Expression<Func<TEntity, bool>> match)
        {
            return GetAll().FirstOrDefault(match);
        }

        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match)
        {
            return await GetAll().FirstOrDefaultAsync(match);
        }

        public virtual ICollection<TEntity> FindAll(Expression<Func<TEntity, bool>> match)
        {
            return GetAll().Where(match).ToList();
        }

        public virtual async Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match)
        {
            return await GetAll().Where(match).ToListAsync();
        }

        public virtual void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public virtual TEntity Update(TEntity t)
        {
            _context.Entry(t).State = EntityState.Modified;

            return t;
        }

        public virtual int Count()
        {
            return GetAll().Count();
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).Count();
        }

        public virtual async Task<int> CountAsync()
        {
            return await GetAll().CountAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).CountAsync();
        }

        public virtual void Save()
        {
            _context.SaveChanges();
        }

        public virtual async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public virtual IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> queryable = GetAll();
            foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
            {
                queryable = queryable.Include(includeProperty);
            }

            return queryable;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Actions
    }
}
```

Pattern açısından oluşturulması gereken her Repository sınıfının bu base sınıftan türetilmesi gerekir. Ayrıca her bir Repository sınıfının temel aldığı bir POCO modeli olmalıdır. Kod yapısına da bakıldığında bu abstract sınıf kullanılarak türetilecek tüm Repository sınıfları bir POCO modelini işaret etmelidir. Belirtilen modellerin POCO modeli olup olmadığı ise **IEntityModelBase** Interface'inden anlaşılmaktadır.

## GetAll()

```
public virtual IQueryable<TEntity> GetAll()
```

Bu metot ilgili tablodaki tüm verileri hedef almaktadır fakat bilgiler henüz çekilmemiştir, sorgu oluşturulma aşamasındadır. Bu metot ile elde edeceğiniz nesne, henüz verisi çekilmemiş olan sorgu nesnesidir. Elde edeceğiniz **IQueryable** nesnesini herhangi bir şekilde somut biçime dönüştürürseniz (liste veya başka bir modele), dönüştürme işlemi aşamasında ilgili veriler çekilecek ve dönüştürmek istediğiniz nesneye aktarılacaktır.

Bu metot genelde büyük sorgular oluşturmak için başlangıç noktası olarak kullanılır.

Örnek Kullanım:

```
var myName = "Mesut Kurak";
var data = _userRepository_.GetAll().Where(i => i.Name == myName);
```

Yukarıdaki örnekte bulunan **data** değişkeni için henüz veri tabanından veri çekilmedi. Fakat aşağıdaki gibi bir kod ile bu değişken somut hale getirilirse, sorgu veri tabanına gönderilecek ve varsa veri çekilecek.

```
return data.Name + " " + data.Surname;
```

## GetAllAsyn()

**GetAll()** metodu gibi tüm verileri hedef alan ancak farklı olarak sorguyu direk veri tabanına gönderip sonuçları çeken bir metottur. Asenkron olarak çalışan bu metot aşağıdaki şekilde return yapar;

```
return await _context.Set<TEntity>().ToListAsync();
```

Yani bu metodu kullandığınızda elde edeceğiniz nesne liste nesnesi olacaktır.

## Add()

```
public virtual TEntity Add(TEntity t)
{
    var entity = _context.Set<TEntity>().Add(t).Entity;
    return entity;
}
```

İlgili tabloya yeni bir veri eklemek için kullanılan metottur. Abstract sınıftan kalıtım alınırken belirtilen POCO modeliyle parametre kabul eder.

Ekleme işlemini hafızaya yapar ve değişiklikler veri tabanına aktarılmaz. Dönüş için hafızaya eklenen model işlenerek geri gönderilir.

Örnek Kullanım;

```
var newUser = _userRepository.Add(new User{
    Name = "Mesut",
    Surname = "Kurak",
    Email = "mesut.kurak@codescientific.com",
    Password = "xxXX000."
});
```

Eğer bu işlemden sonra ilgili Repository üzerinden değişiklikleri kaydetmeye yarayan metot tetiklenmezse, veri nesnel olarak veri tabanına eklenmemiş olacak.

```
var newUser = _userRepository.Add(new User{
    Name = "Mesut",
    Surname = "Kurak",
    Email = "mesut.kurak@codescientific.com",
    Password = "xxXX000."
});
await _userRepository.SaveAsync();
return newUser.Id; 
```

Gibi...

## Find()

```
public virtual TEntity Find(Expression<Func<TEntity, bool>> match)
{
    return GetAll().FirstOrDefault(match);
}
```

Veri tabanından bir kayıt aramak için kullanılır ve geriye sadece 1 kayıt döndürür. Eğer arama sonucunda kayıt bulunamazsa geriye **null** dönülür.

Metot senkron olarak çalışmaktadır.

Örnek;

```
var getMyUserRecord = _userRepository.Find(i => i.Name == "Mesut Kurak");
return getMyUserRecord == null ? 0 : getMyUserRecord.Id;
```

## FindAsync()

```
public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match)
{
    return await GetAll().FirstOrDefaultAsync(match);
}
```

**Find** metodunun asenkron versiyonudur.

Örnek kullanım:

```
var getMyUserRecord = await _userRepository.FindAsync(i => i.Name == "Mesut Kurak");
return getMyUserRecord == null ? 0 : getMyUserRecord.Id;
```

## FindAll()

```
public virtual ICollection<TEntity> FindAll(Expression<Func<TEntity, bool>> match)
{
    return GetAll().Where(match).ToList();
}
```

Verilen filtreye göre bulunan tüm kayıtların listesini döndüren senkron metoddur. Bu metodun **GetAll** metodundan farkı filtre uygulanabilmesidir.

Örnek Kullanım:

```
var findUsers = _userRepository.FindAll(i => i.Name == "Mesut");
return findUsers.Count();
```

## FindAllAsync()

```
public virtual async Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match)
{
    return await GetAll().Where(match).ToListAsync();
}
```

**FindAll** metodunun asenkron versiyonudur.

Örnek kullanım;

```
var findUsers = await _userRepository.FindAllAsync(i => i.Name == "Mesut");
return findUsers.Count();
```

## Delete()

```
public virtual void Delete(TEntity entity)
{
    _context.Set<TEntity>().Remove(entity);
}
```

Tablodan bir veriyi silmek için kullanılır. Fakat **Save** veya **SaveAsync** metotlarından biri çalıştırılmazsa, kayıt fiziki olarak tablodan silinmeyecektir.

Örnek kullanım;

```
var getUser = _userRepository.Find(i => i.Id == 1);
if(getUser != null){
    _userRepository.Delete(getUser);
    _userRepository.Save();
}
```

## Update()

```
public virtual TEntity Update(TEntity t)
{
    _context.Entry(t).State = EntityState.Modified;

    return t;
}
```

Tablodaki bir veriyi güncellemek için kullanılır. Fakat bu güncelleme RAM'de gerçekleştirilir ve fiziki olarak tabloya yansıtılmaz. Bunun için **Save** metodu veya **SaveAsync** metodu tetiklenmektedir.

Örnek kullanım;

```
var getUser = _userRepository.Find(i => i.Id == 1);
if(getUser != null){
    getUser.Email = "mesut.kurak@codescientific.com";
    getUser = _userRepository.Update(getUser);
    _userRepository.Save();
}
```

## Count()

```
public virtual int Count()
{
    return GetAll().Count();
}
```

Tablodaki tüm kayıtların sayısını verir. Herhangi bir filtre uygulanamaz, tüm kayıtlar dikkate alınarak toplam rakam dönülür.

Örnek kullanım;

```
var userCount = _userRepository.Count();
return userCount > 0 ? "Kayıtlı kullanıcı var" : "Kayıtlı kullanıcı yok";
```

## Count(predicate)

```
public virtual int Count(Expression<Func<TEntity, bool>> predicate)
{
    return GetAll().Where(predicate).Count();
}
```

Bu metot filtre uygulanabilir versiyondur. Diğer metoda filtre uygulanamadığı için tablodaki toplam kayıt sayısını elde etmek için başvurulmaktadır fakat filtre uygulanmak isteniyorsa bu sefer de devreye metodun bu cinsi devreye girecektir.

Örnek kullanım;

```
var userCount = _userRepository.Count(i => i.Status == false);
return userCount > 0 ? "Pasif kullanıcı var" : "Pasif kullanıcı yok";
```

## CountAsync(predicate)

```
public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
{
    return await GetAll().Where(predicate).CountAsync();
}
```

Filtre uygulanabilir **Count** metodunun asenkron versiyonudur.

Örnek kullanım;

```
var userCount = await _userRepository.CountAsync(i => i.Status == false);
return userCount > 0 ? "Pasif kullanıcı var" : "Pasif kullanıcı yok";
```

## Save()

```
public virtual void Save()
{
    _context.SaveChanges();
}
```

Hafızada tutulan veri değişikliklerini tabloya fiziki olarak işler. Metot senkron çalışmaktadır.

**Add**, **Update** ve **Delete** metotları değişiklikleri RAM'de gerçekleştirir ve henüz bu değişiklikler tabloya fiziki olarak işlenmez. Eğer bu metot çalıştırılmadan süreç sonlandırılırsa, yapılan değişiklikler tabloya işlenmeden yok olacaktır. Bu nedenle bu değişiklikler sonrasında muhakkak **Save** metodunu (veya asenkron çalışan **SaveAsync** metodunu) çalıştırın.

Ayrıca, eğer yapacağınız değişiklikler bir döngü içerisinde gerçekleştiriliyorsa, örneğin;

```
var requestData = new List{
    new User{
        Name = "Mesut",
        Kurak = "Kurak",
        Email = "mesut.kurak@codescientific.com",
        Password = "xxXX000."
    },
    new User{
        Name = "Yeşim",
        Kurak = "Kurak",
        Email = "yesim.kurak@codescientific.com",
        Password = "xxXX000."
    }
};

foreach(var user in requestData){
    _userRepository.Add(user);
}

await _userRepository.SaveAsync();
// veya _userRepository.Save();
```

Eğer döngü içerisinde save metodunu tetiklerseniz, Entity Framework kısmında bir hata ile karşılaşabilirsiniz. Bu CSBEF dışında oluşan bir hata durumudur.

Bu kısımda önemli bir başka durum daha bulunmakta;
Örneklerde hep bir repository instance'ı üzerinden kodlar yazılmıştır ve yine aynı instance üzerinden **Save** metodu tetiklenmektedir (veya **SaveAsync**). Peki bir servis action'ı içerisinde birden fazla Repository instance'ı kullanılabilir mi? Evet kullanılabilir. Peki aynı anda birden fazla Repository üzerinden değişiklik yapılırsa, hangi Repository instance'ı üzerinden bu metot tetiklenerek değişikliklerin kaydedilmesi sağlanacaktır?

Bu sorunun yanıtını biraz kapsamlı verelim...

CSBEF'in önceki sürümlerinde **Unit of Work** tasarım deseni kullanılmaktaydı. Bu nedenle servis içerisinde ihtiyaç duyulan tüm Repository instance'larına bu **Unit of Work** metodu üzerinden erişiliyordu. Fakat CSBEF ile yapılan bir çok proje sonrasında kendimizin ve kullanan diğer geliştirmeciler ile yaptığımız fikir alış verişinde, kullanılan Service Provider üzeründen oluşturulan Repository ve DBContext instance'ları varken neden birde böyle bir yük bindiriliyor? Bu durum neticesinde yöntem değiştirdik ve bu versiyonlarda (ve sonrasında) kullanılan yapı şu şekildedir;
Tüm Repository instance'ları Service Provider içerisinde scope olarak oluşturulmaktadır. Aynı zamanda DBContext'i de scope olarak oluşturulmaktadır. Bu Repository ve Scope instance'larının **scope** veya başka türlü olmasını yönetebiliyorsunuz. Fakat her ikisinin de aynı türde oluşturulması gerekmektedir. Bu nedenle zaten Repository ve DBContext instance'ları Service Provider içerisinde oluşturulmuşken neden birde **UOF** yapısı oluşturulsun?

Bu yukarıdaki konu elbette yoruma açık bir konudur. Fakat şuan için alınan karar neticesinde CSBEF yapısından **Unit of Work** deseni çıkarılmıştır.

Bu nedenle servis içerisinde ihtiyacınız olan tüm Repository instance'larına Construction içerisinde Dependency olarak veya IServiceProvider üzerinden erişerek elde edebilir ve kullanabilirsiniz. Birden fazla yapılan değişiklikleri kaydetmek için de herhangi birisi üzerinden **Save** veya **SaveAsync** metodunu kullanın. Çünkü tüm Repository instance'ları yine aynı Dependecy mantığıyla ihtiyaç duyulan DBContext'e tek instance üzerinden erişmekte ve kullanmaktadır.

## SaveAsync()

```
public virtual async Task<int> SaveAsync()
{
    return await _context.SaveChangesAsync();
}
```

Yukarıdaki **Save** metodunun asenkron çalışan versiyonudur.

Örnek kullanım;

```
var requestData = new List{
    new User{
        Name = "Mesut",
        Kurak = "Kurak",
        Email = "mesut.kurak@codescientific.com",
        Password = "xxXX000."
    },
    new User{
        Name = "Yeşim",
        Kurak = "Kurak",
        Email = "yesim.kurak@codescientific.com",
        Password = "xxXX000."
    }
};

foreach(var user in requestData){
    _userRepository.Add(user);
}

await _userRepository.SaveAsync();
```

## GetAllIncluding()

```
public virtual IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
{
    IQueryable<TEntity> queryable = GetAll();
    foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
    {
        queryable = queryable.Include(includeProperty);
    }

    return queryable;
}
```

Zincirleme sorgu oluşturmak için kullanılan metoddur. Bu metot ile Foraign bağlantısı olan diğer alanlar üzerinden diğer tabloları sorgunuza ekleyebilirsiniz.

Elbette servis içerisindeki aksiyonları oluştururken belli bir algoritmanız olacak ve bu algoritma içerisinde yer yer karmaşık sorgulara ihtiyacınız olabilir. Fakat bunlar için mümkün mertebe **RepositoryBase** sınıfından kalıtım alarak oluşturacağınız **Repository** sınıflarınız içerisinde özel metodlar oluşturmak daha doğru bir yaklaşım olabilir. Böylece hem geliştirici tarafından algılanması hemde yönetimi daha rahat olacaktır. Bu bir yorumdur, bir zorunluluk değildir. Fakat karmaşık sorgular için **Repository** sınıflarınız içerisinde özel metotlar geliştirmezseniz, **Generic Repository Pattern** tasarım deseninin tam olarak uygulamış veya bundan tam olarak verim sağlamış olamamız durumu söz konusu olabilir.

Örnek kullanım;

```
var findUserInGroupData = await _userInGroupRepository.GetAllIncluding(i => i.User).Where(i => i.User.Name == "Mesut").ToListAsync();
return findUserInGroupData;
```

## Dispose
**RepositoryBase** sınıfı içerisinde yok edici metot bulundurmaktadır. Kullanım şekli olarak yokedici metot kullanılmamaktadır fakat bir şekilde gerekebilir diye sınıf içerisine yerleştirilmiştir.

