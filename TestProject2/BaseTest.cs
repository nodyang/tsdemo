using System;
using System.Net.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
               // .UseStartup<Startup>().UseSerilog()
            var webHostBuilder = WebHost.CreateDefaultBuilder().Inject()
                .UseEnvironment("Development")
                .UseConfiguration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile("appsettings.Development.json")
                    .Build()
                ).UseStartup<Startup>();
            var server = new TestServer(webHostBuilder);

            ServiceProvider= server.Host.Services;
            _client = server.CreateClient();
            
            
        }

        protected T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }
    }
}