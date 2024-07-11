namespace moss
{
    /// <summary>
    /// The shell itself. Initialize it and call the Run method.
    /// </summary>
    public class Shell
    {
        List<Command> Commands;

        public Dictionary<string, string> EnvironmentVariables;
        string HelpString;

        Input input;

        /// <summary>
        /// Initalize a new shell instance.
        /// </summary>
        /// <param name="cmds">An array for all of your OS's commands. You do NOT need a `help`, `echo`, `clear`, `env`, OR `export` command.</param>
        /// <param name="environmentVariables">The environment variables like `PROMPT`, etc...</param>
        /// <param name="user">The user the shell will run as. `root` is the highest privileged one.</param>
        /// <param name="startPath">The FS path that the shell will start from. CANNOT BE A UNIX PATH. For example, `0:\` would be a good start path.</param>
        /// <param name="fakeUnixPath">Set this to true if you want to use / instead of 0:\.</param>
        public Shell(Command[] cmds, Dictionary<string,string>? environmentVariables, string user, string startPath)
        {
            // Commands
            Commands = new List<Command>(cmds);

            // Input
            input = new Input(Commands);

            // Environment variables
            if (environmentVariables != null)
                EnvironmentVariables = environmentVariables;
            else
                EnvironmentVariables = new Dictionary<string, string>();

            #region Environment variables setup
            // Shell
            if (EnvironmentVariables.ContainsKey("SHELL"))
                EnvironmentVariables.Remove("SHELL");

            EnvironmentVariables.Add("SHELL", "moss");

            // User
            if (EnvironmentVariables.ContainsKey("USER"))
                EnvironmentVariables.Remove("USER");

            EnvironmentVariables.Add("USER", user);

            // Working directory
            if (EnvironmentVariables.ContainsKey("PWD"))
                EnvironmentVariables.Remove("PWD");

            EnvironmentVariables.Add("PWD", startPath);

            // Last Return Code
            if (EnvironmentVariables.ContainsKey("RET"))
                EnvironmentVariables.Remove("RET");

            EnvironmentVariables.Add("RET", "0");
            #endregion

            GenerateHelpString();
        }

        void GenerateHelpString()
        {
            HelpString = "help - Shows this help menu.\nenv - Lists all environment variables.\nexport [\"var=val\"] - Turns the env value of `var` into `val`.\nclear - Clears the screen.\necho [str] - Echoes back the arguments. ANSI & formatting does apply.\n";
            foreach (Command cmd in Commands)
            {
                HelpString += cmd.CommandName;

                if (cmd.Options.Length == 1)
                    HelpString += " [" + cmd.Options[0] + "]";
                else if (cmd.Options.Length > 1)
                {
                    HelpString += " [";
                    foreach (string arg in cmd.Options)
                        HelpString += arg + "|";
                    HelpString += "]";
                }

                HelpString += " - " + cmd.Description + "\n";
            }
        }

        public void Run()
        {
            Moss.PrintWithANSI(Prompt.GetPrompt(EnvironmentVariables));
            string comd = input.ReadLine();
            string[] words = Moss.CombineQuotedStrings(comd.Split(' '));
            #region Built-in commands
            switch (words[0])
            {
                case "help":
                    if (EnvironmentVariables.ContainsKey("RET"))
                        EnvironmentVariables.Remove("RET");
                    try
                    {
                        Console.Write(HelpString);
                        EnvironmentVariables.Add("RET", "0");
                    }
                    catch (Exception e)
                    {
                        Moss.PrintWithANSI("help: \\033[91mexception\\033[0m: " + e.Message + "\n");
                        EnvironmentVariables.Add("RET", "1");
                    }
                    break;
                case "env":
                    try
                    {
                        EnvCmd();
                        if (EnvironmentVariables.ContainsKey("RET"))
                            EnvironmentVariables.Remove("RET");
                        EnvironmentVariables.Add("RET", "0");
                    }
                    catch (Exception e)
                    {
                        Moss.PrintWithANSI("env: \\033[91mexception\\033[0m: " + e.Message + "\n");
                        if (EnvironmentVariables.ContainsKey("RET"))
                            EnvironmentVariables.Remove("RET"); 
                        EnvironmentVariables.Add("RET", "1");
                    }
                    break;
                case "export":
                    try
                    {
                        ExportCmd(Moss.CombineQuotedStrings(words.Skip(1).ToArray()));
                        if (EnvironmentVariables.ContainsKey("RET"))
                            EnvironmentVariables.Remove("RET");
                        EnvironmentVariables.Add("RET", "0");
                    }
                    catch (Exception e)
                    {
                        Moss.PrintWithANSI("export: \\033[91mexception\\033[0m: " + e.Message + "\n");
                        if (EnvironmentVariables.ContainsKey("RET"))
                            EnvironmentVariables.Remove("RET");
                        EnvironmentVariables.Add("RET", "1");
                    }
                    break;
                case "clear":
                    if (EnvironmentVariables.ContainsKey("RET"))
                        EnvironmentVariables.Remove("RET");
                    try
                    {
                        Console.Clear();
                        EnvironmentVariables.Add("RET", "0");
                    }
                    catch (Exception e)
                    {
                        Moss.PrintWithANSI("clear: \\033[91mexception\\033[0m: " + e.Message + "\n");
                        EnvironmentVariables.Add("RET", "1");
                    }
                    break;
                case "echo":
                    try
                    {
                        Moss.PrintWithANSI(Prompt.GetActualString(string.Join(' ', words.Skip(1).ToArray()), EnvironmentVariables));
                        Console.WriteLine();
                        EnvironmentVariables["RET"] = "0";
                    }
                    catch (Exception e)
                    {
                        Moss.PrintWithANSI("echo: \\033[91mexception\\033[0m: " + e.Message + "\n");
                        EnvironmentVariables["RET"] = "1";
                    }
                    break;
                #endregion
                default:
                    bool cmdFound = false;
                    foreach (Command cmd in Commands)
                    {
                        if (words[0] == cmd.CommandName)
                        {
                            cmdFound = true;
                            if (EnvironmentVariables.ContainsKey("RET"))
                                EnvironmentVariables.Remove("RET");

                            try
                            {
                                cmd.Action(EnvironmentVariables["PWD"], Moss.CombineQuotedStrings(words.Skip(1).ToArray()), EnvironmentVariables);
                                EnvironmentVariables.Add("RET", "0");
                            }
                            catch (Exception e)
                            {
                                Moss.PrintWithANSI(cmd.CommandName + ": \\033[91mexception\\033[0m: " + e.Message + "\n");
                                EnvironmentVariables.Add("RET", "1");
                            }
                            break;
                        }
                    }
                    if (!cmdFound)
                    {
                        Moss.PrintWithANSI("\\033[91m-moss: " + words[0] + ": command not found\\033[0m\n");
                    }
                    break;
            }
        }

        void EnvCmd()
        {
            foreach (string key in EnvironmentVariables.Keys)
                Console.WriteLine(key + "=" + EnvironmentVariables[key]);
        }

        void ExportCmd(string[] param)
        {
            if (param.Length != 1)
                throw new Exception("Expected valid number of arguments. \"var=val\".");

            string[] split = param[0].Split(new[] { '=' }, 2); // Split only on the first '='
            if (split.Length != 2)
                throw new Exception("Invalid format. Expected \"var=val\".");

            string key = split[0];
            string value = split[1];

            if (EnvironmentVariables.ContainsKey(key))
                EnvironmentVariables.Remove(key);
            EnvironmentVariables.Add(key, value);
        }
    }
}
