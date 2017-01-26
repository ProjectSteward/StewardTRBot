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
            }
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
        public static ILifetimeScope FindContainer()
        {
            var config = GlobalConfiguration.Configuration;
            var resolver = (AutofacWebApiDependencyResolver)config.DependencyResolver;
            return resolver.Container;
        }
    }
}
