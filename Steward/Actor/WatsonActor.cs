using System.Threading.Tasks;
using Steward.Ai.Watson.Conversation;
using Steward.Dialogs;

namespace Steward.Actor
{
    internal class WatsonActor : BotDialogActor, IActor
    {
        private readonly IConversationData conversationData;

        internal WatsonActor(IStewardDialogContext context, IConversationData conversationData) : base(context)
        {
            this.conversationData = conversationData;
        }

        async Task IActor.Execute()
        {
            var listOfResponse = conversationData.OutputText;

            foreach (var text in listOfResponse)
            {
                var replyMessage = text.ToString();

                if (string.IsNullOrWhiteSpace(replyMessage)) continue;

                await PostAsync(replyMessage);
            }
        }

    }
}