using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JSONDatabaseManager.Core;

class Program
{
    static async Task Main()
    {
        // Create a new database
        string dbName = "TestDB";
        string dbPassword = "SecurePass123";
        var volumePath = @"C:\path\to\your\db\folder\";
        JsonDatabase db = new JsonDatabase(dbName, dbPassword, volumePath);

        // Synchronize database with the disk
        await db.InitDiskAsync();
        await db.CreateDocument("Users");

        // Get the 'Users' document
        var usersDocument = db.GetDocument("Users");

        // Insert a new user
        var newItem = usersDocument.Insert(new Dictionary<string, object> { { "name", "John Doe" }, { "age", 30 } });

        // Commit the changes
        await usersDocument.CommitChangesAsync();

        // Update user data
        var updatedData = new Dictionary<string, object> { { "name", "Jane Doe" }, { "age", 28 } };
        bool updateResult = usersDocument.Update(newItem.Id, updatedData);

        // Commit the updated changes
        await usersDocument.CommitChangesAsync();

        // Delete the user
        bool deleteResult = usersDocument.Delete(newItem.Id);

        // Commit the delete operation
        await usersDocument.CommitChangesAsync();

        // Query specific fields
        var queryResult = usersDocument.Query(fields: new List<string> { "name" });

        // Query with filter (age > 25)
        var filteredResult = usersDocument.Query(filter: item => (int)item.Data["age"] > 25);

        // Query with filter and fields (age > 25, select only 'name' field)
        var filteredFieldsResult = usersDocument.Query(
            filter: item => (int)item.Data["age"] > 25,
            fields: new List<string> { "name" }
        );
    }
}
