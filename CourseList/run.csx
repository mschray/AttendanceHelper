#r "Microsoft.WindowsAzure.Storage"
#load "../Shared/FunctionNameHelper.csx"
#load "../Shared/LoggingHelper.csx"
#load "../Shared/Course.csx"

using System.Net;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;

/// <summary>
/// The CourseList function returns a complete list of courses covered by the attendance system 
/// </summary>
/// <param name="req">Incoming request</param>
/// <param name="inTable">CourseTable that provides a complete list of courses</param>
/// <param name="log">Transient log provided by Azure Function infrastructure</param>
/// <returns></returns>
public static HttpResponseMessage Run(HttpRequestMessage req, IQueryable<Course> inTable, TraceWriter log)
{
    try
    {
     
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" invoked.");
                
        if (inTable.ToList().Count <= 0)
        {
            LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" No courses found.");
                 
            return req.CreateResponse(HttpStatusCode.NoContent, $"Method:{FunctionNameHelper.GetFunctionName()} No courses Found.");
        }
            
        else 
        {
            LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" {inTable.ToList().Count} Course(s) found {Flatten(inTable.ToList())}");

            return req.CreateResponse(HttpStatusCode.OK, inTable.ToList() );
        }   
     }   
     catch (Exception ex)
     {
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" execption occured with an {ex.Message} and stacktrace of {ex.StackTrace}");
                
        return req.CreateResponse(HttpStatusCode.NoContent, "Method:{FunctionNameHelper.GetFunctionName()} No courses Found");
        
     }
     
}

private static string Flatten(List<Course> anArray)
{
    StringBuilder strBuilder = new StringBuilder();

    foreach (var item in anArray)
    {
        strBuilder.Append(item + ", ");
    }

    return strBuilder.ToString();

}