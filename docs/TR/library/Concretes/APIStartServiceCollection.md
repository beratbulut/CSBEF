# APIStartServiceCollection
Bu sınıf, CSBEF kütüphanesinin API uygulamasına entegre olabilmesi ve modüler yapıyı devreye sokabilmesi için kullanılmaktadır. Şuan için tek kullanım alanı, ilgili API uygulamasının Startup.cs dosyasıdır.

## Init()

```
IServiceProvider Init(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IServiceCollection services, ApiStartOptionsModel options = null)
```

Bu metot CSBEF yapısının tetiklenmesini ve tüm modüllerin devreye sokulmasını sağlar.

Örnek kullanım;

```
...
public IServiceProvider ConfigureServices(IServiceCollection services)
{
    var serviceInit = new APIStartServiceCollection().Init(Configuration, _hostingEnvironment, services);
    return serviceInit;
}
...
```