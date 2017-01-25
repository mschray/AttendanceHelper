#r "Microsoft.WindowsAzure.Storage"
#load "../Shared/FunctionNameHelper.csx"
#load "../Shared/LoggingHelper.csx"
#load "../Shared/Person.csx"

using System.Net;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;

/// <summary>
/// The ANumberLoader is a function that is used to load all of the Anumbers for students 
/// into the backend Azure tables.  The data loaded into the person object and that is
/// serialed into ANumberTable
/// </summary>
/// <param name="req">Incoming request</param>
/// <param name="outTable">ANumberTable that incoming data is inserted into</param>
/// <param name="log">Transient log provided by Azure Function infrastructure</param>
/// <returns></returns>
public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, ICollector<Person> outTable, TraceWriter log)
{

    try
    {

        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(), $" invoked.  Input was {req}" );

        dynamic requestData = await req.Content.ReadAsAsync<object>();
        string firstName = requestData?.FirstName;
        string lastName = requestData?.LastName;
        string idNumber = requestData?.IDNumber;
        DateTime scanDate = requestData?.ScanDate;

        if (String.IsNullOrEmpty(firstName) || String.IsNullOrEmpty(lastName) || String.IsNullOrEmpty(idNumber))
        {

            LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" Invalid data.  Input was {requestData}" );
                        
            return req.CreateResponse(HttpStatusCode.BadRequest, $"Method:{FunctionNameHelper.GetFunctionName()} Please pass all input fields in the request body");
        }

        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" begin processing.  Input was {requestData}" );

        outTable.Add(new Person()
        {
            PartitionKey = "ANumblerLookup",
            RowKey = Guid.NewGuid().ToString(),
            FirstName = firstName,
            LastName = lastName,
            IDNumber = idNumber,
            ScanDate = scanDate,
        });

 
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" Completed.  Input was {requestData}");

        return req.CreateResponse(HttpStatusCode.Created);
    }
    catch (System.Exception ex)
    {
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(), $" execption occured with an {ex.Message} and stacktrace of {ex.StackTrace}" ); 

        return req.CreateResponse(HttpStatusCode.BadRequest, "See application log for error details");   
    }

}
