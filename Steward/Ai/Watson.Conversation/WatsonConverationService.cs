using System;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
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
                messageObj.input.text = message;
                messageObj.context = inputContext;

                var responseMessage = await client.UploadStringTaskAsync(endpoint, JsonConvert.SerializeObject(messageObj));
                return JsonConvert.DeserializeObject<MessageResponse>(responseMessage);
            }
        }

        protected virtual IWebClient CreateWebClient()
        {
            return new WebClient();
        }

        async Task<MessageResponse> IWatsonConversationService.SendMessage(string message, dynamic context, IDialogContext dialogContext)
        {
            using (var client = CreateWebClient())
            {
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Authorization", $"Basic {credential}");

                for (var i = 0; i < client.Headers.Count; i++)
                {
                    await dialogContext.PostAsync($"{client.Headers.GetKey(i)} : {client.Headers.GetValues(i)[0]}");
                }

                dynamic inputContext = context;

                dynamic messageObj = new ExpandoObject();

                messageObj.input = new ExpandoObject();
                messageObj.input.text = message;
                messageObj.context = inputContext;

                var data = JsonConvert.SerializeObject(messageObj);

                await dialogContext.PostAsync($"End point is : {endpoint}");

                await dialogContext.PostAsync($"Post data is : {data}" );

                var responseMessage = await client.UploadStringTaskAsync(endpoint, data);

                await dialogContext.PostAsync($"responseMessage is : {responseMessage}");

                return JsonConvert.DeserializeObject<MessageResponse>(responseMessage);
            }
        }
    }
}