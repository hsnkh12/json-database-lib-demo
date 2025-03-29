
#JSONDatabaseManager 

JSONDatabaseManager is a simple Object-Relational Mapping (ORM) library that allows for easy manipulation of JSON-based data documents. It supports common database operations like `Insert`, `Update`, `Delete`, `Commit`, and query-like functionality, all without the need for a relational database system.

## Features

- **Insert Data**: Add new records to your documents.
- **Update Data**: Modify existing records.
- **Delete Data**: Remove records from documents.
- **Commit Changes**: Save changes to the disk.
- **Query Data**: Select specific fields or apply filters similar to SQL queries.

## Installation

1. **Clone the Repository**:

   If you're planning to use this library in your own project, you can clone the repository using Git:

   ```bash
   git clone https://github.com/yourusername/JSONDatabaseManager.git
   ```

2. **Install Dependencies**:

   Ensure you have .NET installed. You can download and install it from [here](https://dotnet.microsoft.com/download). Then, navigate to the project directory and install the necessary dependencies by running:

   ```bash
   dotnet restore
   ```

3. **Build the Project**:

   After restoring dependencies, you can build the project:

   ```bash
   dotnet build
   ```

## Usage

### Creating a Database

You can create a `JsonDatabase` by specifying a name, password, and volume path (where your database files will be stored):

```csharp
string dbName = "TestDB";
string dbPassword = "SecurePass123";
var volumePath = "C:\path\to\your\db\folder\";

JsonDatabase db = new JsonDatabase(dbName, dbPassword, volumePath);
await db.SyncFromDiskAsync();
```

### Working with Documents

Documents in `JSONDatabaseManager` are analogous to tables in traditional databases. Each document is represented by a `Document` object, and you can perform operations like Insert, Update, and Delete on it.

#### Insert Data

To insert a new record into a document, create a dictionary of field names and values, and call the `Insert` method:

```csharp
var usersDocument = db.GetDocument("Users");
var newItem = usersDocument.Insert(new Dictionary<string, object> { { "name", "John Doe" }, { "age", 30 } });
```

#### Update Data

To update an existing record, use the `Update` method and provide the item's ID and the updated data:

```csharp
var updatedData = new Dictionary<string, object> { { "name", "Jane Doe" }, { "age", 28 } };
bool updateResult = usersDocument.Update(newItem.Id, updatedData);
```

#### Delete Data

To delete a record by ID, use the `Delete` method:

```csharp
bool deleteResult = usersDocument.Delete(newItem.Id);
```

#### Commit Changes

After making changes (inserts, updates, deletes), call the `CommitChangesAsync` method to save the changes to the disk:

```csharp
await usersDocument.CommitChangesAsync();
```

### Querying Data

The library supports querying similar to SQL. You can query specific fields or apply filters:

#### Query Specific Fields

You can select specific fields like `name` by providing a list of field names:

```csharp
var queryResult = usersDocument.Query(fields: new List<string> { "name" });
```

#### Query with Custom Filter

You can apply custom filters to query specific data. For example, to find users older than 25 years:

```csharp
var filteredResult = usersDocument.Query(filter: item => (int)item.Data["age"] > 25);
```

#### Query with Filter and Specific Fields

You can combine filtering and selecting specific fields in your queries:

```csharp
var filteredFieldsResult = usersDocument.Query(
    filter: item => (int)item.Data["age"] > 25,
    fields: new List<string> { "name" }
);
```

### Example Workflow

Here's a complete example of working with the `JSONDatabaseManager` library:

```csharp
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
        var volumePath = "C:\path\to\your\db\folder\";
        JsonDatabase db = new JsonDatabase(dbName, dbPassword, volumePath);

        // Synchronize database with the disk
        await db.SyncFromDiskAsync();

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
            filter: item => (int)item["age"] > 25,
            fields: new List<string> { "name" }
        );
    }
}
```

## Running Tests

To run the unit tests for the `JSONDatabaseManager` library, use the following command in the root directory of the project:

```bash
dotnet test
```

This will run all unit tests and provide feedback on whether they passed or failed.

## Contributing

Feel free to contribute to this library by forking the repository and submitting pull requests. If you encounter any bugs or have feature requests, please open an issue in the repository.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
