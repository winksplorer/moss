# moss

A command line shell for Cosmos operating systems.

### Features

- Command history
- Tab completion
- Environment variables
- Bash-compatible prompt system (kinda)
- ANSI compatibility (only for text colors)
- Quoted strings are grouped together
- Easy to make commands for

### Using it

On how to add moss to your project, google up "Visual Studio add DLL reference to C# project".

A very, very, VERY basic example of using moss is just this:
```cs
using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using moss;

namespace mossTestOS
{
    public class Kernel : Sys.Kernel
    {
        Shell shell;

        protected override void BeforeRun()
        {
            Command[] cmds = { };
            shell = new(cmds, new Dictionary<string, string>(), "root", "0:\\");
        }

        protected override void Run() 
        {
            shell.Run();
        }
    }
}
```

A little bit better example for stuff like a cool prompt or a hello world command is this:
```cs
using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using moss;

namespace mossTestOS
{
    public class Kernel : Sys.Kernel
    {
        Shell shell;

        protected override void BeforeRun()
        {
            Command[] cmds = { new hello() };
            Dictionary<string,string> env = new Dictionary<string,string>();
            env.Add("PROMPT", "\\033[91m\\u [ \\033[97m\\w \\033[91m] \\$ \\033[0m"); // ANSI prompt
            shell = new(cmds, env, "root", "0:\\");
        }

        protected override void Run() 
        {
            shell.Run();
        }
    }

    public class hello : moss.Command // Hello world command
    {
        public hello()
        {
            CommandName = "hello";
            Description = "Prints \"Hello, world!\"";
        }

        public override void Action(string path, string[] param, Dictionary<string, string> environmentVariables)
        {
            Console.WriteLine("Hello, world!");
            return;
        }
    }
}
```

### Commands
This is the command template.
```cs
public class Command
{
    /// <summary>
    /// The name of your command.
    /// </summary>
    public string CommandName { get; set; }

    /// <summary>
    /// The description of your command.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// All the flags/arguments of your command. for example, just like ["-h", "-v"].
    /// </summary>
    public string[] Options { get; set; }

    /// <summary>
    /// If the command requires the user to be "root" to run it.
    /// </summary>
    public bool RequiresRoot { get; set; } = false;

    /// <summary>
    /// What your command does.
    /// </summary>
    /// <param name="path">The path that your program will run at.</param>
    /// <param name="param">The arguments and parameters to your program.</param>
    /// <param name="environmentVariables">Environment variables from the shell.</param>
    public virtual void Action(string path, string[] param, Dictionary<string, string> environmentVariables)
    {
        return;
    }
}
```

This is a hello world command.
```cs
public class hello : moss.Command
{
    public hello()
    {
        CommandName = "hello";
        Description = "Prints \"Hello, world!\"";
    }

    public override void Action(string path, string[] param, Dictionary<string, string> environmentVariables)
    {
        Console.WriteLine("Hello, world!");
        return;
    }
}
```

This is a command that throws an exception with a message of the arguments.
```cs
public class except : moss.Command
{
    public except()
    {
        CommandName = "except";
        Description = "Throws an exception with an error message of the arguments.";
    }

    public override void Action(string path, string[] param, Dictionary<string, string> environmentVariables)
    {
        if (param.Length == 0)
        {
            throw new Exception("manually created exception");
        } else
        {
            throw new Exception(string.Join(" ", param));
        }
    }
}
```