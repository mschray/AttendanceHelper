#r "Microsoft.WindowsAzure.Storage"
#load "../Shared/FunctionNameHelper.csx"
#load "../Shared/LoggingHelper.csx"
#load "../Shared/Person.csx"

using System.Net;
using Microsoft.WindowsAzure.Storage.Table;

/// <summary>
/// The ANumberLookup function takes a student ID (that was scanned) and looks to see if that student is
/// in the ANumberTable.  If they are we know the first and last name.  If not we don't have that info.
/// </summary>
/// <param name="req">Incoming request</param>
/// <param name="inTable">ANUmberTable that provides data about student</param>
/// <param name="log">Transient log provided by Azure Function infrastructure</param>
/// <returns></returns>
public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, IQueryable<Person> inTable, TraceWriter log)
{
    Person foundPerson = null;
    string aNumber = "";

    try
    {

        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" invoked.  Input was {req}" );

        dynamic requestData = await req.Content.ReadAsAsync<object>();   
        aNumber = requestData?.ANumber;

        if (String.IsNullOrEmpty(aNumber))
        {

            LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" Invalid data.  Input was {requestData}" );
                        
            return req.CreateResponse(HttpStatusCode.BadRequest, $"Method:{FunctionNameHelper.GetFunctionName()} Please pass all input fields in the request body");
        }

        var query = from person in inTable where person.IDNumber == aNumber select person;
        foreach (Person person in query)
        {
            LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" found person {person}" );    

            foundPerson = person;
        }
     }   
     catch (Exception ex)
     {
         
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" execption occured with an {ex.Message} and stacktrace of {ex.StackTrace}"); 

        return req.CreateResponse(HttpStatusCode.NoContent, "$Method:{FunctionNameHelper.GetFunctionName()} see application for exception");
     }
 
     
     if (foundPerson == null)
     {
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" Anumber {aNumber} not found");

        return req.CreateResponse(HttpStatusCode.NoContent, $"Method:{FunctionNameHelper.GetFunctionName()} Anumber {aNumber} not found");
     }
    else
    {
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" Anumber {aNumber} found for person {foundPerson}");

        return req.CreateResponse(HttpStatusCode.OK, foundPerson );
    }   
}
