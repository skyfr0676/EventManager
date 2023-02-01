using CommandSystem;
using EventManagerRework.Features.Extensions.GunGameExtensions;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagerRework.Features.Commands.GunGame
{
    [CommandHandler(typeof(RemoteAdminCommandHandler)), CommandHandler(typeof(GameConsoleCommandHandler))]
    public class GrantCommand : ICommand, IUsageProvider
    {
        public string[] Usage { get; } = new string[1] {"id/name"};

        public string Command { get; } = "grant";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "permet de faire gagner un niveau a un joueur";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 1)
            {
                response = "args";
                return false;
            } 
            if (!sender.CheckPermission(PlayerPermissions.PermissionsManagement))
            {
                response = "you doesn't have specified permissions => PermissionsManagement.";
                return false;
            }
            if (!Player.TryGet(arguments.At(0), out Player plr))
            {
                response = "joueur non trouvé.";
                return false;
            }
            plr.GrantLevel();
            response = "player " + plr.Nickname + " grant a level.";
            return true;
        }
    }
}
