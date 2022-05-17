using FastDapper.Test.Models;
using Xunit;

namespace FastDapper.Test
{
    public class BuilderTest
    {
        [Fact]
        public void ShouldGenerateInsertQuery()
        {
            var query = Builder.BuildInsertStatement<User3>();
            Assert.Equal(@"insert into user3 (""name"", ""age"") values (@Name, @Age)", query);
        }

        [Fact]
        public void ShouldGenerateUpdateQuery()
        {
            var query = Builder.BuildUpdateStatement<User3>();
            Assert.Equal(@"update user3 set name=@Name,age=@Age where id=@Id", query);
        }

        [Fact]
        public void ShouldGenerateBulkUpsertQuery()
        {
            var query = Builder.BuildUpsertStatement<User3>(3, true, u => new { u.Id, u.Name });
            Assert.Equal(
                @"insert into user3 (""Name"", ""Age"") values " +
                "(@Name_0, @Age_0),(@Name_1, @Age_1),(@Name_2, @Age_2) " + 
                "on conflict (id,name) do update set name = excluded.name,age = excluded.age", query);
        }

        [Fact]
        public void ShouldGenerateBulkInsertQueryAndGetFromCache()
        {
            // Add to cache
            Builder.BuildUpsertStatement<User3>(3, true, u => new { u.Id, u.Name });
            var query2 = Builder.BuildUpsertStatement<User3>(3, true, u => new { u.Id, u.Name });
            Assert.Equal(@"insert into user3 (""Name"", ""Age"") values (@Name_0, @Age_0),(@Name_1, @Age_1),(@Name_2, @Age_2) on conflict (id,name) do update set name = excluded.name,age = excluded.age", query2);
        }

        [Fact]
        public void ShouldGenerateCountQueryWithoutParam()
        {
            var query = Builder.BuildCountQuery<User3>();
            Assert.Equal("select count(1) from user3", query);
        }

        [Fact]
        public void ShouldGenerateCountQueryWithOneParam()
        {
            var query = Builder.BuildCountQuery<User3>(new { Id = 1 });
            Assert.Equal("select count(1) from user3 where id = @Id", query);
        }


        [Fact]
        public void ShouldGenerateCountQueryWithMultipleParams()
        {
            var query = Builder.BuildCountQuery<User3>(new { Id = 1, Name = "batata" });
            Assert.Equal("select count(1) from user3 where id = @Id and name = @Name", query);
        }

        [Fact]
        public void ShouldGenerateDeleteAllQuery()
        {
            var query = Builder.BuildDeleteAllQuery<User3>();
            Assert.Equal("delete from user3", query);
        }

        [Fact]
        public void ShouldGenerateTruncateQuery()
        {
            var query = Builder.BuildTruncateQuery<User3>();
            Assert.Equal("truncate table user3", query);
        }

        [Fact]
        public void ShouldGenerateBuildDeleteByIdQuery()
        {
            var query = Builder.BuildDeleteByIdQuery<User3>();
            Assert.Equal("delete from user3 where id = @Id", query);
        }
    }
}
