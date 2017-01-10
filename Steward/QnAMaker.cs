using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace Steward.QnaMaker
{
    [Serializable]
    public class QnaMakerKb
    {
        private readonly Uri qnaBaseUri = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v1.0");
        private readonly string KbId = ConfigurationManager.AppSettings["QnaKnowledgeBaseId"];
        private readonly string SubscriptionKey = ConfigurationManager.AppSettings["QnaSubscriptionKey"];

        // Sample HTTP Request:
        // POST /knowledgebases/{KbId}/generateAnswer
        // Host: https://westus.api.cognitive.microsoft.com/qnamaker/v1.0
        // Ocp-Apim-Subscription-Key: {SubscriptionKey}
        // Content-Type: application/json
        // {"question":"hi"}
        public async Task<QnaMakerResult> SearchKbAsync(string question)
        {
            var responseString = string.Empty;

            //Build the URI
            var uri = new UriBuilder($"{qnaBaseUri}/knowledgebases/{this.KbId}/generateAnswer").Uri;

            var postBody = $"{{\"question\": \"{question}\"}}";

            //Send the POST request
            using (var client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Ocp-Apim-Subscription-Key", this.SubscriptionKey);
                responseString = await client.UploadStringTaskAsync(uri, postBody);
            }

            var result = ConvertResponseFromJson(responseString);
            return result;
        }

        private QnaMakerResult ConvertResponseFromJson(string responseString)
        {
            QnaMakerResult response;
            try
            {
                response = JsonConvert.DeserializeObject<QnaMakerResult>(responseString);
            }
            catch
            {
                throw new Exception("Unable to deserialize QnA Maker response string.");
            }

            return response;
        }

        public class QnaMakerResult
        {
            /// <summary>
            /// The top answer found in the QnA Service.
            /// </summary>
            [JsonProperty(PropertyName = "answer")]
            public string Answer { get; set; }

            /// <summary>
            /// The score in range [0, 100] corresponding to the top answer found in the QnA    Service.
            /// </summary>
            [JsonProperty(PropertyName = "score")]
            public double Score { get; set; }
        }

    }
}