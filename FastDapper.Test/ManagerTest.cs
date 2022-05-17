using FastDapper.Test.Models;
using Xunit;

namespace FastDapper.Test
{
    public class ManagerTest
    {
        [Fact]
        public void ShouldReturnTrueForMappedEntity()
        {
            FastManager.GetOrAdd<User1>();
            Assert.True(FastManager.IsEntityMapped(typeof(User1)));
        }

        [Fact]
        public void ShouldReturnTrueForMappedEntityGeneric()
        {
            FastManager.GetOrAdd<User1>();
            Assert.True(FastManager.IsEntityMapped<User1>());
        }

        [Fact]
        public void ShouldGetEntity()
        {
            FastManager.Map<User1>();
            Assert.NotNull(FastManager.Get<User1>());
        }

        [Fact]
        public void ShouldGetEntityGeneric()
        {
            FastManager.Map<User1>();
            Assert.NotNull(FastManager.Get(typeof(User1)));
        }

        [Fact]
        public void ShouldMapWithDifferentAproats()
        {
            FastManager.Map<User1>();
            FastManager.Map(typeof(User1));
            var val = FastManager.Get(typeof(User1));
            Assert.NotNull(val);
        }
    }
}