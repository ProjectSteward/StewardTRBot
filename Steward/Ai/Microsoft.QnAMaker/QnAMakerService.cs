using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Steward.Configuration;
using Steward.Helper;
using Steward.Logging;
using Steward.Service;

namespace Steward.Ai.Microsoft.QnAMaker
{
    internal class QnAMakerService : IQnAMakerService
    {

        private const string QnAMakerEndPoint = "QnAMaker.Endpoint";
        private const string QnAMakerKnowledgeBaseId = "QnAMaker.KnowledgeBaseId";
        private const string QnAMakerSubscriptionKey = "QnAMaker.SubscriptionKey";

        private readonly ISettings settings;
        private readonly ILog log;
        
        internal QnAMakerService(ISettings settings, ILog log)
        {
            this.settings = settings;
            this.log = log;
        }

        async Task<QnaMakerResult> IQnAMakerService.SearchKbAsync(string question)
        {
            var kbId = settings[QnAMakerKnowledgeBaseId];
            var subscriptionKey = settings[QnAMakerSubscriptionKey];
            var endpoint = settings[QnAMakerEndPoint];

            //Build the URI
            var uri = new UriBuilder($"{endpoint}/knowledgebases/{kbId}/generateAnswer").Uri;

            var postBody = JsonConvert.SerializeObject(new QnaMakerQuestion
            {
                Question = question
            });

            string responseString;

            log.Debug($"postBody : {postBody}");

            //Send the POST request
            using (var client = ServiceResolver.Get<IWebClient>())
            {
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                responseString = await client.UploadStringTaskAsync(uri, postBody);
            }

            log.Debug($"responseString : {responseString}");

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
            catch(Exception exception)
            {
                log.Error($"exception.Message : {exception.Message}");
                log.Error($"exception.StackTrace : {exception.StackTrace}");

                throw new Exception("Unable to deserialize QnA Maker response string.");
            }

            return response;
        }
    }
}