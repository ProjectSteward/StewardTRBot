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
using System.Text.RegularExpressions;

using System.Data.Entity;
//using System.Data.Entity.Migrations;
using System.Linq;

using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Diagnostics;
using System.Text;

namespace readlogs
{
    class Program
    {
        static void Main(string[] args)
        {
            List<qna> listCAEs = File.ReadAllLines("C:\\Users\\dighosh\\Documents\\qna1.tsv") // reads all lines into a string array
                 .Skip(1) // skip header line
                 .Select(f => qna.FromTsv(f)) // uses Linq to select each line and create a new Cae instance using the FromTsv method.
                 .ToList();


            CloudStorageAccount acc = CloudStorageAccount.Parse(
     CloudConfigurationManager.GetSetting("logtableconnectionstring"));
            var tableClient = acc.CreateCloudTableClient();
            var table = tableClient.GetTableReference("StewardHistory");
            TableContinuationToken token = null;

            //TableBatchOperation batchOperation = new TableBatchOperation();

            //var entities = new List<ActivityEntity>();
            //do
            //{
            //    var queryResult = table.ExecuteQuerySegmented(new TableQuery<ActivityEntity>(), token);
            //    entities.AddRange(queryResult.Results);
            //    token = queryResult.ContinuationToken;
            //    foreach (var activity in entities)
            //    {
            //        // activity.ReadEntity(activity.WriteEntity(null), null);
            //    }
            //} while (token != null);
            foreach (qna item in listCAEs)
            {
                TableOperation insertOperation = TableOperation.Insert(item);
                table.Execute(insertOperation);
            }
            
        }

       
        public class qna : TableEntity
        {
            public qna(string source, string question)
            {
                
                this.PartitionKey = source.Replace(' ', '-'); 
                string rk = source + question;
                rk = Regex.Replace(rk, @"/[^\w]/g", "");
                rk = Regex.Replace(rk, @"\s+", "");

                this.RowKey = rk;
            }
            public string question { get; set; }
            public string answer { get; set; }
            public string source { get; set; }


            public static string ToAzureKeyString(string str)
            {
                var sb = new StringBuilder();
                foreach (var c in str
                    .Where(c => c != '/'
                                && c != '\\'
                                && c != '#'
                                && c != '/'
                                && c != '?'
                                && c != '('
                                && c != ')'
                                && c != ','
                                 && c != ','
                                && !char.IsControl(c)))
                {
                    
                    if (char.IsWhiteSpace(c)) { c.ToString().Replace(c,'-'); }
                    sb.Append(c);
                }
                return sb.ToString();
            }
            public static qna FromTsv(string tsvLine)
            {
                string[] values = tsvLine.Split('\t');

                qna cae = new qna(values[2], ToAzureKeyString(values[0]));
                cae.question = Convert.ToString(values[0]);
                cae.answer = Convert.ToString(values[1]);
                cae.source = Convert.ToString(values[2]);

                return cae;
            }
        } 
    }
}
