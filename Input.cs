using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace moss
{
    public class Input
    {
        List<string> cmdHistory;
        List<Command> Commands;

        public Input(List<Command> commands)
        {
            Commands = new(commands);
            Command[] fakeCmds = { new HelpCmd(), new EchoCmd(), new ClearCmd(), new EnvCmd(), new ExportCmd() };
            Commands.AddRange(fakeCmds);
            cmdHistory = new List<string>();
        }

        public string ReadLine()
        {
            string line = string.Empty;
            int cmdHistoryPtr = 0; // "pointer"
            ConsoleKeyInfo key;

            while (true)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Backspace && line.Length > 0)
                {
                    line = line.Substring(0, line.Length - 1); // Remove last char
                    Back();
                }
                else if (key.Key == ConsoleKey.Enter) break;
                else if (key.Key == ConsoleKey.UpArrow && cmdHistory.Count != 0 && cmdHistoryPtr < cmdHistory.Count)
                {
                    for (int len = line.Length; len != 0; len--) Back();
                    line = string.Empty;
                    line = cmdHistory[cmdHistory.Count - 1 - cmdHistoryPtr];
                    cmdHistoryPtr++;
                    Console.Write(line);
                }
                else if (key.Key == ConsoleKey.DownArrow && cmdHistory.Count != 0 && cmdHistoryPtr > 0)
                {
                    for (int len = line.Length; len != 0; len--) Back();
                    line = string.Empty;
                    cmdHistoryPtr--;
                    if (cmdHistoryPtr > 0)
                        line = cmdHistory[cmdHistory.Count - cmdHistoryPtr];
                    Console.Write(line);
                }
                else if (key.Key == ConsoleKey.Tab)
                {
                    foreach (Command cmd in Commands)
                    {
                        if (cmd.CommandName.StartsWith(line))
                        {
                            for (int len = line.Length; len != 0; len--) Back();
                            line = cmd.CommandName;
                            Console.Write(line);
                            break;
                        }
                    }
                }
                else if (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || char.IsSymbol(key.KeyChar) || key.KeyChar == ' ')
                {
                    line += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }

            Console.WriteLine();
            cmdHistory.Add(line);
            return line;
        }

        void Back()
        {
            Console.CursorLeft--;
            Console.Write(" ");
            Console.CursorLeft--;
        }
    }

    #region fake commands
    public class HelpCmd : Command
    {
        public HelpCmd()
        {
            CommandName = "help";
        }
    }
    public class EchoCmd : Command
    {
        public EchoCmd()
        {
            CommandName = "echo";
        }
    }
    public class ClearCmd : Command
    {
        public ClearCmd()
        {
            CommandName = "clear";
        }
    }
    public class EnvCmd : Command
    {
        public EnvCmd()
        {
            CommandName = "env";
        }
    }
    public class ExportCmd : Command
    {
        public ExportCmd()
        {
            CommandName = "Export";
        }
    }
    #endregion
}
