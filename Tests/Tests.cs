using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JSONDatabaseManager.Core;
using Xunit;

namespace JSONDatabaseManager.Tests
{
    public class DocumentTests
    {
        // Test Insert functionality
        [Fact]
        public void TestInsert()
        {
            // Arrange
            var document = new Document("Users", "path/to/document");

            var data = new Dictionary<string, object>
            {
                { "name", "John Doe" },
                { "age", 30 }
            };

            // Act
            var newItem = document.Insert(data);

            // Assert
            Assert.NotNull(newItem);
            Assert.Equal("John Doe", newItem.Data["name"]);
            Assert.Equal(30, newItem.Data["age"]);
            Assert.Single(document.Collection);
        }

        // Test Update functionality
        [Fact]
        public void TestUpdate()
        {
            // Arrange
            var document = new Document("Users", "path/to/document");
            var data = new Dictionary<string, object>
            {
                { "name", "John Doe" },
                { "age", 30 }
            };
            var newItem = document.Insert(data);

            // Update data
            var updatedData = new Dictionary<string, object>
            {
                { "name", "Jane Doe" },
                { "age", 28 }
            };

            // Act
            var updateResult = document.Update(newItem.Id, updatedData);

            // Assert
            Assert.True(updateResult);
            Assert.Equal("Jane Doe", newItem.Data["name"]);
            Assert.Equal(28, newItem.Data["age"]);
        }

        // Test Delete functionality
        [Fact]
        public void TestDelete()
        {
            // Arrange
            var document = new Document("Users", "path/to/document");
            var data = new Dictionary<string, object>
            {
                { "name", "John Doe" },
                { "age", 30 }
            };
            var newItem = document.Insert(data);

            // Act
            var deleteResult = document.Delete(newItem.Id);

            // Assert
            Assert.True(deleteResult);
            Assert.Empty(document.Collection);
        }

        // Test CommitChangesAsync functionality (without actual file operations)
        [Fact]
        public async Task TestCommitChangesAsync()
        {
            // Arrange
            var document = new Document("Users", "path/to/document");
            var data = new Dictionary<string, object>
            {
                { "name", "John Doe" },
                { "age", 30 }
            };
            document.Insert(data);

            // Act & Assert
            await document.CommitChangesAsync();

            // Here, we'll test the behavior without checking the actual file system.
            // You can mock or verify file writes using a file I/O mocking framework.
            Assert.Single(document.Collection);
        }

        // Test Query with specific fields (SQL-like)
        [Fact]
        public void TestQueryWithFields()
        {
            // Arrange
            var document = new Document("Users", "path/to/document");
            document.Insert(new Dictionary<string, object> { { "name", "John Doe" }, { "age", 30 } });
            document.Insert(new Dictionary<string, object> { { "name", "Jane Smith" }, { "age", 25 } });

            // Act
            var queryResult = document.Query(fields: new List<string> { "name" });

            // Assert
            Assert.Equal(2, queryResult.Count());
            var results = queryResult.ToList();
            Assert.Contains(results, r => r.ContainsKey("name") && (string)r["name"] == "John Doe");
            Assert.Contains(results, r => r.ContainsKey("name") && (string)r["name"] == "Jane Smith");
        }

        // Test Query with a custom filter (age > 25)
        [Fact]
        public void TestQueryWithFilter()
        {
            // Arrange
            var document = new Document("Users", "path/to/document");
            document.Insert(new Dictionary<string, object> { { "name", "John Doe" }, { "age", 30 } });
            document.Insert(new Dictionary<string, object> { { "name", "Jane Smith" }, { "age", 25 } });

            // Act
            var filteredResult = document.Query(filter: item => (int)item.Data["age"] > 25);

            // Assert
            Assert.Single(filteredResult);
            var result = filteredResult.First();
            Assert.Equal("John Doe", result["name"]);
            Assert.Equal(30, result["age"]);
        }

        // Test Query with filter and specific fields (age > 25, select only name)
        [Fact]
        public void TestQueryWithFilterAndFields()
        {
            // Arrange
            var document = new Document("Users", "path/to/document");
            document.Insert(new Dictionary<string, object> { { "name", "John Doe" }, { "age", 30 } });
            document.Insert(new Dictionary<string, object> { { "name", "Jane Smith" }, { "age", 25 } });

            // Act
            var filteredFieldsResult = document.Query(
                filter: item => (int)item.Data["age"] > 25,
                fields: new List<string> { "name" }
            );

            // Assert
            Assert.Single(filteredFieldsResult);
            var result = filteredFieldsResult.First();
            Assert.Equal("John Doe", result["name"]);
            Assert.DoesNotContain("age", result);
        }
    }
}
