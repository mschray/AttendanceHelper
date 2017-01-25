#r "Microsoft.WindowsAzure.Storage"
#load "../Shared/FunctionNameHelper.csx"
#load "../Shared/LoggingHelper.csx"

using System.Net;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;

/// <summary>
/// The Logger is a function for the client to send log messages to the server.  This fucntion uses
/// the same code as the server to accomplish its logging with the exception of adding the optional parameter
/// client to the logging request 
/// </summary>
/// <param name="req">Incoming request</param>
/// <param name="log">Transient log provided by Azure Function infrastructure</param>
/// <returns></returns>
public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    try
    {
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" invoked.");

        dynamic requestData = await req.Content.ReadAsAsync<object>();
        string logData = requestData?.LogData;
        DateTime? logDate = requestData?.LogDate; // DateTime? handles null input

        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" processing.  Input was {requestData}");
        
        if (String.IsNullOrEmpty(logData) || logDate == null )
        {
            LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" Please pass a LogData item in the request body. Input was {requestData}");
            
            return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a LogData item in the request body");
        }

        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" processing.  Input was {requestData}");

        // log the data passed from the client - note I am provided the optional Client tag to incidate data from the cleint
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),logData, "Client");
        
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),"Logged Client Details");

        return req.CreateResponse(HttpStatusCode.Created,"Logged Details");

    }
    catch (Exception ex)
    {
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" execption occured with an {ex.Message} and stacktrace of {ex.StackTrace}");
        
        return req.CreateResponse(HttpStatusCode.BadRequest, $"Method:{FunctionNameHelper.GetFunctionName()} - see application log for error details");        
    }

}
