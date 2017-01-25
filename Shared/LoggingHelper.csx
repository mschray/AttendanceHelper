using System;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types

public static class LoggingHelper 
{
    public static void WriteLogMessage(TraceWriter log, string function, string message,string messageSource = "Server")
    {
		// log to the standard Azure function Tracewriter
		log.Info($"Requestor:{messageSource} request Method: {function} - {message}");

        // Log it to table storage		
        WriteLogMessageToTable(function, message, messageSource);
		
    }
    
    private static void WriteLogMessageToTable(string function, string message,string messageSource = "Server")
    {
				
		// Parse the connection string and return a reference to the storage account.
		CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
    		CloudConfigurationManager.GetSetting("testattendancehelper_STORAGE"));
			
		// Create the table client.
		CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

		// Create the CloudTable object that represents the "AppLoggingTable" table.
		CloudTable table = tableClient.GetTableReference("AppLoggingTable");
        
        // Create the table if it doesn't exist.
        table.CreateIfNotExists();
		
        // Add log entry
       LogDataItem logRecord = new LogDataItem()
        {
            PartitionKey = "Functions",
            RowKey = Guid.NewGuid().ToString(),
            Requestor = messageSource,
            LogDate = DateTime.Now,
            LogData = $"Requestor:{messageSource} request Method: {function} - {message}"
        };
        
        // Create the TableOperation object that inserts the customer entity.
        TableOperation insertOperation = TableOperation.Insert(logRecord);

        // Execute the insert operation.
        table.Execute(insertOperation);
                
    }
}

public class LogDataItem : TableEntity
{
    public string Requestor { get; set; }
    public DateTime LogDate { get; set;}
    public string LogData { get; set; }
}