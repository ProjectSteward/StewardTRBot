using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            if (!await IsMessageHandledByWatson(context, message.Text))
            {
                await SearchInQnAMarker(context, message.Text);
                return;
            }

            context.Wait(MessageReceivedAsync);
        }

        protected virtual async Task<bool> IsMessageHandledByWatson(IDialogContext dialogContext, string message)
        {
            try
            {
                string watsonContext;
                if (!dialogContext.PrivateConversationData.TryGetValue(WatsonContextName, out watsonContext))
                {
                    watsonContext = string.Empty;
                }

                var responseMessage = await conversationService.SendMessage(message, watsonContext);

                var responseMessageObject = JObject.Parse(responseMessage);
                var contextObject = responseMessageObject["context"];
                var canBeHandled = contextObject["can_not_be_handled"];

                var handledByWatson = !(canBeHandled != null && Convert.ToBoolean(canBeHandled));

                if(handledByWatson)
                {
                    var toBePersistedContext = JsonConvert.SerializeObject(contextObject);
                    dialogContext.PrivateConversationData.SetValue(WatsonContextName, toBePersistedContext);
                    var listOfResponse = responseMessageObject["output"]["text"];
                    foreach (var text in listOfResponse)
                    {
                        var replyMessage = text.ToString();
                        if (string.IsNullOrWhiteSpace(replyMessage)) continue;
                    await dialogContext.PostAsync(replyMessage);
                    }
                }

                return handledByWatson;
            }
            catch (Exception)
            {
                // We may need some response here to tell the user that we failed to talk with Watson.
                await dialogContext.PostAsync("Sorry! There is something wrong at the moment!");
                await dialogContext.PostAsync("Please try again later!!!!");

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
                    await context.PostAsync(searchResult.Answer);
                    await context.PostAsync("Confidence Level: " + searchResult.Score + "%");
                }
                else
                {
                    await context.PostAsync(Strings_EN.NotFoundInKb);
                    await context.PostAsync("Confidence Level: 0%");
                }
            }
            catch (Exception)
            {
                // TODO: Log Error
                await context.PostAsync(Strings_EN.NotFoundInKb);
                await context.PostAsync("Confidence Level: 0%");
            }
            finally
            {
                await context.PostAsync(Strings_EN.AskForFeedbackMessage);
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}