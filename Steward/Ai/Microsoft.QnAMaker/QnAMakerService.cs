using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Steward.Helper;

namespace Steward.Ai.Microsoft.QnAMaker
{
    [Serializable]
    internal class QnAMakerService : IQnAMakerService
    {
        private readonly string kbId;
        private readonly string subscriptionKey;
        private readonly string endpoint;

        internal QnAMakerService(string endpoint, string kbId, string subscriptionKey)
        {
            this.kbId = kbId;
            this.subscriptionKey = subscriptionKey;
            this.endpoint = endpoint;
        }

        async Task<QnaMakerResult> IQnAMakerService.SearchKbAsync(string question)
        {
            string responseString;

            //Build the URI
            var uri = new UriBuilder($"{endpoint}/knowledgebases/{kbId}/generateAnswer").Uri;

            var postBody = JsonConvert.SerializeObject(new QnaMakerQuestion
            {
                Question = question
            });

            //Send the POST request
            using (var client = CreateWebClient())
            {
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                responseString = await client.UploadStringTaskAsync(uri, postBody);
            }

            var result = ConvertResponseFromJson(responseString);
            return result;
        }

        private static QnaMakerResult ConvertResponseFromJson(string responseString)
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

        protected virtual IWebClient CreateWebClient()
        {
            return new WebClient();
        }
    }
}