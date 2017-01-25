using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types

/// <summary>
/// This is the format Person data used by most of the functions
/// </summary>
public class Person : TableEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IDNumber { get; set; }
    public string CardNumber { get; set; }
    public string Course { get; set; }
    public DateTime ScanDate { get; set;}
    public override string ToString()
    {
        return $"Person: {FirstName} {LastName} {IDNumber} {CardNumber} {Course} {ScanDate}";
    }

}