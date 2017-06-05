using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Steward.Actor;
using Steward.Ai.Watson.Conversation;
using Steward.Ai.Watson.Conversation.Model;
using Steward.Dialogs;

namespace Steward.Manager
{
    internal interface IStewardManager
    {
        IWatsonContext GetWatsonContext(IBotData botData);

        IConversationData GetConversationData(MessageResponse data);

        IActor GetActor(IStewardDialogContext context, IConversationData conversationData);

        IStewardDialogContext CreateStewardDialogContext(IDialogContext context);
    }
}
