using Microsoft.Bot.Builder.Dialogs.Internals;

namespace Steward.Ai.Watson.Conversation
{
    internal interface IWatsonContext
    {
        dynamic Value { get; set; }

        void Remove();
        void Save();
    }
}
