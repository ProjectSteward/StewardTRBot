using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Azure;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Microsoft.WindowsAzure.Storage;
using System.Reflection;
using System.Web.Http;
using log4net.Config;
using Steward.Ai.Microsoft.QnAMaker;
using Steward.Ai.Watson.Conversation;
using Steward.Configuration;
using Steward.Helper;
using Steward.Service;
using Steward.Logging;
using log4net.Appender;
using Steward.Manager;

namespace Steward
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            {
                // http://docs.autofac.org/en/latest/integration/webapi.html#quick-start
                var builder = new ContainerBuilder();

                // register the Bot Builder module
                builder.RegisterModule(new DialogModule());
                // register the alarm dependencies
                builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

                var account = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("logtableconnectionstring"));

                builder.Register(c => new TableBotDataStore(account))
                    .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                    .AsSelf()
                    .SingleInstance();

                builder.RegisterModule(new TableLoggerModule(account, "ActivitiesLogger"));

                // Get your HttpConfiguration.
                var config = GlobalConfiguration.Configuration;

                // Register your Web API controllers.
                builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

                // Set the dependency resolver to be Autofac.
                //var container = builder.Build();
                builder.Update(Conversation.Container);
                config.DependencyResolver = new AutofacWebApiDependencyResolver(Conversation.Container);


                XmlConfigurator.Configure();

                RegisterOtherDependencies();
            }
            
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_End()
        {
            FlushAllLogsInBuffer();
        }

        private void FlushAllLogsInBuffer()
        {
            var rep = log4net.LogManager.GetRepository();
            foreach (var appender in rep.GetAppenders())
            {
                var buffered = appender as BufferingAppenderSkeleton;
                buffered?.Flush();
            }
        }

        private void RegisterOtherDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Settings>().As<ISettings>().SingleInstance();

            builder.RegisterType<LogManager>().As<ILogManager>().SingleInstance();

            builder.Register(ctx => new WebClient()).As<IWebClient>();

            builder.Register(ctx => new WatsonConverationService(ctx.Resolve<ISettings>(), 
                                                                 ctx.Resolve<ILogManager>().GetLogger(typeof(WatsonConverationService))))
                    .As<IWatsonConversationService>()
                    .SingleInstance();


            builder.Register(ctx => new QnAMakerService(ctx.Resolve<ISettings>(),
                                                        ctx.Resolve<ILogManager>().GetLogger(typeof(QnAMakerService))))
                    .As<IQnAMakerService>()
                    .SingleInstance();

            builder.RegisterType<StewardManager>().As<IStewardManager>().SingleInstance();

            ServiceResolver.ServiceProvider = new ServiceProvider(builder.Build());
        }

        public static ILifetimeScope FindContainer()
        {
            var config = GlobalConfiguration.Configuration;
            var resolver = (AutofacWebApiDependencyResolver)config.DependencyResolver;
            return resolver.Container;
        }
    }
}
