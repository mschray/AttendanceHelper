#load "LogDataItem.csx"
#load "AzureLogAnalyticsHelper.csx"
#r "Newtonsoft.Json"

using System;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types
using Newtonsoft.Json;

public static class LoggingHelper 
{
    
    /// <summary>
    /// This method echoes the incoming log message to the Azure Functions (transient log) 
    /// </summary>
    /// <param name="log">Reference to the TraceWriter passed into the run method</param>
    /// <param name="function">Name of the calling Azure function</param>
    /// <param name="message">Message to log</param>
    /// <param name="messageSource">Name of the log message source defaults to server</param>
    /// <param name="messageSource">Name of the calling method</param>
    /// <returns></returns>
    public static void WriteLogMessage(TraceWriter log, string azureFunction, string message,string messageSource = "Server",[System.Runtime.CompilerServices.CallerMemberName] string function = "")
    {
		// log to the standard Azure function Tracewriter
		log.Info($"Requestor: {messageSource}, Azure Function {azureFunction}, Method: {function} - {message}");

        // Log it to table storage		
        WriteLogMessageToTable(azureFunction, function, message, messageSource);
		
    }
   
   /// <summary>
    /// This method echoes the incoming log message to the Azure Functions (transient log) 
    /// </summary>
    /// <param name="log">Reference to the TraceWriter passed into the run method</param>
    /// <param name="function">Name of the calling Azure function</param>
    /// <param name="message">Message to log</param>
    /// <param name="messageSource">Name of the log message source defaults to server</param>
    /// <param name="messageSource">Name of the calling method</param>
    /// <returns></returns>
    public static void WriteLogAnalticsEntry(TraceWriter log, string azureFunction, string message,string messageSource = "Server",[System.Runtime.CompilerServices.CallerMemberName] string function = "")
    {
		// log to the standard Azure function Tracewriter
		log.Info($"Requestor: {messageSource}, Azure Function {azureFunction}, Method: {function} - {message}");

        // Log it to table storage		
        WriteLogMessageToLogAnalytics(azureFunction, function, message, messageSource);
		
    }
   
    /// <summary>
    /// This method writes the incoming log message Azure Table Storage to make log messages persistent 
    /// </summary>
    /// <param name="function">Name of the calling function</param>
    /// <param name="message">Message to log</param>
    /// <param name="messageSource">Name of the log message source defaults to server</param>
    /// <param name="messageSource">Name of the calling method</param>
    /// <returns></returns>
    private static void WriteLogMessageToTable(string azureFunction,string function, string message,string messageSource = "Server")
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
            AzureFunction = azureFunction,
            Method = function,
            LogDate = DateTime.Now,
            LogData = message
        };
        
        // Create the TableOperation object that inserts the customer entity.
        TableOperation insertOperation = TableOperation.Insert(logRecord);

        // Execute the insert operation.
        table.Execute(insertOperation);
                
    }

    /// <summary>
    /// This method writes the incoming log message Azure log Analytics service 
    /// </summary>
    /// <param name="function">Name of the calling function</param>
    /// <param name="message">Message to log</param>
    /// <param name="messageSource">Name of the log message source defaults to server</param>
    /// <param name="messageSource">Name of the calling method</param>
    /// <returns></returns>
    private static void WriteLogMessageToLogAnalytics(string azureFunction,string function, string message,string messageSource = "Server")
    {
         // Add log entry
         LogDataItem logRecord = new LogDataItem()
         {
            PartitionKey = "Functions",
            RowKey = Guid.NewGuid().ToString(),
            Requestor = messageSource,
            AzureFunction = azureFunction,
            Method = function,
            LogDate = DateTime.Now,
            LogData = message
         };
         
         var json = JsonConvert.SerializeObject(logRecord);
         
         AzureLogAnalyticsHelper.WriteLogEntry(json);             
    }    
}
