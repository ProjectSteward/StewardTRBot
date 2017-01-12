using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Steward.QnaMaker;
using System.Text.RegularExpressions;
using Microsoft.Bot.Connector;

namespace Steward
{

    [LuisModel("0b907fd2-d812-4723-807c-72358c3c3199", "9cc6c294920a4214b0d895c142edc4d8")]
    [Serializable]
    public class StewardluisGuide : LuisDialog<object>
    {
        public const string Entity_location = "Location";

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: "
                 + $"Hi!, I'm Steward, i try to help you to get answers for some of the issues and queries that you have for the help desk, \n you can ask me questions line: \n where to download abc? etc.\n you can always enter **Help** to get more info.";
           // + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

       
        [LuisIntent("Greetings")]
        public async Task GetGreetings(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Count == 0 )
            {
                await context.PostAsync($"Hi thanks for dropping by! \n **how can I help you?**");
            }
            else
            {
                string switch_on = result.Entities[0].Entity;
                if (String.IsNullOrEmpty(switch_on))
                    switch_on = "default";

                switch (switch_on)
                {
                    case "hello":
                        await context.PostAsync($"Hi thanks for checking in :)\n **how can I help you?**!");
                        break;
                    case "good morning":
                        await context.PostAsync($"Very good morning to you!\n **how can I help you?**");
                        break;
                    case "good evening":
                        await context.PostAsync($"Good Evening to you out there :)\n **how can I help you?**!");
                        break;
                    case "good night":
                        await context.PostAsync($"you have a good night!");
                        break;
                    default:
                        await context.PostAsync("hi there!\n **how can I help you?**");
                        break;
                }
            }
            

            context.Wait(MessageReceived);
        }

        [LuisIntent("Appreciation")]
        public async Task AppreciationResonse(IDialogContext context, LuisResult result)
        {
                await context.PostAsync($"Thanks for dropping by. \n\n Remember you can always ask me new queries at any time.");
         
            context.Wait(MessageReceived);
        }

        [LuisIntent("Feedback")]
        public async Task GetFeedback(IDialogContext context, LuisResult result)
        {
            await GetFeedbackgeneric(context);
        }

        public async Task GetFeedbackgeneric(IDialogContext context)
        {
            PromptDialog.Choice(
                      context,
                      AfterRateAsync,
                      new int[5] { 1, 2, 3, 4, 5 },
                       "Thanks for dropping by! \n If you would like to **rate** us, please type **Feedback** \n\n Remember you can always ask me new queries at any time.",
                      promptStyle: PromptStyle.Auto);
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

            var text = $"Thanks for your feedback, have a nice day! \n\n Remember you can always ask me new queries at any time.";
            await context.PostAsync(text);
            context.Wait(MessageReceived);
        }


        //Will run when no intent is triggered
        [LuisIntent("Help")]
        [LuisIntent("None")]
        public async Task NoIntent(IDialogContext context, LuisResult result)
        {
           
            try
            {
                string intent = result.Intents[0].Intent;

                Dictionary<string, string> props = new Dictionary<string, string>();
                props.Add("Question", result.Query);

                //if (await IsAuthenticated(context))
                //{
                    bool sentReply = false;

                    // TODO keep one instance or pool, or just make static
                    var qnaMakerKb = new QnaMakerKb();
                    var searchResult = await qnaMakerKb.SearchKbAsync(result.Query);

                    if (searchResult != null && searchResult.Score > 0)
                    {
                        sentReply = true;

                        // TODO move from AzureSearchHelper
                        string replyContent = StripHTML(searchResult.Answer);

                        await context.PostAsync(searchResult.Answer);

                    }

                    if (!sentReply)
                    {
                        await context.PostAsync("Sorry, I wasn't able to find anything in the Knowledge Base. I've made a note and will try to get this added moving forwards.");
                        props.Add("FoundInKB", "false");
                    }
                    else
                    {
                   // await context.PostAsync("\n\n*Please enter/type \"Accept\" if you have no further question, else please carry on with your questions :)");    
                    PromptDialog.Choice(
                     context,
                     AfterMathAsync,
                     new String[2] { "Comeback later", "New query" },
                      "Here's what we you can do next:",
                     promptStyle: PromptStyle.Auto);
                }

            }
            catch (Exception e)
            {
                //tc.TrackTrace($"NoIntent:: Exception: {e.Message}");
                //await HandleConversationException(context, e);
            }
            finally
            {
               // context.Wait(MessageReceived);
            }
        }

        public async Task AfterMathAsync(IDialogContext context, IAwaitable<string> argument)
        {
            try
            {
                var txtNext = "";
                switch (await argument)
                {
                    case "Comeback later":
                       // await GetFeedbackgeneric(context);
                        txtNext = "Thanks for dropping by! \n If you would like to **rate** us, please type **Feedback** \n\n Remember you can always ask me new queries at any time.";
                        break;
                    case "New query":
                        txtNext = "Please carry on...";
                        break;
                    default:
                        // hours = "11am to 10pm";
                        break;
                }

                var text = txtNext;
                await context.PostAsync(text);
            }
            catch (Exception ex)
            {

                throw;
            }
           

            context.Wait(MessageReceived);
        }

        public static string StripHTML(string text)
        {
            text = replaceHyperlinks(text);
            text = replaceLineBreaks(text);
            text = replaceParagraphs(text);

            text = Regex.Replace(text, @"<[^>]+>|&rsquo;", "").Trim();
            text = Regex.Replace(text, @"&.*?;", "").Trim();

            return text;
        }


        private static string replaceParagraphs(string text)
        {
            text = text.Replace("</p>", Environment.NewLine + Environment.NewLine);
            return text;
        }

        private static string replaceLineBreaks(string text)
        {
            text = text.Replace("<br>", Environment.NewLine);
            return text;
        }

        private static string replaceHyperlinks(string text)
        {
            text = text.Replace("\"", "'");
            try
            {
                // REPLACE HYPERLINKS
                string pat = @"<a(.)+<\/a>";
                Regex r = new Regex(pat, RegexOptions.IgnoreCase);
                Match m = r.Match(text);
                if (m.Success)
                {

                    string value = m.Value;
                    string replacement = replacehyperlink(value);

                    text = text.Replace(value, replacement);

                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;

            }

            return text;
        }

        private static string replacehyperlink(string hyperlinktext)
        {
            string newHrefText = "";
            string label = "";
            string linkTextminusHref = "";


            string pat = @"href='[^']+'";
            Regex r = new Regex(pat, RegexOptions.IgnoreCase);
            Match m = r.Match(hyperlinktext);
            if (m.Success)
            {
                Group g = m.Groups[0];
                CaptureCollection cc = g.Captures;
                Capture c = cc[0];
                string linktext = c.Value;

                linkTextminusHref = linktext.Replace("href='", "").Replace("'", "");
            }

            pat = @"(?<=<a href='[^']+'>)(.*)(?=<\/a\>)";
            r = new Regex(pat, RegexOptions.IgnoreCase);
            m = r.Match(hyperlinktext);
            if (m.Success)
            {
                Group g = m.Groups[0];
                CaptureCollection cc = g.Captures;
                Capture c = cc[0];
                label = c.Value;
            }

            newHrefText = "[" + label + "](" + linkTextminusHref + ")";
            return newHrefText;
        }
    }
}