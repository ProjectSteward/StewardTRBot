using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Steward.Ai.Watson.Conversation;
using Steward.Localization;
using Steward.Logging;
using Steward.Manager;
using Steward.Service;

namespace Steward.Dialogs
{
    [Serializable]
    internal class StewardWatsonDialog : IDialog<object>
    {

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        internal async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            await ProcessMessage(context, (await argument).Text);

            context.Wait(MessageReceivedAsync);
        }

        private async Task ProcessMessage(IDialogContext context, string message)
        {
            if (await IsMessageHandledByFeedback(context, message))
            {
                return;
            }

            await SendMessageToWatson(context, message);
        }

        protected virtual async Task<bool> IsMessageHandledByFeedback(IDialogContext context, string message)
        {
            if (string.IsNullOrEmpty(message) || !message.StartsWith("feedback", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            await PostAsync(context, Strings_EN.AfterRating);
            return true;
        }

        protected virtual async Task SendMessageToWatson(IDialogContext dialogContext, string message)
        {
            var log = GetLog();

            try
            {
                var manager = GetObject<IStewardManager>();

                var watsonContext = manager.GetWatsonContext(dialogContext);

                IConversationData conversationData = manager.GetConversationData(await GetObject<IWatsonConversationService>().SendMessage(message, watsonContext.Value));

                if (conversationData.IsEnded())
                {
                    watsonContext.Remove();
                }
                else
                {
                    watsonContext.Value = conversationData.Context;
                    watsonContext.Save();
                }

                await manager.GetActor(manager.CreateStewardDialogContext(dialogContext), conversationData).Execute();
            }
            catch (Exception exception)
            {
                log.Error($"exception.Message : {exception.Message}");
                log.Error($"exception.StackTrace : {exception.StackTrace}");
            }
        }

        protected virtual async Task PostAsync(IDialogContext context, string message)
        {
            await context.PostAsync(message);
        }

        protected virtual ILog GetLog()
        {
            return ServiceResolver.Get<ILogManager>().GetLogger(typeof(StewardWatsonDialog));
        }

        protected virtual T GetObject<T>()
        {
            return ServiceResolver.Get<T>();
        }
    }
}