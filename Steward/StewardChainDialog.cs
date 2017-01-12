using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Steward
{
    [Serializable]
    public class StewardDialog : IDialog<object>
    {
        protected int count = 1;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            try
            {
                var message = await argument;
                
                    PromptDialog.Choice(
                       context,
                       AfterRateAsync,
                       new int[5] { 1, 2, 3, 4, 5 },
                        "#H1 Thanks for spending time today \n\n If you have further questions please carry on\n else \n\n Please rate this service between 1 to 5, 1 being unsatisfactory, 5 being excellent",
                       promptStyle: PromptStyle.Auto);


            }
            catch (Exception ex)
            {

                throw;
            }
            finally {
                context.Wait(MessageReceivedAsync);
            }
        }
        public async Task AfterRateAsync(IDialogContext context, IAwaitable<int> argument)
        {
            //var hours = string.Empty;
            switch (await argument)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    //hours = "5pm to 11pm";
                    break;
                default:
                    // hours = "11am to 10pm";
                    break;
            }

            var text = $"Thanks for your feedback, have a nice day!";
            await context.PostAsync(text);
            context.Wait(MessageReceivedAsync);
        }
    }
}