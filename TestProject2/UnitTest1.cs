using Ts.Application;
using Xunit;
using Xunit.Abstractions;

namespace TestProject2
{
    public class UnitTest1:BaseTest
    {
        [Fact]
        public void Test1()
        {
            var userService = GetService<UserService>();
            Assert.NotNull(userService);
        }
        public UnitTest1(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

}