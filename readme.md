# AzureStorageTools
Sample code to work with Azure Storage. First class is for batching data into Table. 

**Versions**

- v0.3 - 8/8/15 - Initial


## TableBatchManager
This sample class provides an easy way to batch data into Azure Table. There are a few rules to be aware of when batching data into Table:

- Max of 100 entities per batch 
- All entities in a batch have to be in the same partition
- Max of 4MB per batch*

The first two limitations are enforced in the TableBatchManager. The size limitation is not currently implemented. 

The class works by having a Dictionary to buffer entities by partition key. And when a partition reaches 100, it creates and runs a Task to insert the batch into the Table and continues working. 


## LoadTableData
This is a sample console application that shows how to use the TableBatchManager. It reads from a CSV file, creates an entity for each line, and feeds it to a TableBatchManager object. The CSV it is built to work with comes from the [Consumer Complaint Database on data.gov](http://catalog.data.gov/dataset/consumer-complaint-database)

It contains ~400k records. **If you want to try it out - be aware that you may have to adjust the column to entity field mapping (I've seen the column order change between downloads - don't know why).** On an A4 VM in the same region as storage I was able to achieve ~1200 records per second. 


## TODO
- Add an optional check for the size of the batch so that it doesn't exceed the limit