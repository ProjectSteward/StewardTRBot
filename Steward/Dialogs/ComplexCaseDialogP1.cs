using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Steward.Localization;
using Steward.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System.Text.RegularExpressions;

namespace Steward.Dialogs
{
    [Serializable]
    public class ComplexCaseDialogP1
    {
        public static async Task StartAsync(IDialogContext context)
        {
               PromptDialog.Confirm(context, AfterConfirming_TRaddinAppear, Strings_EN.TRTabAppear, promptStyle: PromptStyle.Auto);
        }
        public static async Task AfterConfirming_TRaddinAppear(IDialogContext context, IAwaitable<bool> confirmation)
        {
            if (await confirmation)
            {
                PromptDialog.Confirm(context, AfterConfirming_TRaddinIncomplete, Strings_EN.TRMissingMenu, promptStyle: PromptStyle.Auto);
            }
            else
            {
                PromptDialog.Confirm(context, AfterConfirming_TRaddinDisabledDeactivated, Strings_EN.TRAddInDisabledInactive, promptStyle: PromptStyle.Auto);
            }
        }
        public static async Task AfterConfirming_TRaddinIncomplete(IDialogContext context, IAwaitable<bool> confirmation)
        {
            if (await confirmation)
            {
                PromptDialog.Confirm(context, AfterConfirming_OfficeRepair, Strings_EN.TRRepair, promptStyle: PromptStyle.Auto);
            }
            else
            {
                await context.PostAsync(Strings_EN.ThanksMessage);
            }
        }
        public static async Task AfterConfirming_OfficeRepair(IDialogContext context, IAwaitable<bool> confirmation)
        {
            if (await confirmation)
            {
                await context.PostAsync(Strings_EN.ThanksMessage);

            }
            else
            {
                //await context.PostAsync($"Let me think what to do with you next!");
                PromptDialog.Confirm(context, AfterConfirming_Nothingworks, Strings_EN.TRReInstall, promptStyle: PromptStyle.Auto);
            }
        }

        public static async Task AfterConfirming_TRaddinDisabledDeactivated(IDialogContext context, IAwaitable<bool> confirmation)
        {
            if (await confirmation)
            {
                PromptDialog.Confirm(context, AfterConfirming_AddInConfigProb, Strings_EN.TRAdinEnable, promptStyle: PromptStyle.Auto);
            }
            else
            {
                PromptDialog.Confirm(context, AfterConfirming_AddInConfigProb, Strings_EN.TRAdinConfigureManual, promptStyle: PromptStyle.Auto);
            }

        }
        public static async Task AfterConfirming_AddInConfigProb(IDialogContext context, IAwaitable<bool> confirmation)
        {
            if (await confirmation)
            {

                await context.PostAsync(Strings_EN.ThanksMessage);
                //context.Wait(MessageReceived);
            }
            else
            {
                PromptDialog.Confirm(context, AfterConfirming_Nothingworks, Strings_EN.TRAddinUAC, promptStyle: PromptStyle.Auto);
            }

        }
        public static async Task AfterConfirming_Nothingworks(IDialogContext context, IAwaitable<bool> confirmation)
        {
            if (await confirmation)
            {

                await context.PostAsync(Strings_EN.ThanksMessage);
            }
            else
            {
                await context.PostAsync(Strings_EN.TRTechSupportMandatory);
                await context.PostAsync(Strings_EN.AskForFeedbackMessage);
            }
            
        }


    }
   
 }
