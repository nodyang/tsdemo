using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ts.Web.Core;
using Xunit.Abstractions;
namespace TestProject2
{
    public class BaseTest
    {
        public readonly ITestOutputHelper _testOutputHelper;

        public HttpClient _client { get; }

        private IServiceProvider ServiceProvider;
        public BaseTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            var server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseConfiguration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile("appsettings.Development.json")
                    .Build()
                )
                .UseStartup<Startup>());

            ServiceProvider= server.Host.Services;
            _client = server.CreateClient();
            
            
        }

        protected T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }
    }
}