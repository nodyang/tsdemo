using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Furion;
using Ts.Application;
using Ts.Web.Core;
using Xunit;
using Xunit.Abstractions;

namespace TestProject2
{
    public class UnitTest1:BaseTest
    {
        [Fact]
        public void Test1()
        {
            var userService = GetService<IUserService>();
            Assert.NotNull(userService);
            var users = userService.GetAll();
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(users));
        }

        [Fact]
        public void TestSing()
        {
            SinTest sinTest = GetService<SinTest>();
            Assert.NotNull(sinTest);
            _testOutputHelper.WriteLine(sinTest.GetHashCode().ToString());
            _testOutputHelper.WriteLine(sinTest.Name.ToString());
            Thread.Sleep(2000);
            var siTest= App.GetService<SinTest>();
            Assert.NotNull(sinTest);
            _testOutputHelper.WriteLine(siTest.Name.ToString());
            
            Thread.Sleep(2000);
            IServiceProvider ser = GetService<IServiceProvider>();
            Assert.NotNull(ser);
            _testOutputHelper.WriteLine(((SinTest)ser.GetService(typeof(SinTest)))?.Name.ToString());
            
            
            Thread.Sleep(2000);
            var siTestss= App.GetService<SinTest>(ser);
            Assert.NotNull(siTestss);
            _testOutputHelper.WriteLine(siTestss.GetHashCode().ToString());
            _testOutputHelper.WriteLine(siTestss.Name.ToString());
            
        }
        [Fact]
        public async Task TestHttp()
        {
            //
            var @async = await _client.GetAsync("https://localhost:5001/api/system/description");
            var readAsStringAsync =await @async.Content.ReadAsStringAsync();
            _testOutputHelper.WriteLine(readAsStringAsync);
        }
        public UnitTest1(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

}