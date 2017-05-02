using System;
using System.Dynamic;
using System.Text;
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
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Authorization", $"Basic {credential}");

                dynamic inputContext = context;

                dynamic messageObj = new ExpandoObject();

                messageObj.input = new ExpandoObject();
                messageObj.input.text = CorrectInputMessage(message);
                messageObj.context = inputContext;

                var data = JsonConvert.SerializeObject(messageObj);

                var responseMessage = await client.UploadStringTaskAsync(endpoint, data);

                return JsonConvert.DeserializeObject<MessageResponse>(responseMessage);
            }
        }

        protected virtual IWebClient CreateWebClient()
        {
            return new WebClient();
        }

        private string CorrectInputMessage(string inputMessage)
        {
            var message = new StringBuilder(inputMessage);

            message.Replace("\n", "        ");
            message.Replace("\r", "        ");
            message.Replace("\t", "    ");

            return message.ToString();
        }
    }
}