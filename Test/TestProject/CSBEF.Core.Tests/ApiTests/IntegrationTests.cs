using Xunit;
using CSBEF.Core.Tests.ApiEnv;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace CSBEF.Core.Tests.ApiTests
{
    /// <summary>
    /// TODO: To be translated into English
    /// CSBEF entegrasyonu ile ilgili süreçleri test etmek için kullanılan entegrasyon test sınıfıdır.
    /// Bu sınıf içerisinde, CSBEF'in ilgili API uygulamasına startup.cs içerisinden entegre edilmesiyle ilgili tüm süreçlerin testleri yer almaktadır ve
    /// bunun dışında başka testlerin bu sınıf içerisinde yer almaması önerilir.
    /// </summary>
    public class IntegrationTests
    {
        /// <summary>
        /// TODO: To be translated into English
        /// API uygulamasının başarılı bir şekilde ayağa kaldırılmasını test eden birim testi.
        /// </summary>
        [Fact]
        public void ToIntegration_ShouldRunning()
        {
            Assert.True(true);
        }

        /// <summary>
        /// TODO: To be translated into English
        /// "HttpContextAccessor" instance'ının ServiceProvider içerisine başarılı bir şekilde eklenmesini test eden birim testi.
        /// </summary>
        [Fact]
        public async Task ToIntegration_ShouldInjectedContextAccessor()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/test/CheckHttpContextAccessorInjection");

            // Act
            var response = await TestServerManager.Client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ToIntegration_ShouldCountLoadedModuleStatus200()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/test/CountLoadedModule");

            // Act
            var response = await TestServerManager.Client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Modül kütüphanelerinin reflection ile yüklenmesini test eden birim testi.
        /// </summary>
        [Fact]
        public async Task ToIntegration_ShouldCountLoadedModuleHigherThanZero()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/test/CountLoadedModule");

            // Act
            var response = await TestServerManager.Client.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("2", responseText);
        }

        /// <summary>
        /// TODO: To be translated into English
        /// DbContext instance'ının başarılı bir şekilde oluşmasını test eden birim testi.
        /// </summary>
        [Fact]
        public async Task ToIntegration_ShouldGetDbContextIdStatus200()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/test/GetDbContextContextId");

            // Act
            var response = await TestServerManager.Client.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(!string.IsNullOrWhiteSpace(responseText));
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Test için oluşturulan birinci modülün birinci entity'sinin DbContext içerisine başarılı bir şekilde eklenmesini test eden birim testi.
        /// </summary>
        [Fact]
        public async Task ToIntegration_ShouldGetTestOneEntityInstanceForFakeModuleOne()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/test/GetTestOneEntityInstanceForFakeModuleOne");

            // Act
            var response = await TestServerManager.Client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Test için oluşturulan birinci modülün ikinci entity'sinin DbContext içerisine başarılı bir şekilde eklenmesini test eden birim testi.
        /// </summary>
        [Fact]
        public async Task ToIntegration_ShouldGetTestTwoEntityInstanceForFakeModuleOne()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/test/GetTestTwoEntityInstanceForFakeModuleOne");

            // Act
            var response = await TestServerManager.Client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Test için oluşturulan ikinci modülün birinci entity'sinin DbContext içerisine başarılı bir şekilde eklenmesini test eden birim testi.
        /// </summary>
        [Fact]
        public async Task ToIntegration_ShouldGetTestOneEntityInstanceForFakeModuleTwo()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/test/GetTestOneEntityInstanceForFakeModuleTwo");

            // Act
            var response = await TestServerManager.Client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Test için oluşturulan ikinci modülün ikinci entity'sinin DbContext içerisine başarılı bir şekilde eklenmesini test eden birim testi.
        /// </summary>
        [Fact]
        public async Task ToIntegration_ShouldGetTestTwoEntityInstanceForFakeModuleTwo()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/test/GetTestTwoEntityInstanceForFakeModuleTwo");

            // Act
            var response = await TestServerManager.Client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}