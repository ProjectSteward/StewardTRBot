using System;
using System.Dynamic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Steward.Ai.Watson.Conversation.Model;
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

        async Task<MessageResponse> IWatsonConversationService.SendMessage(string message, dynamic context)
        {
            using (var client = CreateWebClient())
            {
                dynamic inputContext = context;

                dynamic messageObj = new ExpandoObject();

                messageObj.input = new ExpandoObject();
                messageObj.input.text = string.Empty;

                if (inputContext == null)
                {
                    // Try to send the first conversation
                    var welcomeMessage =
                        JsonConvert.DeserializeObject<MessageResponse>(
                            await UploadStringTaskAsync(client, JsonConvert.SerializeObject(messageObj)));
                    inputContext = welcomeMessage.Context;
                }

                messageObj.input.text = message;
                messageObj.context = inputContext;

                var responseMessage = await UploadStringTaskAsync(client, JsonConvert.SerializeObject(messageObj));
                return JsonConvert.DeserializeObject<MessageResponse>(responseMessage);
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