using System;
using System.Threading.Tasks;
using Steward.Dialogs;
using Steward.Logging;
using Steward.Service;

namespace Steward.Actor
{
    internal class BotDialogActor
    {
        private readonly IStewardDialogContext context;

        internal BotDialogActor(IStewardDialogContext context)
        {
            this.context = context;
        }

        protected Task PostAsync(string message)
        {
            return context.PostAsync(message);
        }

        protected string GetMessage()
        {
            return context.Activity.AsMessageActivity().Text;
        }

        protected ILog GetLog(Type type)
        {
            return ServiceResolver.Get<ILogManager>().GetLogger(type);
        }

        protected T GetObject<T>()
        {
            return ServiceResolver.Get<T>();
        }
    }
}