#r "Microsoft.WindowsAzure.Storage"
#load "../Shared/FunctionNameHelper.csx"
#load "../Shared/LoggingHelper.csx"
#load "../Shared/Person.csx"

using System.Net;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;

/// <summary>
/// The LookupIDCard function returns a person with a matching ID card number from CardDataTable 
/// </summary>
/// <param name="req">Incoming request</param>
/// <param name="inTable">CardDataTable that is a list of known student ID cards</param>
/// <param name="log">Transient log provided by Azure Function infrastructure</param>
/// <returns></returns>
public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, IQueryable<Person> inTable, TraceWriter log)
{
    try
    {
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" invoked.");
        
        dynamic requestData = await req.Content.ReadAsAsync<object>();   
        string inputCardNumber = requestData?.IDNumber;

        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" processing.  Input was {requestData}");
                

        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" Seach for card # {inputCardNumber}");

        Person foundPerson = null;

        var query = from person in inTable where person.CardNumber == inputCardNumber select person;
                
        foreach (dynamic person in query)
        {
            
            LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" Found person {person}");

            foundPerson = person;
        }
        
        if (foundPerson == null)
        {
            LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" Did not find person via card number.  Input was {requestData}");

            return req.CreateResponse(HttpStatusCode.NoContent, "Card not found");
        }
        else
        {
            LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" Found Person {foundPerson}. Input was {requestData}");

            return req.CreateResponse(HttpStatusCode.OK, foundPerson );
        }    
            
    }
    catch(Exception ex)
    {
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" execption occured with an {ex.Message} and stacktrace of {ex.StackTrace}");
        
        return req.CreateResponse(HttpStatusCode.BadRequest, $"Method:{FunctionNameHelper.GetFunctionName()} - see application log for error details");        
    }

}
