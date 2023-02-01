using CommandSystem;
using EventManagerRework.Features.Enums;
using EventManagerRework.Features.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagerRework.Features.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler)), CommandHandler(typeof(GameConsoleCommandHandler))]
    public class LaunchCommand : ICommand, IUsageProvider
    {
        public string[] Usage { get; } = new string[1] { "name of the event" };

        public string Command { get; } = "launch";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "permet de lancer un event.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 1)
            {
                response = "args";
                return false;
            }
            if (!Enum.TryParse(arguments.At(0), out EventType e))
            {
                response = "list of events possible : \nGunGame";
                return false;
            }
            if (!e.Launch(out string reponse))
            {
                response = "an error has occured with lauching event GunGame : " + reponse;
                return false;
            }
            response = "event launched " + e + ".";
            return true;
        }
    }
}
