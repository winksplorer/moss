using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moss
{
    public class Moss
    {
        public static string VersionNum { get; } = "0.4";
        public static string Version { get; } = "moss-" + VersionNum;

        public static string[] CombineQuotedStrings(string[] input)
        {
            List<string> result = new List<string>();
            StringBuilder combined = new StringBuilder();
            bool inQuotes = false;

            foreach (var part in input)
            {
                if (part.StartsWith("\"") && !inQuotes)
                {
                    inQuotes = true;
                    combined.Clear();
                    combined.Append(part);
                }
                else if (inQuotes)
                {
                    combined.Append(" ").Append(part);
                    if (part.EndsWith("\""))
                    {
                        inQuotes = false;
                        result.Add(combined.ToString().Trim('\"')); // Remove surrounding quotes
                    }
                }
                else
                    result.Add(part);
            }

            // In case the input has a dangling quote, add the last combined string.
            if (inQuotes)
                result.Add(combined.ToString().Trim('\"'));

            return result.ToArray();
        }

        public static void PrintWithANSI(string str)
        {
            // Optimization: If there is no ANSI escape codes, then just don't bother
            if (!str.Contains("\\033["))
            {
                Console.Write(str);
                return;
            }

            // Otherwise, go through the string
            for (int i = 0; i < str.Length; i++)
            {
                // if str[i] is a \
                if (str[i] == '\\')
                {
                    // if it's a valid ansi escape code
                    if (str[i + 1] == '0' && str[i + 2] == '3' && str[i + 3] == '3' && str[i + 4] == '[')
                    {
                        // parse the escape code and find it's actual code
                        int ANSIEnd;
                        string code = string.Empty;
                        for (int j = i + 5; ; j++)
                        {
                            if (str[j] == 'm') { ANSIEnd = j; break; }
                            code += str[j];
                        }

                        if (code.Contains(';')) code = code.Split(';')[1]; // Ignore bold, underline, etc...

                        // Foreground
                        switch (code)
                        {
                            case "0": Console.ResetColor(); break;

                            // Foreground
                            case "30": Console.ForegroundColor = ConsoleColor.Black; break;
                            case "31": Console.ForegroundColor = ConsoleColor.DarkRed; break;
                            case "32": Console.ForegroundColor = ConsoleColor.DarkGreen; break;
                            case "33": Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                            case "34": Console.ForegroundColor = ConsoleColor.DarkBlue; break;
                            case "35": Console.ForegroundColor = ConsoleColor.DarkMagenta; break;
                            case "36": Console.ForegroundColor = ConsoleColor.DarkCyan; break;
                            case "37": Console.ForegroundColor = ConsoleColor.Gray; break;

                            // High Intensity Foreground
                            case "90": Console.ForegroundColor = ConsoleColor.DarkGray; break;
                            case "91": Console.ForegroundColor = ConsoleColor.Red; break;
                            case "92": Console.ForegroundColor = ConsoleColor.Green; break;
                            case "93": Console.ForegroundColor = ConsoleColor.Yellow; break;
                            case "94": Console.ForegroundColor = ConsoleColor.Blue; break;
                            case "95": Console.ForegroundColor = ConsoleColor.Magenta; break;
                            case "96": Console.ForegroundColor = ConsoleColor.Cyan; break;
                            case "97": Console.ForegroundColor = ConsoleColor.White; break;

                            // Background
                            case "40": Console.BackgroundColor = ConsoleColor.Black; break;
                            case "41": Console.BackgroundColor = ConsoleColor.DarkRed; break;
                            case "42": Console.BackgroundColor = ConsoleColor.DarkGreen; break;
                            case "43": Console.BackgroundColor = ConsoleColor.DarkYellow; break;
                            case "44": Console.BackgroundColor = ConsoleColor.DarkBlue; break;
                            case "45": Console.BackgroundColor = ConsoleColor.DarkMagenta; break;
                            case "46": Console.BackgroundColor = ConsoleColor.DarkCyan; break;
                            case "47": Console.BackgroundColor = ConsoleColor.Gray; break;

                            // High Intensity Background
                            case "100": Console.BackgroundColor = ConsoleColor.DarkGray; break;
                            case "101": Console.BackgroundColor = ConsoleColor.Red; break;
                            case "102": Console.BackgroundColor = ConsoleColor.Green; break;
                            case "103": Console.BackgroundColor = ConsoleColor.Yellow; break;
                            case "104": Console.BackgroundColor = ConsoleColor.Blue; break;
                            case "105": Console.BackgroundColor = ConsoleColor.Magenta; break;
                            case "106": Console.BackgroundColor = ConsoleColor.Cyan; break;
                            case "107": Console.BackgroundColor = ConsoleColor.White; break;

                            default:
                                break;
                        }

                        i = ANSIEnd;
                    }
                }
                else
                    Console.Write(str[i]);
            }
        }
    }
}
