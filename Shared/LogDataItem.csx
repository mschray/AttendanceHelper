#r "Microsoft.WindowsAzure.Storage"

using System;
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types

/// <summary>
/// This is the format LogDataItem used to store basic log data
/// </summary>
public class LogDataItem : TableEntity
{
    public string Requestor { get; set; }
    public string AzureFunction { get; set; }
    public string Method { get; set; }
    public DateTime LogDate { get; set;}
    public string LogData { get; set; }
}