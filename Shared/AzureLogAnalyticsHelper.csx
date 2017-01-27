using System;
using System.Net;
using Microsoft.Azure;
using System.Security.Cryptography;

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

        private static void Initialize()
        {
            // Parse the connection string and return the configruation setting.
            if (String.IsNullOrEmpty(CustomerId)) 
                CustomerId = CloudConfigurationManager.GetSetting("OPERATIONS_MANAGEMENT_WORKSPACE");
            
            if (String.IsNullOrEmpty(SharedKey)) 
                SharedKey = CloudConfigurationManager.GetSetting("OPERATIONS_MANAGEMENT_KEY");
            
            if (String.IsNullOrEmpty(LogName))
                LogName = CloudConfigurationManager.GetSetting("LOG_ANALYTICS_APPNAME") ;
        }

        public static void WriteLogEntry(string json)
        {
            // make sure all the needed app settings are loaded
            Initialize();
            
            // Create a hash for the API signature
            var datestring = DateTime.UtcNow.ToString("r");
            string stringToHash = "POST\n" + json.Length + "\napplication/json\n" + "x-ms-date:" + datestring + "\n/api/logs";
            
            
            string hashedString = BuildSignature(stringToHash, SharedKey);
            
            string signature = "SharedKey " + CustomerId + ":" + hashedString;
            
            PostData(signature, datestring, json);
        }
        
		// Build the API signature
        public static string BuildSignature(string message, string secret)
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

		// Send a request to the POST API endpoint
        public static void PostData(string signature, string date, string json)
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