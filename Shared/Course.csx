using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types

/// <summary>
/// This is the format Course data used to store basic course info
/// </summary>
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