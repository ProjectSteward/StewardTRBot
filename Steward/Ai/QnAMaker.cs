using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Steward.Ai
{
    [Serializable]
    public class QnaMakerKb
    {
        private readonly Uri _qnaBaseUri = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v1.0");
        private readonly string _kbId = ConfigurationManager.AppSettings["QnaKnowledgeBaseId"];
        private readonly string _subscriptionKey = ConfigurationManager.AppSettings["QnaSubscriptionKey"];

        // Sample HTTP Request:
        // POST /knowledgebases/{KbId}/generateAnswer
        // Host: https://westus.api.cognitive.microsoft.com/qnamaker/v1.0
        // Ocp-Apim-Subscription-Key: {SubscriptionKey}
        // Content-Type: application/json
        // {"question":"hi"}
        public async Task<QnaMakerResult> SearchKbAsync(string question)
        {
            string responseString;

            //Build the URI
            var uri = new UriBuilder($"{_qnaBaseUri}/knowledgebases/{_kbId}/generateAnswer").Uri;

            var postBody = JsonConvert.SerializeObject(new QnaMakerQuestion
            {
                Question = question
            });

            //Send the POST request
            using (var client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
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

        public class QnaMakerQuestion
        {
            /// <summary>
            /// The top answer found in the QnA Service.
            /// </summary>
            [JsonProperty(PropertyName = "question")]
            public string Question { get; set; }
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