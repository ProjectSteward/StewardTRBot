using System;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Steward.Ai;
using Steward.Ai.Microsoft.QnAMaker;
using Steward.Ai.Watson.Conversation;

namespace Steward.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, CreateStewardWatsonGuide);
            }
            else
            {
                await HandleSystemMessage(activity);
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private async Task<Activity> HandleSystemMessage(Activity activity)
        {
            switch (activity.Type)
            {
                case ActivityTypes.DeleteUserData:
                    // Implement user deletion here
                    // If we handle user deletion, return a real message
                    break;
                case ActivityTypes.ConversationUpdate:
                    // Handle conversation state changes, like members being added and removed
                    // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                    // Not available in all channels
                    var userId = activity.From.Id;
                    if (!string.IsNullOrWhiteSpace(userId) && userId.ToLower().Contains("thongppat"))
                    {
                        var client = new ConnectorClient(new Uri(activity.ServiceUrl), new MicrosoftAppCredentials());
                        var reply = activity.CreateReply();
                        reply.Text = "ConversationUpdate evnet received";
                        await client.Conversations.ReplyToActivityAsync(reply);
                    }
                    break;

                case ActivityTypes.EndOfConversation:
                    break;
                case ActivityTypes.ContactRelationUpdate:
                    // Handle add/remove from contact lists
                    // Activity.From + Activity.Action represent what happened
                    var relationUpdateAction = activity.AsContactRelationUpdateActivity();
                    if (relationUpdateAction.Action == ContactRelationUpdateActionTypes.Remove)
                    {
                        using (var scope = BeginLifetimeScope(activity as IMessageActivity))
                        {
                            var botData = scope.Resolve<IBotData>();
                            await botData.LoadAsync(default(CancellationToken));

                            botData.PrivateConversationData.Clear();

                            var stack = scope.Resolve<IDialogStack>();
                            stack.Reset();
                            await botData.FlushAsync(default(CancellationToken));

                            var stateClient = scope.Resolve<IStateClient>();
                            stateClient.BotState.DeleteStateForUser(activity.ChannelId, activity.From.Id);
                        }
                    }
                    break;
                case ActivityTypes.Typing:
                    // Handle knowing tha the user is typing
                    break;
                case ActivityTypes.Ping:
                    break;
            }

            return null;
        }

        internal virtual StewardWatsonGuide CreateStewardWatsonGuide()
        {
            var watsonEndpoint = ConfigurationManager.AppSettings["Watson.Endpoint"];
            var watsonCredential = ConfigurationManager.AppSettings["Watson.Credential"];

            var qnaMakerEndpoint = ConfigurationManager.AppSettings["QnAMaker.Endpoint"];
            var qnaMakerKbId = ConfigurationManager.AppSettings["QnAMaker.KnowledgeBaseId"];
            var qnaMakerSubscriptionKey = ConfigurationManager.AppSettings["QnAMaker.SubscriptionKey"];

            return new StewardWatsonGuide(new WatsonConverationService(watsonEndpoint, watsonCredential)
                , new QnAMakerService(qnaMakerEndpoint, qnaMakerKbId, qnaMakerSubscriptionKey));
        }

        protected virtual ILifetimeScope BeginLifetimeScope(IMessageActivity messageActivity)
        {
            return DialogModule.BeginLifetimeScope(Conversation.Container, messageActivity);
        }
    }
}