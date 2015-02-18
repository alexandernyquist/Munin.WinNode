﻿namespace Munin.WinNode.Commands
{
    class UnknownCommand : ICommand
    {
        public string Command
        {
            get { return "unknown"; }
        }

        public bool EndResponseWithPeriod
        {
            get { return false; }
        }

        public void Execute(string[] arguments, out string response)
        {
            var commands = CommandManager.AllCommands();
            response = string.Format("# Unknown command. Try {0} or quit", string.Join(", ", commands));
        }
    }
}