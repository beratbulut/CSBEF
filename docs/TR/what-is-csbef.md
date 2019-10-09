# CSBEF Nedir
CSBEF, API uygulamalarını modüler şekilde geliştirmeye yarayan bir kütüphanedir.

CSBEF bağımsız hazırlanabilen modüller için alt yapı sağlamanın yanı sıra, dahil olduğu API projesinin de tüm yönetimini ele almaktadır. Bu sayede modüllerin çalışabilmesini ve bir birleriyle haberleşebilmelerini sağlamaktadır.

CSBEF kütüphanesi, API uygulamalarına bağımlılık olarak entegre edildiği gibi, geliştirilecek modüllerde de ana bağımlılık olarak yer almaktadır. Çünkü modüllerin API uygulaması ile entegre çalışabilmeleri ve haberleşmenin sağlanabilmesi için gereken tüm dinamikler, CSBEF.Core kütüphanesinde yer almaktadır.

CSBEF kütüphanesinde birçok tasarım deseni yer almaktadır. Bu sayede geliştirilen modüllerin tümü modern yapılarla geliştirilebilmektedir.

CSBEF kütüphanesinde, modüller için çeşitli yapılar sağladığı gibi, bu amacın dışında da kullanılabilecek durumdadır fakat ana kuruluş amacı bu değildir. İçerisindeki tasarım desenleri için gereken yapıları ve çeşitli araçları kullanmak için bu kütüphaneyi farklı projelerde de kullanabilirsiniz.

## Olumlu Yanları
Bu kütüphaneyi kullanarak API projenizi oluşturduğunuzda;

- Projeyi Class Library kütüphaneleri ile yapabileceğiniz gibi küçük parçalara bölebilirsiniz fakat CSBEF içerisindeki modüller için sağlanan alt yapı sayesinde, alt yapı konusunda herhangi bir vakit kaybetmez, güncel prensiplerle bu parçaları oluşturup bir araya getirerek projeyi tamamlayabilirsiniz.
- İçerisindeki modern ve geçerli tasarım desenleri ve prensipler sayesinde güncel geliştirmeler yapabilirsiniz.
- Alt yapı diye kastedilen konular için (Log'lama yapısı, kullanıcı denetimi, yetki denetimi, hata denetimi, modül iletişim kuralları, modül haberleşme sistemi, alt-üst modül kavramı, vd.) geliştirilmiş yapılar için sürekli geliştirme yapan geliştirmeciler sayesinde bu konuda sürekli güncel kalırsınız.
- Tamamı (ve tüm bağımlılıklarıyla birlikte) Open Source olan bu kütüphaneyi tüm API projelerinizde kullanabilirsiniz ve kimseye bağımlı olmazsınız
- Bu kütüphaneyi temel alarak kendi geliştirme standartlarınızı oluşturabilirsiniz ve projelerinizle ekibinizi bu bağlamda çalıştırabilir, geliştirebilirsiniz.

## Olumsuz Yanları
- Her ne kadar iç kurgusu en iyi şekilde yapılmaya çalışılmış olsa da, içerisinde bir çok yapı bulundurmaktadır ve modüllerin çalıştırılması, yerleştirilmesi için bir çok tetikleme kullanılmaktadır. Bunların gereksiz durumda çalışmamasını sağlayacak mekanizmalar bulunmasına rağmen gerek kütüphane yükü gerekse de bu denetimler nedeniyle küçük projelerde dezavantaj haline gelmektedir. Küçük projelerde işe özel ve pragmatik çözümler, böyle büyük bir yapıyı kullanmaya göre çok daha avantajlı olacaktır.
- Yine küçük projelerde yukarıdaki madde göz ardı edilse bile üretim aşamasına da olumsuz etki yaratacaktır. Çünkü sonuçta orta ve büyük kapsamlı projelerde büyük avantaj sağlamak için çeşitli iş kurallarıyla yapılandırılması gerekmektedir. Bu kurallar özellikle takım çalışmasına büyük katkı sağlanması için oluşturulmuştur. Küçük projeler çoğu zaman 1 geliştiriciyle yapılabildiğinden, bu iş kuralları yazılımın yapım sürecinde olumsuz bir etki yaratacaktır.

Yukarıdaki maddelerde de anlaşılacağı üzere bu kütüphane şuan için küçük projelerde kullanılması pek önerilmeyen bir kütüphanedir. Nedir bu küçük projeler? İçerik yönetimine dahi sahip olmayan kurumsal tanıtım siteleri seviyesindeki projeler, bu kapsamda işaret edilen küçük projelere bir örnektir. Fakat içerisinde menü yönetimi, içerik yönetimi, ve benzeri yönetimlerin bulunduğu projeler, bu kapsamdaki küçük projeler değildir ve bu tarz projelerde bu olumsuz yönlere rağmen kullanmak avantaj sağlayacaktır. Bu avantaj yapım sürecinde de etkisi gösterebileceği gibi özellikle ileriye yönelik büyütülme ihtimali olan bu projelerde bu kütüphaneyi kullanmak, özellikle geniş zaman sürecinde büyük avantaj sağlayacaktır.