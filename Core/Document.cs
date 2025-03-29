using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace JSONDatabaseManager.Core
{
    public class Document
    {
        public readonly DocumentConfigurations Config;
        public readonly List<DataItem> Collection;

        public Document(string name, string path)
        {
            Random random = new Random();
            var config = new DocumentConfigurations
            {
                Id = random.Next(),
                Name = name,
                DocumentPath = path
            };
            this.Config = config;
            this.Collection = new List<DataItem>();
        }

        public DataItem Insert(Dictionary<string, object> data)
        {
            var item = new DataItem(data);
            Collection.Add(item);
            return item;
        }

        public bool Update(int id, Dictionary<string, object> updatedData)
        {
            var item = Collection.FirstOrDefault(d => d.Id == id);
            if (item != null)
            {
                item.Data = updatedData;
                return true;
            }

            return false;
        }

        public bool Delete(int id)
        {
            var item = Collection.FirstOrDefault(d => d.Id == id);
            if (item != null)
            {
                Collection.Remove(item);
                return true;
            }

            return false;
        }

        public async Task CommitChangesAsync()
        {
            try
            {
                var serializedItems = Collection.Select(item => item.Serialize()).ToArray();
                string json = $"[{string.Join(",", serializedItems)}]";
                var dataPath = Path.Combine(Config.DocumentPath, "data.json");
                await File.WriteAllTextAsync(dataPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving document: {ex.Message}");
            }
        }

        public IEnumerable<Dictionary<string, object>> Query(Func<DataItem, bool>? filter = null,
            List<string>? fields = null)
        {
            var queryResult = filter == null ? Collection : Collection.Where(filter);

            if (fields != null && fields.Any())
            {
                return queryResult.Select(item =>
                {
                    var filteredData = item.Data.Where(d => fields.Contains(d.Key))
                        .ToDictionary(k => k.Key, v => v.Value);
                    return filteredData;
                });
            }

            return queryResult.Select(item => item.Data);
        }
    }
}