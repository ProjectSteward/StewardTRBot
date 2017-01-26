//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Steward.Dialogs;
//using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Builder.FormFlow;
//using Microsoft.Bot.Builder.Luis;
//using Newtonsoft.Json;
//using Microsoft.Bot.Builder.Luis.Models;

//namespace Steward.Ai
//{
//    [LuisModel("0b907fd2-d812-4723-807c-72358c3c3199", "9cc6c294920a4214b0d895c142edc4d8")]
//    [Serializable]
//    public class ComplexluisGuide : LuisDialog<TRAddIn>
//    {
//        private readonly BuildFormDelegate<TRAddIn> MakeTRAddInSolutionForm;

//        internal ComplexluisGuide(BuildFormDelegate<TRAddIn> makeTRAddInSolutionForm)
//        {
//            this.MakeTRAddInSolutionForm = makeTRAddInSolutionForm;
//        }

//        [LuisIntent("Complex")]
//        public async Task ProcessPizzaForm(IDialogContext context, LuisResult result)
//        //{
//        //    var TRAddInForm = new FormDialog<TRAddIn>(new TRAddIn(), this.MakeTRAddInSolutionForm, FormOptions.PromptInStart);
//        //    context.Call<TRAddIn>(TRAddInForm, TRAddInComplete);
//        }
//        //private async Task TRAddInComplete(IDialogContext context, IAwaitable<TRAddIn> result)
//        //{
//        //    TRAddIn order = null;
//        //    try
//        //    {
//        //        order = await result;
//        //    }
//        //    catch (OperationCanceledException)
//        //    {
//        //        await context.PostAsync("You canceled the form!");
//        //        return;
//        //    }

//        //    if (order != null)
//        //    {
//        //        await context.PostAsync("Your Pizza Order: " + order.ToString());
//        //    }
//        //    else
//        //    {
//        //        await context.PostAsync("Form returned empty response!");
//        //    }

//        //    context.Wait(MessageReceived);
//        //}
//    }
//}
