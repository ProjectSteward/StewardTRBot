using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using static Microsoft.Bot.Builder.Azure.TableLogger;
using Microsoft.Azure;
using Microsoft.Bot.Connector;
using Microsoft.WindowsAzure.Storage.Table.Protocol;

namespace readlogs
{
    class Program
    {
        static void Main(string[] args)
        {
            var acc = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("logtableconnectionstring"));
            var tableClient = acc.CreateCloudTableClient();
            var table = tableClient.GetTableReference("ActivitiesLogger");
            TableContinuationToken token = null;

            var qq = BuildQuery(null, null, new DateTime(2016, 01, 01)); // get conversation since date
            do
            {
                var queryResult = table.ExecuteQuerySegmented(qq, (pKey, rowKey, timestamp, properties, etag) =>
                {
                    var entity = new ActivityEntity();
                    entity.ReadEntity(properties, null);
                    return entity.Activity;
                }, token);
 
                foreach (var result in queryResult)
                {
                    var activity =  result as Activity;
                    if (activity == null)
                        continue;

                    Console.WriteLine("{0} --> {1} @{2}:\t {3}\r\n\r\n", 
                        GetSenderText(activity.From), 
                        GetSenderText(activity.Recipient), 
                        activity.Timestamp, activity.Text);
                }
                token = queryResult.ContinuationToken;
            } while (token != null);

            Console.ReadLine();
        }

        private static string GetSenderText(ChannelAccount ca)
        {
            return string.IsNullOrEmpty(ca.Name) ? ca.Id : ca.Name;
        }

        // copied from TableLogger.cs in Microsoft.Bot.Builder.Azure
        private static TableQuery BuildQuery(string channelId, string conversationId, DateTime oldest)
        {
            var query = new TableQuery();
            string filter = null;
            if (channelId != null && conversationId != null)
            {
                var pkey = ActivityEntity.GeneratePartitionKey(channelId, conversationId);
                filter = TableQuery.GenerateFilterCondition(TableConstants.PartitionKey, QueryComparisons.Equal, pkey);
            }
            else if (channelId != null)
            {
                var pkey = ActivityEntity.GeneratePartitionKey(channelId, "");
                filter = TableQuery.GenerateFilterCondition(TableConstants.PartitionKey, QueryComparisons.GreaterThanOrEqual, pkey);
            }
            if (oldest != default(DateTime))
            {
                var rowKey = ActivityEntity.GenerateRowKey(oldest);
                var rowFilter = TableQuery.GenerateFilterCondition(TableConstants.RowKey, QueryComparisons.GreaterThanOrEqual, rowKey);
                filter = filter == null ? rowFilter : TableQuery.CombineFilters(filter, TableOperators.And, rowFilter);
            }
            if (filter != null)
            {
                query.Where(filter);
            }
            return query;
        }
        
    }

    
}
