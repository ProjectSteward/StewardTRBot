using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Steward.Ai.Watson.Conversation.Model;

namespace Steward.Ai.Watson.Conversation
{
    internal interface IWatsonConversationService
    {
        Task<MessageResponse> SendMessage(string message, dynamic context = null);
    }
}