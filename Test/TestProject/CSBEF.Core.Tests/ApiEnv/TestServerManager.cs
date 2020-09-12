using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace CSBEF.Core.Tests.ApiEnv
{
    /// <summary>
    /// TODO: To be translated into English
    /// Tüm birim testleri için gereken ön hazırlıkların yapıldığı static sınıftır.
    /// Sınıfın static olmasının nedeni; gerekenlerin sadece bir kere oluşmasını garanti altına almaktır.
    /// Testlerin tümü yada biri çalıştırıldığında, buradaki gereksinimler kendini tekrarlamaz, sadece 1 kere oluşturulur.
    /// Özellikle test amaçlı oluşturulan API içerisine yapılan entegrasyon, farklı durumlarda işlemlerini test sayısı kadar arttırabilir.
    /// Bu sınıf bunun önüne geçmek için oluşturulmuştur.
    /// </summary>
    public static class TestServerManager
    {
        private static TestServer server;
        private static HttpClient client;

        /// <summary>
        /// TODO: To be translated into English
        /// Entegrasyon testleri için gereken API uygulamasının instance'ına erişmeyi sağlayan property.
        /// </summary>
        public static TestServer Server
        {
            get
            {
                if (server == null)
                {
                    server = new TestServer(new WebHostBuilder()
                        .UseEnvironment("Development")
                        .UseStartup<TestApiStartup>()
                        .UseConfiguration(new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build()
                        )
                    );


                }

                return server;
            }
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Test amaçlı oluşturulan API uygulamasına, birim testleri içerisinden request atabilmek için kullanılan ve "HttpClient" tipine sahip property.
        /// </summary>
        public static HttpClient Client
        {
            get
            {
                if (client == null)
                {
                    client = Server.CreateClient();
                }

                return client;
            }
        }
    }
}