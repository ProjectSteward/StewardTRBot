using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Steward.Ai.Salesforce.Model;
using Steward.Ai.ThomsonReuters.Model;
using Steward.Ai.Watson.Conversation;
using Steward.Configuration;
using Steward.Dialogs;
using Steward.Helper;
using Steward.Logging;
using Steward.Service;

namespace Steward.Actor
{
    internal class OwnerServiceActor : BotDialogActor, IActor
    {
        private readonly IConversationData conversationData;
        private const string RequestDataName = "requestData";
        private const string AppOwnerServiceEndPointName = "AppOwner.Endpoint";

        private readonly ISettings settings;
        private readonly ILog log;

        internal OwnerServiceActor(IStewardDialogContext context, IConversationData conversationData, ISettings settings, ILog log) : base(context)
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

                string appName = data.appName;

                await PostAsync("Finding owners of " + appName);

                var endpoint = GetEndPoint(data);

                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Accept", "application/json");

                try
                {
                    string responseMessage = await client.DownloadStringTaskAsync(endpoint);

                    log.Debug($"responseMessage : {responseMessage}");

                    if (string.IsNullOrWhiteSpace(responseMessage))
                    {
                        await PostAsync($"{appName} is not available!");

                        return;
                    }

                    var owners = JsonConvert.DeserializeObject<AppOwner[]>(responseMessage);

                    await PostAsync($"Owners for {appName}");

                    foreach (var owner in owners)
                    {
                        var replyMessage = string.Empty;
                        replyMessage += $"**Name : **\n\n{owner.Name}\n\n\n\n";
                        replyMessage += $"** Email :**\n\n{owner.Email}\n\n\n\n";
                        await PostAsync(replyMessage);
                    }

                }
                catch (WebException webExc)
                {
                    log.Error(webExc.StackTrace);

                    await PostAsync("Sorry! There is something wrong at the moment. Please try again later.");
                }
            }
        }

        private Uri GetEndPoint(dynamic data)
        {
            return new UriBuilder(settings[AppOwnerServiceEndPointName] + "/" + data.appName).Uri;
        }
    }
}