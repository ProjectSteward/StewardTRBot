using Autofac;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.History;
using Microsoft.Bot.Connector;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.Bot.Builder.Azure.TableLogger;
using Microsoft.Azure;

namespace readlogs
{
    class Program
    {
        static void Main(string[] args)
        {
           
                         
            CloudStorageAccount acc = CloudStorageAccount.Parse(
     CloudConfigurationManager.GetSetting("logtableconnectionstring"));
            var tableClient = acc.CreateCloudTableClient();
            var table = tableClient.GetTableReference("BotData");
            TableContinuationToken token = null;


            var entities = new List<ActivityEntity>();
            do
            {
                var queryResult = table.ExecuteQuerySegmented(new TableQuery<ActivityEntity>(), token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
                foreach (var activity in entities)
                {
                   // activity.ReadEntity(activity.WriteEntity(null), null);
                }               
            } while (token != null);

        }
    }
}
