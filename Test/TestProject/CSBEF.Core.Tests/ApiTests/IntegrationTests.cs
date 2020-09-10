using Xunit;
using CSBEF.Core.Tests.ApiEnv;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace CSBEF.Core.Tests.ApiTests
{
    public class IntegrationTests
    {
        [Fact]
        public void ToIntegration_ShouldRunning()
        {
            Assert.True(true);
        }

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