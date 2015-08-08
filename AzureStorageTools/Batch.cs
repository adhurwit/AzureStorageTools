    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;


namespace AzureStorageTools
{
    public class Batch
    {
        List<TableEntity> Entities { get; set; }
        string TableName { get; set; }
        CloudTable Table { get; set; }
        bool Merge { get; set; }
        public Batch(CloudTable table, List<TableEntity> entities, bool merge) 
        {
            Entities = entities;
            Table = table;
            Merge = merge;
            TableBatchManager.LogInfo("Batch created");
        }

        public void Load()
        {
            TableBatchManager.LogInfo("Load batch called");
            try
            {
            TableBatchOperation op = new TableBatchOperation();
            foreach (var e in Entities)
            {
                if (Merge)
                    op.InsertOrMerge(e);
                else
                    op.InsertOrReplace(e);
            }
            Table.ExecuteBatch(op);
            TableBatchManager.LogInfo("exec batch");
            }
            catch(Exception ex)
            {
                TableBatchManager.LogError("Error: " + ex);

            }
        }

    }
}
