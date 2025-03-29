using System.Text.Json;
using System.Text.Json.Serialization;

namespace JSONDatabaseManager.Core
{
    public class DataBaseConfigurations
    {
        [JsonPropertyName("database_name")] public required string Name { get; set; }

        [JsonPropertyName("database_password")]
        public required string Password { get; set; }

        [JsonPropertyName("database_root_path")]
        public required string DatabaseRootPath { get; set; }

        public string GetDataPath() => Path.Combine(DatabaseRootPath, "data");
        public string GetCachePath() => Path.Combine(DatabaseRootPath, "cache");
        public string GetConfigPath() => Path.Combine(DatabaseRootPath, "meta_data.json");

        public string Serialize()
        {
            var serializedConfig =
                JsonSerializer.Serialize(new
                {
                    database_name = Name,
                    database_root_path = DatabaseRootPath,
                    database_data_path = GetDataPath(),
                    database_cache_path = GetCachePath(),
                    database_password = Password
                }, new JsonSerializerOptions { WriteIndented = true });

            return serializedConfig;
        }
    }
}