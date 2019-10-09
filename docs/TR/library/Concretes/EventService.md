# EventService

CSBEF.Core, modüller arasındaki iletişimi sağlamak için **Event Draven Pattern** sunmaktadır. Modüllerin tümünden ilk önce kendi Event tanımları alınır. Daha sonrasında yine tüm modüllere toplanan bu Event'ların bulunduğu liste gönderilerek dahil olmak istediklerine dahil olmalarını ister. Böylece gerek modüller arası iletişim, gerekse de alt-üst modül kavramları gerçekleştirilmektedir.

CSBEF.Core, ServiceCollection içerisine bu sınıfın bir instance'ını yerleştirir ve içerisini yukarıdaki şekilde doldurur. Normal şartlarda bu entegrasyon süresi sonrasında herhangi bir modülün veya modül içerisindeki servislerin bu instance'a erişmesine gerek yoktur. Fakat özel ihtiyaçlar neticesinde eğer böyle bir ihtiyaç duyulursa, Container'dan bu instance'a erişilebilir.

## GetEvent

```
public IEventModel GetEvent(string moduleName, string eventName)
```

Modüllerden toplanılan Event listesinden birine erişmek için kullanılan metottur. 

Örnek kullanım;

```
var getUserAddBeforeEvent = _eventService.GetEvent("UserManagement", "UserManagement.Add.Before");
...
Console.WriteLine(getUserAddBeforeEvent.EventInfo.EventName);
Console.WriteLine(getUserAddBeforeEvent.EventInfo.ModuleName);
Console.WriteLine(getUserAddBeforeEvent.EventInfo.ServiceName);
Console.WriteLine(getUserAddBeforeEvent.EventInfo.ActionName);
Console.WriteLine(getUserAddBeforeEvent.EventInfo.DenyHubUse);
Console.WriteLine(getUserAddBeforeEvent.EventInfo.AccessHubs.Count());
Console.WriteLine(getUserAddBeforeEvent.EventInfo.EventType.ToString());
...
```

## AddEvent

```
public void AddEvent(string eventName, string moduleName, string serviceName, string actionName, EventTypeEnum eventType, bool denyHubUse = false, List<string> accessHubs = null)
```

Event havuzuna yeni bir Event ekler. Parametre açıklamaları;

- **eventName:** Event için benzersiz isim. Burada belirtilecek Event adının diğer Event tanımlarındaki isimlerle çakışmaması gerekir (özellikle aynı modülün Event tanımları arasında).
- **moduleName:** İlgili modülün adı
- **serviceName:** İlgili servisin adı
- **actionName:** İlgili metodun adı
- **eventType:** Event'ın metot içerisindeki yerini belirten değerdir.
- **EventTypeEnum:** tipinde olan bu değer "before" ve "after" şeklinde iki seçenek sunmaktadır. 
- **denyHubUse:** Henüz geliştirilmesi devam eden bir özellik için şimdiden eklenmiş bir bilgidir. Bu parametre için ilgili özellik tamamlanana kadar **false** gönderilmelidir.
- **accessHubs:** Henüz geliştirilmesi devam eden bir özellik için şimdiden eklenmiş bir bilgidir. Bu parametre için ilgili özellik tamamlanana kadar **null** gönderilmelidir.

## GetAllEvents

```
public List<IEventModel> GetAllEvents()
```

Event havuzundaki tüm Event tanımlarını veren metottut.