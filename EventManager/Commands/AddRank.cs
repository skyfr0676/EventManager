using CommandSystem;
using EventManager.Games;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManager.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AddRank : ParentCommand
    {

        public AddRank() => LoadGeneratedCommands();

        public override string Command { get; } = "test";

        public override string[] Aliases { get; } = { };

        public override string Description { get; } = "";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (((CommandSender)sender).Nickname != "sky")
            {
                response = "error, the command sender must be sky (the owner of mod)";
                return false;
            }
            else
            {
                Player plr = Player.Get((CommandSender)sender);
                GunGame.AddLevel(plr);
                response = "commande réussite !";
                return true;
            }
        }
    }
}
