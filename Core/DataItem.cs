using System.Text.Json;
using System.Text.Json.Serialization;

namespace JSONDatabaseManager.Core;

public class DataItem
{
    [JsonPropertyName("data_item_id")] public int Id { get; set; }

    public Dictionary<string, object> Data { get; set; }

    public DataItem(Dictionary<string, object> data)
    {
        Data = data;
        Id = data.ContainsKey("data_item_id") && data["data_item_id"] is JsonElement jsonElement &&
             jsonElement.TryGetInt32(out int id)
            ? id
            : new Random().Next();
    }

    public string Serialize()
    {
        var dataWithId = new Dictionary<string, object>(Data)
        {
            ["data_item_id"] = Id
        };

        return JsonSerializer.Serialize(dataWithId, new JsonSerializerOptions { WriteIndented = true });
    }
}