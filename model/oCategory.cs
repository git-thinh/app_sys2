using Newtonsoft.Json;

namespace app_sys
{
    public class oCategory
    {
        [JsonProperty("parent")]
        public string Parent { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; } = 0;

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public string Whatever { get; set; }
    }
}
