using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Steward.Ai.Salesforce.Model;
using Steward.Ai.Watson.Conversation;
using Steward.Configuration;
using Steward.Dialogs;
using Steward.Helper;
using Steward.Logging;
using Steward.Service;

namespace Steward.Actor
{
    internal class ServiceCloudCaseActor : BotDialogActor, IActor
    {
        private readonly IConversationData conversationData;

        private const string RequestDataName = "requestData";
        private const string ServiceCloudEndPointName = "ServiceCloud.Endpoint";

        private readonly ISettings settings;
        private readonly ILog log;
        
        public ServiceCloudCaseActor(IStewardDialogContext context, IConversationData conversationData, ISettings settings, ILog log) : base(context)
        {
            this.conversationData = conversationData;
            this.settings = settings;
            this.log = log;
        }

        async Task IActor.Execute()
        {
            using (var client = ServiceResolver.Get<IWebClient>())
            {
                dynamic data = conversationData[RequestDataName];

                string caseId = data.caseId;

                await PostAsync("Finding case #" + caseId + " information...");

                var endpoint = GetEndPoint(data);

                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Accept", "application/json");

                try
                {
                    string responseMessage = await client.DownloadStringTaskAsync(endpoint);

                    log.Debug($"responseMessage : {responseMessage}");

                    if (string.IsNullOrWhiteSpace(responseMessage))
                    {
                        await PostAsync($"The case #{caseId} is not available!");

                        return;
                    }

                    dynamic anObject = JsonConvert.DeserializeObject<ServiceCloudCase>(responseMessage);

                    var replyMessage = GetReplyMessage(anObject);

                    await PostAsync(replyMessage);
                }
                catch (WebException webExc)
                {
                    log.Error(webExc.StackTrace);

                    await PostAsync("Sorry! There is something wrong at the moment. Please try again later.");
                }
            }
        }

        private string GetReplyMessage(ServiceCloudCase serviceCloudCase)
        {
            return $"**Case number :**\n\n{serviceCloudCase.CaseNumber}\n\n\n\n" +
                   $"**Subject :**\n\n{serviceCloudCase.Subject}\n\n\n\n" +
                   $"**Description :**\n\n{serviceCloudCase.Description}\n\n\n\n" +
                   $"**Status :**\n\n{serviceCloudCase.Status}\n\n\n\n" +
                   $"**Link :**\n\n{serviceCloudCase.Url}";
        }

        private Uri GetEndPoint(dynamic data)
        {
            return new UriBuilder(settings[ServiceCloudEndPointName] + "/" + data.caseId).Uri;
        }
    }
}