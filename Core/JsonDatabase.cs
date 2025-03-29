using System.Text.Json;

namespace JSONDatabaseManager.Core;

public class JsonDatabase
{
    public readonly DataBaseConfigurations Config;
    public readonly List<Document> Documents;

    public JsonDatabase(string name, string password, string volumePath)
    {
        var config = new DataBaseConfigurations
        {
            Name = name,
            Password = password,
            DatabaseRootPath = Path.Combine(volumePath, name),
        };

        this.Config = config;
        this.Documents = [];
    }

    public async Task InitDiskAsync()
    {
        var dataPath = this.Config.GetDataPath();
        var cachePath = this.Config.GetCachePath();
        var configPath = this.Config.GetConfigPath();


        if (!Directory.Exists(this.Config.DatabaseRootPath))
            Directory.CreateDirectory(this.Config.DatabaseRootPath);

        if (!Directory.Exists(dataPath))
            Directory.CreateDirectory(dataPath);

        if (!Directory.Exists(cachePath))
            Directory.CreateDirectory(cachePath);

        await File.WriteAllTextAsync(configPath, this.Config.Serialize());
    }

    public async Task SyncFromDiskAsync()
    {
        try
        {
            var configPath = this.Config.GetConfigPath();

            if (!File.Exists(configPath))
            {
                throw new Exception("Database configuration file not found.");
            }

            var configJson = await File.ReadAllTextAsync(configPath);
            var loadedConfig = JsonSerializer.Deserialize<DataBaseConfigurations>(configJson);

            if (loadedConfig == null)
            {
                throw new Exception("Error parsing database configuration file.");
            }

            if (loadedConfig.Password != this.Config.Password)
            {
                throw new Exception("Invalid password.");
            }

            this.Config.Name = loadedConfig.Name;
            this.Config.Password = loadedConfig.Password;
            this.Config.DatabaseRootPath = loadedConfig.DatabaseRootPath;

            var dataPath = this.Config.GetDataPath();
            if (!Directory.Exists(dataPath))
            {
                throw new Exception("Data folder not found.");
            }

            var dirTasks = Directory.GetDirectories(dataPath).Select(async dir =>
            {
                try
                {
                    var document = await LoadDocumentFromDiskAsync(dir);

                    if (document is null)
                    {
                        Console.WriteLine($"Skipping directory '{dir}' due to invalid document configuration.");
                        return;
                    }

                    this.Documents.Add(document);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing directory '{dir}': {ex.Message}");
                }
            }).ToArray();

            await Task.WhenAll(dirTasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during sync from disk: {ex.Message}");
        }
    }

    private async Task<Document?> LoadDocumentFromDiskAsync(string dir)
    {
        var configFilePath = Path.Combine(dir, "meta_data.json");
        var dataFilePath = Path.Combine(dir, "data.json");

        if (!File.Exists(configFilePath))
            return null;

        var docConfigJson = await File.ReadAllTextAsync(configFilePath);
        var docConfig = JsonSerializer.Deserialize<DocumentConfigurations>(docConfigJson);

        if (docConfig == null)
        {
            return null;
        }

        var document = new Document(docConfig.Name, docConfig.DocumentPath);

        if (File.Exists(dataFilePath))
        {
            try
            {
                var dataJson = await File.ReadAllTextAsync(dataFilePath);
                var items = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(dataJson);
                List<DataItem> dataItems = [];
                if (items != null)
                {
                    dataItems = items.Select(item => { return new DataItem(item); }).ToList();
                }

                document.Collection.AddRange(dataItems);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading data file for document '{docConfig.Name}': {ex.Message}");
            }
        }
        
        return document;
    }


    public async Task<Document> CreateDocument(string name)
    {
        string documentPath = Path.Combine(this.Config.DatabaseRootPath, "data", name);

        var doc = new Document(name, documentPath);

        if (!Directory.Exists(documentPath))
            Directory.CreateDirectory(documentPath);

        string configFilePath = Path.Combine(documentPath, "meta_data.json");
        await File.WriteAllTextAsync(configFilePath, doc.Config.Serialize());

        string dataFilePath = Path.Combine(documentPath, "data.json");
        await File.WriteAllTextAsync(dataFilePath, "[]");

        this.Documents.Add(doc);

        return doc;
    }

    public Document? GetDocument(string name)
    {
        return Documents.FirstOrDefault(d => d.Config.Name == name);
    }
}