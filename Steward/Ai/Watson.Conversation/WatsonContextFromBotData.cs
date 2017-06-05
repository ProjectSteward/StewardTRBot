using Microsoft.Bot.Builder.Dialogs.Internals;

namespace Steward.Ai.Watson.Conversation
{
    internal class WatsonContextFromBotData : IWatsonContext
    {
        private static readonly string WatsonContextName = "WATSON.CONTEXT";

        private readonly IBotData botData;
        private dynamic data;

        internal WatsonContextFromBotData(IBotData botData)
        {
            this.botData = botData;
        }

        dynamic IWatsonContext.Value
        {
            get
            {
                dynamic watsonContext;
                if (!botData.PrivateConversationData.TryGetValue(WatsonContextName, out watsonContext))
                {
                    watsonContext = null;
                }

                return watsonContext;
            }

            set
            {
                data = value;
            }
        }

        void IWatsonContext.Remove()
        {
            botData.PrivateConversationData.RemoveValue(WatsonContextName);
        }

        void IWatsonContext.Save()
        {
            botData.PrivateConversationData.SetValue(WatsonContextName, data);
        }
    }
}