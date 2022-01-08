using DapperOperations.Mapping;
using DapperOperations.Test.Models;
using System.Linq;
using System.Reflection;
using Xunit;

namespace DapperOperations.Test
{
    public class ManagerTest
    {
        [Fact]
        public void ShouldGetKey()
        {
            var mapper = Manager.Map<User1>();
            Assert.Equal("id", mapper?.KeyMap?.Item2);
        }

        [Fact]
        public void ShouldGetAllProperties()
        {
            var mapper = Manager.Map<User1>();
            AssertUser1(mapper);
        }

        [Fact]
        public void ShouldGetAllPropertiesWithDataIgnored()
        {
            var mapper = Manager.Map<User2>();
            AssertUser1(mapper);
            User2 user;
            string? items = null;
            mapper?.ColumnsMap.TryGetValue(nameof(user.Items), out items);
            Assert.Null(items);
        }

        [Fact]
        public void ShouldMapFromDirectlyType()
        {
            var mapper = Manager.Map(typeof(User1));
            AssertUser1(mapper);
        }

        [Fact]
        public void ShouldMapForGetOrMappMethod()
        {
            var mapper = Manager.GetOrAdd(typeof(User1));
            AssertUser1(mapper);
        }

        [Fact]
        public void ShouldMapForGetOrMappMethodGeneric()
        {
            var mapper = Manager.GetOrAdd<User1>();
            AssertUser1(mapper);
        }

        [Fact]
        public void ShouldReturnTrueForMappedEntity()
        {
            Manager.GetOrAdd<User1>();
            Assert.True(Manager.IsEntityMapped(typeof(User1)));
        }

        [Fact]
        public void ShouldReturnTrueForMappedEntityGeneric()
        {
            Manager.GetOrAdd<User1>();
            Assert.True(Manager.IsEntityMapped<User1>());
        }

        [Fact]
        public void ShouldGetEntity()
        {
            Manager.Map<User1>();
            Assert.NotNull(Manager.Get<User1>());
        }

        [Fact]
        public void ShouldGetEntityGeneric()
        {
            Manager.Map<User1>();
            Assert.NotNull(Manager.Get(typeof(User1)));
        }

        [Fact]
        public void ShouldMapFromListOfType()
        {
            var mapper = Manager.Map(Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(a => a.Namespace == "DapperOperations.Test.Models").ToList())[0];
            AssertUser1(mapper);
        }

        [Fact]
        public void ShouldMapFromAssembly()
        {
            var mapper = Manager.Map(typeof(User1).Assembly, "DapperOperations.Test.Models")[0];
            AssertUser1(mapper);
        }

        [Fact]
        public void ShouldMapFromAssemblyName()
        {
            var mapper = Manager.MapFromAssemblyName("DapperOperations.Test", "DapperOperations.Test.Models")[0];
            AssertUser1(mapper);
        }

        private static void AssertUser1(MappedEntity? mapper)
        {
            User1? user;

            var name = "";
            var age = "";
            var createdDate = "";

            mapper?.ColumnsMap.TryGetValue(nameof(user.Name), out name);
            mapper?.ColumnsMap.TryGetValue(nameof(user.CreatedDate), out createdDate);
            mapper?.ColumnsMap.TryGetValue(nameof(user.Age), out age);

            Assert.Equal("name", name);
            Assert.Equal("age", age);
            Assert.Equal("createdDate", createdDate);
        }
    }
}