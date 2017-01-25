#r "Microsoft.WindowsAzure.Storage"
#load "../Shared/FunctionNameHelper.csx"
#load "../Shared/LoggingHelper.csx"

using System.Net;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, ICollector<Person> outTable, TraceWriter log)
{

    try
    {

        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(), $" invoked.  Input was {req}" );

        //log.Info($"Method:{FunctionNameHelper.GetFunctionName()} invoked.  Input was {req}");

        dynamic requestData = await req.Content.ReadAsAsync<object>();
        string firstName = requestData?.FirstName;
        string lastName = requestData?.LastName;
        string idNumber = requestData?.IDNumber;

        if (String.IsNullOrEmpty(firstName) || String.IsNullOrEmpty(lastName) || String.IsNullOrEmpty(idNumber))
        {

            LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" Invalid data.  Input was {requestData}" );
            //log.Info($"Method:{FunctionNameHelper.GetFunctionName()} Invalid data.  Input was {requestData}");
                        
            return req.CreateResponse(HttpStatusCode.BadRequest, $"Method:{FunctionNameHelper.GetFunctionName()} Please pass all input fields in the request body");
        }

        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" begin processing.  Input was {requestData}" );
        //log.Info($"Method:{FunctionNameHelper.GetFunctionName()} begin processing.  Input was {requestData}");

        outTable.Add(new Person()
        {
            PartitionKey = "ANumblerLookup",
            RowKey = Guid.NewGuid().ToString(),
            FirstName = firstName,
            LastName = lastName,
            IDNumber = idNumber
        });

 
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" Completed.  Input was {requestData}");
        //log.Info($"Method:{FunctionNameHelper.GetFunctionName()} Completed.  Input was {requestData}");

        return req.CreateResponse(HttpStatusCode.Created);
    }
    catch (System.Exception ex)
    {
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(), $" execption occured with an {ex.Message} and stacktrace of {ex.StackTrace}" ); 
        //log.Info($"Method:{FunctionNameHelper.GetFunctionName()} execption occured with an {ex.Message} and stacktrace of {ex.StackTrace}");

        return req.CreateResponse(HttpStatusCode.BadRequest, "See application log for error details");   

    }

}

public class Person : TableEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IDNumber { get; set; }
}