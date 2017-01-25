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

        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),"Called with input {req}" );

        //log.Info($"Method:{FunctionNameHelper.GetFunctionName()} called with input was {req}");

        dynamic requestData = await req.Content.ReadAsAsync<object>();
        string firstName = requestData?.FirstName;
        string lastName = requestData?.LastName;
        string idNumber = requestData?.IDNumber;
        string cardNumber = requestData?.CardNumber;
        string course = requestData?.Course;
        DateTime scanDate = requestData?.ScanDate;
        
        if (String.IsNullOrEmpty(firstName) || String.IsNullOrEmpty(lastName) || String.IsNullOrEmpty(idNumber) || String.IsNullOrEmpty(cardNumber) || String.IsNullOrEmpty(course) )
        {
                LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" invalid input in request object.  Input was {requestData}" );

                //log.Info($"Method:{FunctionNameHelper.GetFunctionName()} invalid input in request object.  Input was {requestData}");

                return req.CreateResponse(HttpStatusCode.BadRequest, "Please make sure that all request fields are present in the request body");
        }
 
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(), $" Begin processing.  Input was {requestData}");
        //log.Info($"Method:{FunctionNameHelper.GetFunctionName()} Begin processing.  Input was {requestData}");

        outTable.Add(new Person()
        {
            PartitionKey = "Functions",
            RowKey = Guid.NewGuid().ToString(),
            FirstName = firstName,
            LastName = lastName,
            IDNumber = idNumber,
            CardNumber = cardNumber,
            ScanDate = scanDate,
            Course = course
            
        });

        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" ID Card added to CardDataTable.  Input was {requestData}" );
        //log.Info($"Method:{FunctionNameHelper.GetFunctionName()} ID Card added to CardDataTable.  Input was {requestData}");

        return req.CreateResponse(HttpStatusCode.Created);

    }
    catch (System.Exception ex)
    {
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" execption occured with an {ex.Message} and stacktrace of {ex.StackTrace}" );
        //log.Info($"Method:{FunctionNameHelper.GetFunctionName()} execption occured with an {ex.Message} and stacktrace of {ex.StackTrace}");

        return req.CreateResponse(HttpStatusCode.BadRequest, $"Method:{FunctionNameHelper.GetFunctionName()} - see application log for error details");   
    }
}

public class Person : TableEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IDNumber { get; set; }
    public string CardNumber { get; set; }
    public string Course { get; set; }
    public DateTime ScanDate { get; set;}
}