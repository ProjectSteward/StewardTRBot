using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Steward.Ai.Microsoft.QnAMaker;
using Steward.Ai.Watson.Conversation;
using Steward.Localization;

namespace Steward.Ai
{
    [Serializable]
    internal class StewardWatsonGuide : IDialog<object>
    {
        private readonly IWatsonConversationService conversationService;
        private readonly IQnAMakerService qnAMakerService;
        private const string WatsonContextName = "WATSON.CONTEXT";

        internal StewardWatsonGuide(IWatsonConversationService conversationService, IQnAMakerService qnAMakerService)
        {
            this.conversationService = conversationService;
            this.qnAMakerService = qnAMakerService;
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        internal async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (IsAFeedback(message.Text))
            {
                await PostAsync(context, Strings_EN.AfterRating);
                context.Wait(MessageReceivedAsync);
                return;
            }

            if (!await IsMessageHandledByWatson(context, message.Text))
            {
                await SearchInQnAMarker(context, message.Text);
                return;
            }

            context.Wait(MessageReceivedAsync);
        }


        protected virtual bool IsAFeedback(string message)
        {
            return !string.IsNullOrEmpty(message) && message.StartsWith("feedback", StringComparison.OrdinalIgnoreCase);
        }

        protected virtual async Task<bool> IsMessageHandledByWatson(IDialogContext dialogContext, string message)
        {
            try
            {
                dynamic watsonContext;
                if (!dialogContext.PrivateConversationData.TryGetValue(WatsonContextName, out watsonContext))
                {
                    watsonContext = null;
                }

                var responseMessage = await conversationService.SendMessage(message, watsonContext);
                var contextObject = responseMessage.Context;
                var canBeHandled = responseMessage.Context.can_not_be_handled;

                var handledByWatson = !(canBeHandled != null && Convert.ToBoolean(canBeHandled));

                if (!handledByWatson) return handledByWatson;

                dialogContext.PrivateConversationData.SetValue(WatsonContextName, contextObject);

                var listOfResponse = responseMessage.Output.text;
                foreach (var text in listOfResponse)
                {
                    var replyMessage = text.ToString();
                    if (string.IsNullOrWhiteSpace(replyMessage)) continue;

                    await PostAsync(dialogContext, replyMessage);
                }

                return handledByWatson;
            }
            catch (Exception exception)
            {
                // We may need some response here to tell the user that we failed to talk with Watson.
                var userId = dialogContext.Activity.From.Id;
                if (!string.IsNullOrWhiteSpace(userId) && userId.ToLower().Contains("thongpipat"))
                {
                    dynamic watsonContext;
                    if (!dialogContext.PrivateConversationData.TryGetValue(WatsonContextName, out watsonContext))
                    {
                        watsonContext = null;
                    }

                    if (watsonContext != null)
                    {
                        await PostAsync(dialogContext, "The current context value is " + watsonContext.ToString());
                    }

                    await PostAsync(dialogContext, exception.Message);
                    await PostAsync(dialogContext, exception.StackTrace);
                }

                await PostAsync(dialogContext, "Sorry! could you please try again.");

                return true;
            }
        }

        protected virtual async Task SearchInQnAMarker(IDialogContext context, string message)
        {
            try
            {
                // TODO keep one instance or pool, or just make static
                var searchResult = await qnAMakerService.SearchKbAsync(message);

                if (searchResult != null && searchResult.Score > 1.0)
                {
                    await PostAsync(context, searchResult.Answer);
                    await PostAsync(context, "Confidence Level: " + searchResult.Score + "%");
                }
                else
                {
                    await PostAsync(context, Strings_EN.NotFoundInKb);
                    await PostAsync(context, "Confidence Level: 0%");
                }
            }
            catch (Exception)
            {
                // TODO: Log Error
                await PostAsync(context, Strings_EN.NotFoundInKb);
                await PostAsync(context, "Confidence Level: 0%");
            }
            finally
            {
                await PostAsync(context, Strings_EN.AskForFeedbackMessage);

                context.Wait(MessageReceivedAsync);
            }
        }

        protected virtual async Task PostAsync(IDialogContext context, string message)
        {
            await context.PostAsync(message);
        }
    }
}