using System;
using System.Threading.Tasks;
using Steward.Ai.Microsoft.QnAMaker;
using Steward.Dialogs;
using Steward.Localization;

namespace Steward.Actor
{
    internal class QnAMakerActor : BotDialogActor, IActor
    {
        internal QnAMakerActor(IStewardDialogContext context) : base(context)
        {
        }

        async Task IActor.Execute()
        {
            var log = GetLog(typeof(QnAMakerActor));

            try
            {
                var message = GetMessage();

                var qnAMakerService = GetObject<IQnAMakerService>();

                // TODO keep one instance or pool, or just make static
                var searchResult = await qnAMakerService.SearchKbAsync(message);

                if (searchResult != null && searchResult.Score > 1.0)
                {
                    await PostAsync(searchResult.Answer);
                    await PostAsync("Confidence Level: " + searchResult.Score + "%");

                    log.Debug($"message : {message}");
                    log.Debug($"answer : {searchResult.Answer}");
                    log.Debug($"Confidence Level: {searchResult.Score}%");

                }
                else
                {
                    await PostAsync(Strings_EN.NotFoundInKb);
                    await PostAsync("Confidence Level: 0%");
                }
            }
            catch (Exception exception)
            {
                log.Error($"exception.Message : {exception.Message}");
                log.Error($"exception.StackTrace : {exception.StackTrace}");

                // TODO: Log Error
                await PostAsync(Strings_EN.NotFoundInKb);
                await PostAsync("Confidence Level: 0%");
            }
            finally
            {
                await PostAsync(Strings_EN.AskForFeedbackMessage);
            }
        }
    }
}