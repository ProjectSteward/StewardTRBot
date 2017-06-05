using Newtonsoft.Json;

namespace Steward.Ai.Salesforce.Model
{
    public class ServiceCloudCase
    {
        [JsonProperty("caseNumber")]
        public string CaseNumber { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdDate")]
        public string CreateDate { get; set; }

        [JsonProperty("lastModifiedDate")]
        public string LastModifiedDate { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}