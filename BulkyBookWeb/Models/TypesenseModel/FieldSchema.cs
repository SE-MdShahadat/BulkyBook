using Newtonsoft.Json;

namespace BulkyBookWeb.Models.TypesenseModel
{
    public class FieldSchema
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
