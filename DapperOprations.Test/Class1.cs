using DapperOperations;
using DapperOprations.Test.Models;
using Xunit;

namespace DapperOprations.Test
{
    public class MappingTest
    {
        [Fact]
        public void ShouldMap()
        {
            var mapper = Manager.Map<User1>();
            Assert.Equal("Id", mapper?.KeyMap?.Item1);
        }
    }
}