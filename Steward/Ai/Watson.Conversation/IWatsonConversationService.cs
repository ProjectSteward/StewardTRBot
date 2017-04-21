using System.Threading.Tasks;

namespace Steward.Ai.Watson.Conversation
{
    internal interface IWatsonConversationService
    {
        Task<string> SendMessage(string message, string context = "");
    }
}