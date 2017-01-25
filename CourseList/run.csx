#r "Microsoft.WindowsAzure.Storage"
#load "../Shared/FunctionNameHelper.csx"
#load "../Shared/LoggingHelper.csx"

using System.Net;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;

public static HttpResponseMessage Run(HttpRequestMessage req, IQueryable<Course> inTable, TraceWriter log)
{
    try
    {
        //log.Info($"Method:{FunctionNameHelper.GetFunctionName()} $" invoked. Input was {req}");
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" invoked.");
                
        //var query = from Course in inTable select Course;

        if (inTable.ToList().Count <= 0)
        {
            log.Info($"Method:{FunctionNameHelper.GetFunctionName()} No courses found.");

            return req.CreateResponse(HttpStatusCode.NoContent, $"Method:{FunctionNameHelper.GetFunctionName()} No courses Found.");
        }
            
        else 
        {
            log.Info($"Method:{FunctionNameHelper.GetFunctionName()} Course(s) found {Flatten(inTable.ToList())}");

            return req.CreateResponse(HttpStatusCode.OK, inTable.ToList() );
        }   

        log.Info($"Method:{FunctionNameHelper.GetFunctionName()} Query complete. The result had a length of {inTable.ToList().Count}");
     }   
     catch (Exception ex)
     {
        log.Info($"Method:{FunctionNameHelper.GetFunctionName()} execption occured with an {ex.Message} and stacktrace of {ex.StackTrace}");
                
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

public class Course : TableEntity
{
    public string CourseNumber { get; set; }
    public string CourseName { get; set; }
    public string LeadInstructor { get; set; }
    public override string ToString()
    {
        return $"CourseNumber {CourseNumber}, CourseName {CourseName}, LeadInstructor {LeadInstructor}";
    }
}