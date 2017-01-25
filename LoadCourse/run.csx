#r "Microsoft.WindowsAzure.Storage"
#load "../Shared/FunctionNameHelper.csx"
#load "../Shared/LoggingHelper.csx"

using System.Net;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, ICollector<Course> outTable, TraceWriter log)
{
    try
    {
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" invoked.  Input was {req}" );

        dynamic requestData = await req.Content.ReadAsAsync<object>();
        string courseNumber = requestData?.CourseNumber;
        string courseName = requestData?.CourseName;
        string leadInstructor = requestData?.LeadInstructor;

        if (String.IsNullOrEmpty(courseName) || String.IsNullOrEmpty(courseNumber) || String.IsNullOrEmpty(leadInstructor)  )
        {

            LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" invalid input one or more fields blank.  Input was {requestData}");

            return req.CreateResponse(HttpStatusCode.BadRequest, $"Method:{FunctionNameHelper.GetFunctionName()} Please pass all required fields in as part of the request body");
        }

        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" Processing started.  Input was {req}");


        outTable.Add(new Course()
        {
            PartitionKey = "CourseLookup",
            RowKey = Guid.NewGuid().ToString(),
            CourseNumber = courseNumber,
            CourseName = courseName,
            LeadInstructor = leadInstructor
        });

 
        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" completed.  Input was {requestData}");

        return req.CreateResponse(HttpStatusCode.Created);

    }
    catch (Exception ex)
    {

        LoggingHelper.WriteLogMessage(log, FunctionNameHelper.GetFunctionName(),$" execption occured with an {ex.Message} and stacktrace of {ex.StackTrace}");

        return req.CreateResponse(HttpStatusCode.BadRequest, "Method:{FunctionNameHelper.GetFunctionName()} - see application log for error details");        
    }

}

public class Course : TableEntity
{
    public string CourseNumber { get; set; }
    public string CourseName { get; set; }
    public string LeadInstructor { get; set; }
}