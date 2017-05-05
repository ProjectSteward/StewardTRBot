using System;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Steward.Ai.Watson.Conversation.Model;
using Steward.Configuration;
using Steward.Helper;
using Steward.Logging;
using Steward.Service;

namespace Steward.Ai.Watson.Conversation
{
    internal class WatsonConverationService : IWatsonConversationService
    {
        private readonly ISettings settings;
        private readonly ILog log;

        private const string WatsonEndPoint = "Watson.Endpoint";
        private const string WatsonCredential = "Watson.Credential";

        internal WatsonConverationService(ISettings settings, ILog log)
        {
            this.settings = settings;
            this.log = log;
        }

        async Task<MessageResponse> IWatsonConversationService.SendMessage(string message, dynamic context)
        {
            using (var client = ServiceResolver.Get<IWebClient>())
            {
                var credential = settings[WatsonCredential];
                var endpoint = new UriBuilder(settings[WatsonEndPoint]).Uri;

                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Authorization", $"Basic {credential}");

                dynamic inputContext = context;

                dynamic messageObj = new ExpandoObject();

                messageObj.input = new ExpandoObject();
                messageObj.input.text = CorrectInputMessage(message);
                messageObj.context = inputContext;

                var data = JsonConvert.SerializeObject(messageObj);

                log.Debug($"message : {message}");
                log.Debug($"data : {data}");

                var responseMessage = await client.UploadStringTaskAsync(endpoint, data);

                log.Debug($"responseMessage : {responseMessage}");

                return JsonConvert.DeserializeObject<MessageResponse>(responseMessage);
            }
        }

        private static string CorrectInputMessage(string inputMessage)
        {
            var message = new StringBuilder(inputMessage);

            message.Replace("\n", "        ");
            message.Replace("\r", "        ");
            message.Replace("\t", "    ");

            return message.ToString();
        }
    }
}