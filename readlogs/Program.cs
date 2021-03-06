﻿using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.IO;
using System.Linq;
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
            var acc = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("logtableconnectionstring"));
            var tableClient = acc.CreateCloudTableClient();
            var table = tableClient.GetTableReference("ActivitiesLogger");
            TableContinuationToken token = null;

            var tempFile = Path.GetTempPath() + "Steward.log";

            try
            {
                File.Delete(tempFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Warning - Temp file is not found so we didn't delete");
                Console.WriteLine($"Exception - {ex.Message}");
            }

            var qq = BuildQuery(null, null, new DateTime(2017, 04, 20)); // get conversation since date
            do
            {
                var queryResult = table.ExecuteQuerySegmented(qq, (pKey, rowKey, timestamp, properties, etag) =>
                {
                    var entity = new ActivityEntity();
                    entity.ReadEntity(properties, null);
                    return entity.Activity;
                }, token);

                foreach (var result in queryResult.OrderBy(r => r.Timestamp))
                {
                    var activity = result as Activity;
                    if (activity == null)
                        continue;

                    var replyText = activity.Text;
                    if (string.IsNullOrEmpty(replyText))
                    {
                        // try checking the attachment
                        if (activity.Attachments != null && activity.Attachments.Any())
                        {
                            replyText = activity.Attachments[0].Content.ToString();
                        }
                    }

                    var text = $"{GetSenderText(activity.From)} --> {GetSenderText(activity.Recipient)} @{activity.Timestamp}:\t {replyText}";

                    if (activity.From != null && IsBot(activity.From.Id) == false)
                    {
                        text = "\r\n" + text;
                    }

                    Console.WriteLine(text);

                    try
                    {
                        using (var file = new StreamWriter(tempFile, true))
                        {
                            file.WriteLine(text);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fail to write this text - {text}");
                        Console.WriteLine($"Exception - {ex.Message}");
                    }
                }
                token = queryResult.ContinuationToken;
            } while (token != null);

            Console.ReadLine();
        }

        private static bool IsBot(string idToCheck)
        {
            if (string.IsNullOrEmpty(idToCheck))
                return false;

            if (string.Equals(idToCheck, "StewardTRBot@us4Vaf9M1mA", StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (string.Equals(idToCheck,"56800324", StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;
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
