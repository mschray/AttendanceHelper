using System;
using System.Text.RegularExpressions;
using System.Reflection;

public static class FunctionNameHelper 
{
    /// <summary>
    /// This method grabs the assembly name of the calling code and return just the assembly name, which in Azure functions (mostly)
    /// is the name of the calling function. 
    /// </summary>
    /// <returns>The name of the assembly simplied down to just the assembly name (not version # and such) </returns>
    public static string GetFunctionName()
    {

        string fullDisplayName = Assembly.GetCallingAssembly().GetName().Name;

		string pattern = @"[^f-]*\w*(?=__)";
        Regex rgx = new Regex(pattern);
        Match result = rgx.Match(fullDisplayName);
        if (result.Success)
        	return result.Value;
        else
        	return fullDisplayName;
		
    }
}