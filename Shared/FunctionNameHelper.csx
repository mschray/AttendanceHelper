using System;
using System.Text.RegularExpressions;
using System.Reflection;

public static class FunctionNameHelper 
{
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