using System.Threading.Tasks;
using Steward.Ai.Watson.Conversation.Model;

namespace Steward.Ai.Watson.Conversation
{
    internal interface IWatsonConversationService
    {
        Task<MessageResponse> SendMessage(string message, dynamic context = null);
    }
}