using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moss
{
    public class Prompt
    {
        public static string DefaultPrompt { get; } = "-\\s-\\v\\$ ";
        public static string GetPrompt(Dictionary<string, string> environmentVariables)
        {
            if (environmentVariables.ContainsKey("PROMPT"))
                return GetActualString(environmentVariables["PROMPT"], environmentVariables);
            else
                return GetActualString(DefaultPrompt, environmentVariables);
        }

        public static string GetActualString(string str, Dictionary<string, string> environmentVariables)
        {
            // TODO: implement time
            return str.Replace("\\u", environmentVariables["USER"]) // Username
                .Replace("\\h", "cosmos") // Hostname (short)
                .Replace("\\H", "cosmos.gocosmos.org") // Hostname
                .Replace("\\w", environmentVariables["PWD"]) // Working Directory
                .Replace("\\W", Path.GetFileNameWithoutExtension(environmentVariables["PWD"])) // Working Directory (Basename)
                .Replace("\\n", "\n") // Newline
                .Replace("\\r", "\r") // Carriage Return
                // Keep bell (\a) as is
                // Keep terminal (\l) as is
                .Replace("\\s", environmentVariables["SHELL"]) // Shell
                .Replace("\\v", Moss.VersionNum) // moss Version
                .Replace("\\v", Moss.VersionNum) // moss Release
                // Keep History Number (\!) as is
                // Keep Command Number (\#) as is
                // Keep Jobs (\j) as is
                .Replace("\\$", GetPromptSymbol(environmentVariables["USER"]).ToString()) // Prompt Sign
                .Replace("$?", environmentVariables["RET"]) // Exit Status
                .Replace("\\\\", "\\"); // /
        }

        public static char GetPromptSymbol(string user)
        {
            if (user == "root")
                return '#';
            else
                return '$';
        }
    }
}
