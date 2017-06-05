using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Steward.Ai.Watson.Conversation;
using Steward.Ai.Watson.Conversation.Model;
using Steward.Configuration;
using Steward.Dialogs;
using Steward.Helper;
using Steward.Logging;
using Steward.Manager;
using Steward.Service;
using IServiceProvider = Steward.Service.IServiceProvider;

namespace StewardunitTest.Actor
{
    [TestFixture]
    public class ServiceCloudCaseActorTest
    {
        [Test]
        public async Task TestExecute()
        {
            var responseData = JsonConvert.DeserializeObject<MessageResponse>("{\"context\" : { \"ask_for_case\": true, \"can_not_be_handled\" : true, \"requestData\" : { \"caseId\":\"04541810\"}}}");

            var conversationData = new ConversationData(responseData);

            IStewardManager manager = new StewardManager();

            var provider = new Mock<IServiceProvider>();
            ServiceResolver.ServiceProvider = provider.Object;

            var settings = new Mock<ISettings>();
            var webClient = new Mock<IWebClient>();

            var logManager = new Mock<ILogManager>();
            
            var log = new Mock<ILog>();

            log.Setup(o => o.Debug(It.IsAny<object>())).Callback((object message) => 
            {
                Console.WriteLine(message);
            });

            logManager.Setup(o => o.GetLogger(It.IsAny<Type>())).Returns(log.Object);

            settings.Setup(o => o["ServiceCloud.Endpoint"]).Returns("http://eikonci.int.thomsonreuters.com:9001/api/case");

            provider.Setup(o => o.Get<ISettings>()).Returns(settings.Object);
            provider.Setup(o => o.Get<IWebClient>()).Returns(new WebClient());
            provider.Setup(o => o.Get<ILogManager>()).Returns(logManager.Object);


            var context = new Mock<IStewardDialogContext>();

            context.Setup(o => o.PostAsync(It.IsAny<string>())).Returns((string message) =>
            {
                Console.WriteLine(message);
                return Task.FromResult(true);
            });

            var actor = manager.GetActor(context.Object, conversationData);
            try
            {
                await actor.Execute();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }
    }
}
