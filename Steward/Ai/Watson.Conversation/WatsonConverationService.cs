using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steward.Helper;

namespace Steward.Ai.Watson.Conversation
{
    [Serializable]
    internal class WatsonConverationService : IWatsonConversationService
    {
        private readonly Uri endpoint;
        private readonly string credential;

        internal WatsonConverationService(string endpoint, string credential)
        {
            this.credential = credential;
            this.endpoint = new UriBuilder(endpoint).Uri;
        }

        async Task<string> IWatsonConversationService.SendMessage(string message, string context)
        {
            using (var client = CreateWebClient())
            {
                if (string.IsNullOrWhiteSpace(context))
                {
                    // Try to send the first conversation
                    var welcomeMessage = JObject.Parse(await UploadStringTaskAsync(client, "{ \"input\" : { \"text\" : \"\" } }"));
                    context = JsonConvert.SerializeObject(welcomeMessage["context"]);
                }

                var responseMessage = await UploadStringTaskAsync(client, "{ \"input\" : { \"text\" : \"" + message +"\" }, \"context\" : "+ context + " }");
                return responseMessage;
            }
        }

        private async Task<string> UploadStringTaskAsync(IWebClient client, string data)
        {
            var headers = client.Headers;

            const string contentType = "Content-Type";
            if(headers[contentType] == null)
            { 
                headers.Add(contentType, "application/json");
            }

            const string authorization = "Authorization";
            if (headers[authorization] == null)
            {
                client.Headers.Add(authorization, $"Basic {credential}");
            }

            return await client.UploadStringTaskAsync(endpoint, data);
        }

        protected virtual IWebClient CreateWebClient()
        {
            return new WebClient();
        }
    }
}