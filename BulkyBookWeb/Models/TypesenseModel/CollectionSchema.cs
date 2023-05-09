using Newtonsoft.Json;

namespace BulkyBookWeb.Models.TypesenseModel
{
    public class CollectionSchema
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fields")]
        public List<FieldSchema> Fields { get; set; }
    }
}
