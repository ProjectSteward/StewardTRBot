using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace Steward.Dialogs
{
    internal interface IStewardDialogContext
    {
        IActivity Activity { get; }

        Task PostAsync(string message);
    }
}
