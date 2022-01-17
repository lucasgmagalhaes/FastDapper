using FastDapper.Test.Models;
using Xunit;

namespace FastDapper.Test
{
    public class BuilderTest
    {
        [Fact]
        public void ShouldGenerateInsertQuery()
        {
            FastManager.Map<User3>();
            var query = Builder.BuildInsertStatement<User3>();
            Assert.Equal(@"INSERT INTO user3 (""name"", ""age"") VALUES (@Name, @Age)", query);
        }

        [Fact]
        public void ShouldGenerateUpdateQuery()
        {
            FastManager.Map<User3>();
            var query = Builder.BuildUpdateStatement<User3>();
            Assert.Equal(@"UPDATE user3 SET name=@Name,age=@Age WHERE id=@Id", query);
        }

        [Fact]
        public void ShouldGenerateBulkInsertQuery()
        {
            FastManager.Map<User3>();
            var query = Builder.BuildUpsertStatement<User3>(3, true, u => new {u.Id, u.Name });
            Assert.Equal(@"INSERT INTO user3 (""Name"", ""Age"") VALUES (@Name_0, @Age_0),(@Name_1, @Age_1),(@Name_2, @Age_2) ON CONFLICT (id,name) DO UPDATE SET name = EXCLUDED.name,age = EXCLUDED.age", query);
        }

        [Fact]
        public void ShouldGenerateBulkInsertQueryAndGetFromCache()
        {
            FastManager.Map<User3>();
            Builder.BuildUpsertStatement<User3>(3, true, u => new { u.Id, u.Name });
            var query2 = Builder.BuildUpsertStatement<User3>(3, true, u => new { u.Id, u.Name });
            Assert.Equal(@"INSERT INTO user3 (""Name"", ""Age"") VALUES (@Name_0, @Age_0),(@Name_1, @Age_1),(@Name_2, @Age_2) ON CONFLICT (id,name) DO UPDATE SET name = EXCLUDED.name,age = EXCLUDED.age", query2);
        }
    }
}
