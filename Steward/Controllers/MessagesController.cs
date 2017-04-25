using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
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
                var watsonEndpoint = ConfigurationManager.AppSettings["Watson.Endpoint"];
                var watsonCredential = ConfigurationManager.AppSettings["Watson.Credential"];

                var qnaMakerEndpoint = ConfigurationManager.AppSettings["QnAMaker.Endpoint"];
                var qnaMakerKbId = ConfigurationManager.AppSettings["QnAMaker.KnowledgeBaseId"];
                var qnaMakerSubscriptionKey = ConfigurationManager.AppSettings["QnAMaker.SubscriptionKey"];

                await Conversation.SendAsync(activity, () => new StewardWatsonGuide(  new WatsonConverationService(watsonEndpoint, watsonCredential)
                                                                                    , new QnAMakerService(qnaMakerEndpoint, qnaMakerKbId, qnaMakerSubscriptionKey)));

            }
            else
            {
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static Activity HandleSystemMessage(IActivity message)
        {
            switch (message.Type)
            {
                case ActivityTypes.DeleteUserData:
                    // Implement user deletion here
                    // If we handle user deletion, return a real message
                    break;
                case ActivityTypes.ConversationUpdate:
                    // Handle conversation state changes, like members being added and removed
                    // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                    // Not available in all channels
                    break;
                case ActivityTypes.ContactRelationUpdate:
                    // Handle add/remove from contact lists
                    // Activity.From + Activity.Action represent what happened
                    break;
                case ActivityTypes.Typing:
                    // Handle knowing tha the user is typing
                    break;
                case ActivityTypes.Ping:
                    break;
            }

            return null;
        }
    }
}