using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moss
{
    /// <summary>
    /// A command.
    /// </summary>
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
}
