using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using CSBEF.Core.Tests.ApiEnv;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace CSBEF.Core.Tests.ApiTests
{
    public class IntegrationTests
    {
        private readonly HttpClient client;

        public IntegrationTests()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<TestApiStartup>()
            );

            client = server.CreateClient();
        }

        [Fact]
        public void ToCtorServer_SholdRunning()
        {
            Assert.True(true);
        }

        [Fact]
        public async Task ToCtorServer_SholdInjectedContextAccessor()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/test/CheckHttpContextAccessorInjection");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ToCtorServer_SholdCountLoadedModuleStatus200()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/test/CountLoadedModule");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ToCtorServer_SholdCountLoadedModuleHigherThanZero()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/test/CountLoadedModule");

            // Act
            var response = await client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("2", await response.Content.ReadAsStringAsync());
        }
    }
}