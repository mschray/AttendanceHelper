#r "Microsoft.WindowsAzure.Storage"
#load "../Shared/FunctionNameHelper.csx"

using System.Net;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, ICollector<LogDataItem> outTable, TraceWriter log)
{
    try
    {
        log.Info($"Method:{FunctionNameHelper.GetFunctionName()} invoked. Input was {req} ");

        dynamic requestData = await req.Content.ReadAsAsync<object>();
        string logData = requestData?.LogData;
        DateTime? logDate = requestData?.LogDate; // DateTime? handles null input
        
        if (String.IsNullOrEmpty(logData) || logDate == null )
        {
            log.Info($"Method:{FunctionNameHelper.GetFunctionName()} Please pass a LogData item in the request body. Input was {req}");

            return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a LogData item in the request body");
        }

        log.Info($"Method:{FunctionNameHelper.GetFunctionName()} Processing");

        // Add log entry - cast the DateTime? to a DateTime
        outTable.Add(new LogDataItem()
        {
            PartitionKey = "Functions",
            RowKey = Guid.NewGuid().ToString(),
            Requestor = "Client",
            LogDate = (DateTime)logDate,
            LogData = logData
            
        });

        log.Info($"Method:{FunctionNameHelper.GetFunctionName()} Completed");

        return req.CreateResponse(HttpStatusCode.Created,"Logged Details");

    }
    catch (Exception ex)
    {
        log.Info($"Method:{FunctionNameHelper.GetFunctionName()} execption occured with an {ex.Message} and stacktrace of {ex.StackTrace}");   

        return req.CreateResponse(HttpStatusCode.BadRequest, $"Method:{FunctionNameHelper.GetFunctionName()} - see application log for error details");        
    }

}

public class LogDataItem : TableEntity
{
    public string Requestor { get; set; }
    public DateTime LogDate { get; set;}
    public string LogData { get; set; }
}