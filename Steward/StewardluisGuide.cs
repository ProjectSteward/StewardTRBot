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
                 + $"Hi!, I'm Steward, i try to help you to get answers for some of the issues and queries that you have for the help desk, \\n you can ask me questions line: \\n where to download abc? etc.\\n you can always enter 'Help' to get more info.";
           // + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        //[LuisIntent("Get weather")]
        //public async Task GetWeather(IDialogContext context, LuisResult result)
        //{
        //    var cities = (IEnumerable<City>)Enum.GetValues(typeof(City));
        //    EntityRecommendation location;

        //    if (!result.TryFindEntity(Entity_location, out location))
        //    {
        //        PromptDialog.Choice(context,
        //                            SelectCity,
        //                            cities,
        //                            "In which city do you want to know the weather forecast?");
        //    }
        //    else
        //    {
        //        Rootobject weatherObj = new Rootobject();
        //        //var weatherREST = "http://api.openweathermap.org/data/2.5/forecast/city?q=" + result.Entities[0].Entity + "&APPID=75347511bed7141f8891a058749cd03d";
        //        //using (var client = new HttpClient())
        //        //{
        //        //    client.BaseAddress = new Uri(weatherREST);
        //        //    client.DefaultRequestHeaders.Accept.Clear();
        //        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        //    // New code:
        //        //    var response = await client.GetAsync(weatherREST);
        //        //    response.EnsureSuccessStatusCode();

                    
        //        //}
        //        weatherObj = await getmyweather(result.Entities[0].Entity);
        //        await context.PostAsync($"The weather in " +  weatherObj.city.name + " is " + Math.Floor((weatherObj.list[0].main.temp) - 273.15) + "&#8451; and it's going to be " + weatherObj.list[0].weather[0].description);
        //        context.Wait(MessageReceived);
        //    }
        //}
        [LuisIntent("Greetings")]
        public async Task GetGreetings(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Count == 0 )
            {
                await context.PostAsync($"Hi thanks for checking in!");
            }
            else
            {
                string switch_on = result.Entities[0].Entity;
                if (String.IsNullOrEmpty(switch_on))
                    switch_on = "default";

                switch (switch_on)
                {
                    case "hello":
                        await context.PostAsync($"Hi thanks for checking in :)!");
                        break;
                    case "good morning":
                        await context.PostAsync($"Very good morning to you!");
                        break;
                    case "good evening":
                        await context.PostAsync($"Good Evening to you out there :)!");
                        break;
                    case "good night":
                        await context.PostAsync($"you have a good night!");
                        break;
                    default:
                        await context.PostAsync("hi there!");
                        break;
                }
            }
            

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
                        await context.PostAsync("\n\n*Powered by the new QnA Maker Cognitive Service*");
                        props.Add("FoundInKB", "true");
                    }

                    //tc.TrackEvent(intent, props);
                //}
                //else
                //{
                //    //IMessageActivity responseMessage = await GenerateAuthenticationResponse(result.Query);
                //    await context.PostAsync(responseMessage);
                //}
            }
            catch (Exception e)
            {
                //tc.TrackTrace($"NoIntent:: Exception: {e.Message}");
                //await HandleConversationException(context, e);
            }
            finally
            {
                context.Wait(MessageReceived);
            }
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