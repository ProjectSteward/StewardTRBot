using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Steward.Localization;

namespace Steward.Ai
{
    [LuisModel("0b907fd2-d812-4723-807c-72358c3c3199", "9cc6c294920a4214b0d895c142edc4d8")]
    [Serializable]
    public class StewardluisGuide : LuisDialog<object>
    {
        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = await item;
            var handledFeedback = false;
            if (!string.IsNullOrEmpty(message.Text) && message.Text.StartsWith("feedback", StringComparison.OrdinalIgnoreCase))
                handledFeedback = HandleFeedbackWorkflow(context, message.Text);
            
            if (!handledFeedback)
                await base.MessageReceived(context, item);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(Strings_EN.NoneIntent);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greetings")]
        public async Task GetGreetings(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Count == 0)
            {
                await context.PostAsync(Strings_EN.GreetingMessage);
            }
            else
            {
                var switchOn = result.Entities[0].Entity;
                if (string.IsNullOrEmpty(switchOn))
                    switchOn = "default";

                switchOn = switchOn.ToLower();

                switch (switchOn)
                {
                    case "good morning":
                        await context.PostAsync(Strings_EN.GoodMorningMessage);
                        break;
                    case "good evening":
                        await context.PostAsync(Strings_EN.GoodEveningMessage);
                        break;
                    case "good night":
                        await context.PostAsync(Strings_EN.GoodNightMessage);
                        break;
                    default:
                        await context.PostAsync(Strings_EN.GreetingMessage);
                        break;
                }
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("Appreciation")]
        public async Task AppreciationResonse(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(Strings_EN.ThanksMessage);
            context.Wait(MessageReceived);
        }

        //Will run when no intent is triggered
        [LuisIntent("Help")]
        public async Task AskHelpResponse(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(Strings_EN.HelpMessage);
            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        public async Task NoIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                // TODO: Add below to tracing system
                //var intent = result.Intents[0].Intent;
                //var props = new Dictionary<string, string> { { "Question", result.Query } };

                var sentReply = false;

                // TODO keep one instance or pool, or just make static
                var qnaMakerKb = new QnaMakerKb();
                var searchResult = await qnaMakerKb.SearchKbAsync(result.Query);

                // Handle when it's more than 50%
                if (searchResult != null && searchResult.Score > 40)
                {
                    sentReply = true;

                    // TODO move from AzureSearchHelper
                    //var replyContent = MessageHelper.StripHtml(searchResult.Answer);

                    await context.PostAsync(searchResult.Answer);
                }

                if (!sentReply)
                {
                    await context.PostAsync(Strings_EN.NotFoundInKb);

                    // TODO Add to tracing system
                    // props.Add("FoundInKB", "false");
                }

                await context.PostAsync(Strings_EN.ThanksMessage);
                context.Wait(MessageReceived);
            }
            catch (Exception)
            {
                // TODO: Log Error
            }
        }

        private bool HandleFeedbackWorkflow(IDialogContext context, string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;

            try
            {
                // TODO: Record Feedback Text
                //var feedbackText = message;

                // Prompt rating
                PromptDialog.Choice(
                    context,
                    AfterRateAsync,
                    new[] { 1, 2, 3, 4, 5 },
                    Strings_EN.PromptRating);
                return true;
            }
            catch (Exception)
            {
                // TODO: Log the error
                return false;
            }
        }

        private async Task AfterRateAsync(IDialogContext context, IAwaitable<int> result)
        {
            //TODO: Record Rating
            //var rating = await result;

            await context.PostAsync(Strings_EN.AfterRating);
            context.Wait(MessageReceived);
        }
    }
}