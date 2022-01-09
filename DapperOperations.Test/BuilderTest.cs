using DapperOperations.Test.Models;
using Xunit;

namespace DapperOperations.Test
{
    public class BuilderTest
    {
        [Fact]
        public void ShouldGenerateInsertQuery()
        {
            DapperOperation.Map<User3>();
            var query = Builder.BuildInsertStatement<User3>();
            Assert.Equal(@"INSERT INTO user3 (""name"", ""age"") VALUES (@Name, @Age)", query);
        }

        [Fact]
        public void ShouldGenerateUpdateQuery()
        {
            DapperOperation.Map<User3>();
            var query = Builder.BuildUpdateStatement<User3>();
            Assert.Equal(@"UPDATE user3 SET name=@Name,age=@Age WHERE id=@Id", query);
        }

        [Fact]
        public void ShouldGenerateBulkInsertQuery()
        {
            DapperOperation.Map<User3>();
            var query = Builder.BuildBulkInsertStatement<User3>(3);
            Assert.Equal(@"UPDATE user3 SET name=@Name,age=@Age WHERE id=@Id", query);
        }
    }
}
