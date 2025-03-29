using System.Text.Json;
using System.Text.Json.Serialization;

namespace JSONDatabaseManager.Core;

public class DocumentConfigurations
{
    [JsonPropertyName("document_id")] public required int Id { get; set; }
    [JsonPropertyName("document_name")] public required string Name { get; set; }
    [JsonPropertyName("document_path")] public required string DocumentPath { get; set; }

    public string GetDataPath() => Path.Combine(DocumentPath, "data.json");
    public string GetCachePath() => Path.Combine(DocumentPath, "cache.json");

    public string Serialize()
    {
        var serializedConfig =
            JsonSerializer.Serialize(new
            {
                document_id = Id,
                document_name = Name,
                document_path = DocumentPath,
                document_data_path = GetDataPath(),
                document_cache_path = GetCachePath()
            }, new JsonSerializerOptions { WriteIndented = true });

        return serializedConfig;
    }
}