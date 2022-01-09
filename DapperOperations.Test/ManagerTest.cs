using DapperOperations.Test.Models;
using Xunit;

namespace DapperOperations.Test
{
    public class ManagerTest
    {
        [Fact]
        public void ShouldReturnTrueForMappedEntity()
        {
            DapperOperation.GetOrAdd<User1>();
            Assert.True(DapperOperation.IsEntityMapped(typeof(User1)));
        }

        [Fact]
        public void ShouldReturnTrueForMappedEntityGeneric()
        {
            DapperOperation.GetOrAdd<User1>();
            Assert.True(DapperOperation.IsEntityMapped<User1>());
        }

        [Fact]
        public void ShouldGetEntity()
        {
            DapperOperation.Map<User1>();
            Assert.NotNull(DapperOperation.Get<User1>());
        }

        [Fact]
        public void ShouldGetEntityGeneric()
        {
            DapperOperation.Map<User1>();
            Assert.NotNull(DapperOperation.Get(typeof(User1)));
        }
    }
}