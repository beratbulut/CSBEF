using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace CSBEF.Core.Tests.ApiEnv
{
    public static class TestServerManager
    {
        private static TestServer server;
        private static HttpClient client;

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