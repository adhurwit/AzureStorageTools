using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadTableData
{
    public class ComplaintEntity : TableEntity
    {
        public ComplaintEntity() { }
        public ComplaintEntity(string product, int cid)
        {
            PartitionKey = product;
            RowKey = cid.ToString();
        }
        
        public DateTime Received { get; set; }
        public string SubProduct { get; set; }
        public string Issue { get; set; }
        public string SubIssue { get; set; }
        public string Narrative { get; set; }
        public string Response { get; set; }
        public string Company { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Submitted { get; set; }
        public DateTime Sent { get; set; }
        public string ResponseStatus { get; set; }
        public string Timely { get; set; }
        public string Disputed { get; set; }

    }
}
