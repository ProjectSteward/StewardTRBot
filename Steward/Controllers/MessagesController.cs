using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Steward.Ai;
using Microsoft.Bot.Builder.Azure;
using Autofac;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Bot.Builder.History;
using Microsoft.Azure;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.FormFlow;
using Steward.Dialogs;

namespace Steward.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
       
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                
                if (activity.Text == "Complex")
                {
                   // await Conversation.SendAsync(activity, MakeRoot);
                }
                else
                {
                    await Conversation.SendAsync(activity, () => new StewardluisGuide());
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static Activity HandleSystemMessage(IActivity message)
        {
            switch (message.Type)
            {
                case ActivityTypes.DeleteUserData:
                    // Implement user deletion here
                    // If we handle user deletion, return a real message
                    break;
                case ActivityTypes.ConversationUpdate:
                    // Handle conversation state changes, like members being added and removed
                    // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                    // Not available in all channels
                    break;
                case ActivityTypes.ContactRelationUpdate:
                    // Handle add/remove from contact lists
                    // Activity.From + Activity.Action represent what happened
                    break;
                case ActivityTypes.Typing:
                    // Handle knowing tha the user is typing
                    break;
                case ActivityTypes.Ping:
                    break;
            }

            return null;
        }

        private static IForm<TRAddIn> BuildForm()
        {
            var builder = new FormBuilder<TRAddIn>();

            //ActiveDelegate<TRAddIn> isBYO = (pizza) => pizza. == PizzaOptions.BYOPizza;
            //ActiveDelegate<TRAddIn> isSignature = (pizza) => pizza.Kind == PizzaOptions.SignaturePizza;
            //ActiveDelegate<TRAddIn> isGourmet = (pizza) => pizza.Kind == PizzaOptions.GourmetDelitePizza;
            //ActiveDelegate<TRAddIn> isStuffed = (pizza) => pizza.Kind == PizzaOptions.StuffedPizza;

            return builder
                // .Field(nameof(PizzaOrder.Choice))
                .Field(nameof(TRAddIn.mainQ))
                .Field(nameof(TRAddIn.disabled))
                .Field(nameof(TRAddIn.inactive))
                .Field(nameof(TRAddIn.UACPoppup))
                .Field(nameof(TRAddIn.RibbonInComplete))
                .AddRemainingFields()
                .Confirm("Is the TR Showing up at all?")
                .Confirm("check if the TR addIn is disabled?")
                .Confirm("Check if the TR AddIn in Inactive?")
                .Confirm("Is the UAC Poppup happening?")
                .Confirm("Is the Ribbon Complete?")
                .Build()
                ;
        }
        //internal static IDialog<TRAddIn> MakeRoot()
        //{
        //  //  return Chain.From(() => new ComplexluisGuide(BuildForm));
        //}
        //static MessagesController()
        //{
        //    try
        //    {
        //        var builder = new ContainerBuilder();
        //        var store = new TableBotDataStore(CloudStorageAccount.Parse(
        //        CloudConfigurationManager.GetSetting("logtableconnectionstring")));
        //        builder.Register(c => store)
        //            .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
        //            .AsSelf()
        //            .SingleInstance();
        //    }
        //    catch (System.Exception ex)
        //    {

        //        throw;
        //    }

        //    //await logger.LogAsync(activity);
        //}
    }
}