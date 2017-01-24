using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace conversationLogRead
{
    class Program
    {
        static void Main(string[] args)
        {
            var tableName = "TableLoggerTestActivities";
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            account.CreateCloudTableClient().GetTableReference(tableName).DeleteIfExists();


            var acc = new CloudStorageAccount(
                         new StorageCredentials("account name", "account key"), false);
            var tableClient = acc.CreateCloudTableClient();
            var table = tableClient.GetTableReference("table name");
            TableContinuationToken token = null;
            var entities = new List<object>();
            do
            {
                var queryResult = table.ExecuteQuerySegmented(new TableQuery<object>(), token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);
        }
    }
}
