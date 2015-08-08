using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureStorageTools;
using System.IO;
using CsvHelper;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading;
using System.Configuration;

namespace LoadTableData
{
    class Program
    {
        static void Main(string[] args)
        {
            // CSV from http://catalog.data.gov/dataset/consumer-complaint-database
            // args[0] = tablename
            // args[1] = csv filename

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var batchman = new TableBatchManager(ConfigurationManager.AppSettings["accountName"],
                ConfigurationManager.AppSettings["accountName"],
                args[0], true);

            // open read file
            var csv = new CsvReader(File.OpenText(args[1]));
            csv.Configuration.HasHeaderRecord = true;
            int i = 0;
            while (csv.Read())
            {
                var c = new ComplaintEntity(csv.GetField<string>(1),
                    csv.GetField<int>(15));

                c.Received = csv.GetField<DateTime>(0);
                c.SubProduct = csv.GetField<string>(2);
                c.Issue = csv.GetField<string>(3);
                c.SubIssue = csv.GetField<string>(4);
                c.Narrative = csv.GetField<string>(5);
                c.Response = csv.GetField<string>(6);
                c.Company = csv.GetField<string>(7);
                c.State = csv.GetField<string>(8);
                c.Zip = csv.GetField<string>(9);
                c.Submitted = csv.GetField<string>(10);
                c.Sent = csv.GetField<DateTime>(11);
                c.ResponseStatus = csv.GetField<string>(12);
                c.Timely = csv.GetField<string>(13);
                c.Disputed = csv.GetField<string>(14);

                i++; Console.WriteLine("row: " + i);
                batchman.AddEntity(c);
            }

            batchman.Finish();

            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalSeconds);
        }

    }
}
