using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Steward.Actor;
using Steward.Ai.Watson.Conversation;
using Steward.Ai.Watson.Conversation.Model;
using Steward.Configuration;
using Steward.Dialogs;
using Steward.Logging;
using Steward.Service;

namespace Steward.Manager
{
    internal class StewardManager : IStewardManager
    {
        IStewardDialogContext IStewardManager.CreateStewardDialogContext(IDialogContext context)
        {
            return new StewardDialogContext(context);
        }

        IActor IStewardManager.GetActor(IStewardDialogContext context, IConversationData conversationData)
        {
            if (conversationData.IsHandled())
            {
                return new WatsonActor(context, conversationData);
            }

            if (conversationData.IsAskingForOwner())
            {
                return new OwnerServiceActor(context
                                            , conversationData
                                            , ServiceResolver.Get<ISettings>()
                                            , ServiceResolver.Get<ILogManager>().GetLogger(typeof(OwnerServiceActor)));
            }

            if (conversationData.IsAskingForServiceCloudCase())
            {
                return   new ServiceCloudCaseActor(context
                                                    , conversationData
                                                    , ServiceResolver.Get<ISettings>()
                                                    , ServiceResolver.Get<ILogManager>().GetLogger(typeof(ServiceCloudCaseActor)));
            }

            return new QnAMakerActor(context);
        }

        IConversationData IStewardManager.GetConversationData(MessageResponse data)
        {
            return new ConversationData(data);
        }

        IWatsonContext IStewardManager.GetWatsonContext(IBotData botData)
        {
            return new WatsonContextFromBotData(botData);
        }


    }
}
