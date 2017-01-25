using System;
using System.Net;
using Microsoft.Azure;
using System.Security.Cryptography;

/// <summary>
/// This class provide a wrapper to Azure Log Analytics service.  To use it you must
/// make sure you have the follow configuration settings (in AppSettings in Azure or in config files
/// - OPERATIONS_MANAGEMENT_WORKSPACE - Operations Manager Workspace ID
/// - OPERATIONS_MANAGEMENT_KEY - Key for accessing Operations Manager Service
/// - LOG_ANALYTICS_APPNAME - Name that will appear the app creating log records
/// </summary>
class AzureLogAnalyticsHelper
{

    // Update customerId to your Operations Management Suite workspace ID
    static string CustomerId = "";
        
    // For sharedKey, use either the primary or the secondary Connected Sources client authentication key   
    static string SharedKey = "";

    // LogName is name of the event type that is being submitted to Log Analytics
    static string LogName = "";

    // You can use an optional field to specify the timestamp from the data. If the time field is not specified, Log Analytics assumes the time is the message ingestion time
    static string TimeStampField = "";

    /// <summary>
    /// Called internally to get configuration information
    /// </summary>
    private static void Initialize()
    {
        // Parse the connection string and return a reference to the storage account.
		CustomerId = CloudConfigurationManager.GetSetting("OPERATIONS_MANAGEMENT_WORKSPACE");
            
        SharedKey = CloudConfigurationManager.GetSetting("OPERATIONS_MANAGEMENT_KEY");
            
        LogName = CloudConfigurationManager.GetSetting("LOG_ANALYTICS_APPNAME") ;
            
    }

    /// <summary>
    /// Public method that callers use to write there record to Azure Log Analytics
    /// </summary>
    /// <param name="json"></param>
    public static void WriteLogEntry(string json)
    {
        Initialize();
            
        // Create a hash for the API signature
        var datestring = DateTime.UtcNow.ToString("r");
        string stringToHash = "POST\n" + json.Length + "\napplication/json\n" + "x-ms-date:" + datestring + "\n/api/logs";
            
            
        string hashedString = BuildSignature(stringToHash, SharedKey);
            
        string signature = "SharedKey " + CustomerId + ":" + hashedString;
            
        PostData(signature, datestring, json);
    }

     
    /// <summary>
    ///Build the API signature 
    /// </summary>
    /// <param name="message">The message to build</param>
    /// <param name="secret">Key to service</param>
    /// <returns></returns>
    private static string BuildSignature(string message, string secret)
    {
        var encoding = new System.Text.ASCIIEncoding();
        byte[] keyByte = Convert.FromBase64String(secret);
        byte[] messageBytes = encoding.GetBytes(message);
        using (var hmacsha256 = new HMACSHA256(keyByte))
        {
            byte[] hash = hmacsha256.ComputeHash(messageBytes);
            return Convert.ToBase64String(hash);
        }
    }
    
    /// <summary>
    /// Send a request to the POST Azure Log Analytics
    /// </summary>
    /// <param name="signature">Signature from BuildSignature</param>
    /// <param name="date">Date</param>
    /// <param name="json">JSON message to log</param>
    private static void PostData(string signature, string date, string json)
    {
        string url = "https://"+ CustomerId +".ods.opinsights.azure.com/api/logs?api-version=2016-04-01";
                        
        using (var client = new WebClient())
        {
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            client.Headers.Add("Log-Type", LogName);
            client.Headers.Add("Authorization", signature);
            client.Headers.Add("x-ms-date", date);
            client.Headers.Add("time-generated-field", TimeStampField);
            client.UploadString(new Uri(url), "POST", json);
        }
            
            
    }
}