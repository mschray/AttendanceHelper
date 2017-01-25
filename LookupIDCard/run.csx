#r "Microsoft.WindowsAzure.Storage"
#load "../Shared/FunctionNameHelper.csx"
#load "../Shared/LoggingHelper.csx"

using System.Net;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, IQueryable<Person> inTable, TraceWriter log)
{
    try
    {
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" invoked.  Input was {req}");
        
        dynamic requestData = await req.Content.ReadAsAsync<object>();   
        string inputCardNumber = requestData?.CardNumber;

        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" processing.  Input was {requestData}");
                
        Person foundPerson = null;
        var query = from person in inTable where person.CardNumber == inputCardNumber select person;
        foreach (Person person in query)
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

public class Person : TableEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IDNumber { get; set; }
    public string CardNumber { get; set; }
}