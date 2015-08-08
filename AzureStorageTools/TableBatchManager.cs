using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace AzureStorageTools
{
    public class TableBatchManager
    {
        string AccountName { get; set; }
        string AccountKey { get; set; }
        CloudStorageAccount Account { get; set; }
        string TableName { get; set; }
        Dictionary<string, List<TableEntity>> BatchBuffer = new Dictionary<string, List<TableEntity>>();
        public int MaxPartitions = 1000;
        bool Merge { get; set; }
        List<Task> Tasks = new List<Task>();

        public TableBatchManager(string accountName, string accountKey, string tableName, bool merge = true)
        {
            AccountName = accountName;
            AccountKey = accountKey;
            TableName = tableName;
            Merge = merge;

            StorageCredentials cre = new StorageCredentials(AccountName, AccountKey);
            Account = new CloudStorageAccount(cre, false);
            ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(Account.TableEndpoint);
            tableServicePoint.UseNagleAlgorithm = false;

            CloudTableClient cl = Account.CreateCloudTableClient();
            CloudTable table = cl.GetTableReference(TableName);
            table.CreateIfNotExists();
        }

        public void AddEntity(TableEntity entity) 
        {
            if (! BatchBuffer.ContainsKey(entity.PartitionKey))
                BatchBuffer.Add(entity.PartitionKey, new List<TableEntity>() { entity });
            else
            {
                var part = BatchBuffer[entity.PartitionKey];
                part.Add(entity);

                // send batch if at limit
                if (part.Count == 100)
                {
                    TableBatchManager.LogInfo("partition has 100");
                    LoadBatch(part);
                    BatchBuffer.Remove(entity.PartitionKey);
                }
            }

            if (BatchBuffer.Keys.Count == MaxPartitions)
                ClearBuffer();
        }

        public void Finish()
        {
            ClearBuffer();
            Task.WaitAll(Tasks.ToArray());
        }

        public static void LogInfo(string log)
        {
            Console.WriteLine("[INFO] " + log);
        }
        public static void LogError(string log)
        {
            Console.WriteLine("[ERROR] " + log);
        }

        void LoadBatch(List<TableEntity> b)
        {
            CloudTableClient cl = Account.CreateCloudTableClient();
            cl.DefaultRequestOptions.PayloadFormat = TablePayloadFormat.JsonNoMetadata;

            CloudTable table = cl.GetTableReference(TableName);
            Batch batch = new Batch(table, b, Merge);
            TableBatchManager.LogInfo("run task");
            var t = Task.Run(()=>batch.Load());
            Tasks.Add(t);
            TableBatchManager.LogInfo("Tasks: " + Tasks.Count);
        }



        void ClearBuffer()
        {
            TableBatchManager.LogInfo("clear buffer");
            foreach(var b in BatchBuffer.Keys)
                LoadBatch(BatchBuffer[b]);
            BatchBuffer = new Dictionary<string, List<TableEntity>>();
        }
    }
}
