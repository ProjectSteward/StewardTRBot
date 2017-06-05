using Newtonsoft.Json;

namespace Steward.Ai.ThomsonReuters.Model
{
    public class AppOwner
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }
        
    }
}