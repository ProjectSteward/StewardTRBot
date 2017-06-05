using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Steward.Dialogs
{
    internal class StewardDialogContext : IStewardDialogContext
    {
        private readonly IDialogContext context;

        internal StewardDialogContext(IDialogContext context)
        {
            this.context = context;
        }

        IActivity IStewardDialogContext.Activity => context.Activity;

        Task IStewardDialogContext.PostAsync(string message)
        {
            return context.PostAsync(message);
        }
    }
}