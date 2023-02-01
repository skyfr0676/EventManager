using Team = EventManagerRework.Features.Added.Vip.Team;
using EventManagerRework.Features.Extensions.VipExtensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Event = Exiled.Events.Handlers;
using Exiled.Events.EventArgs.Item;

namespace EventManagerRework.Events.Vip
{
    public class Vip
    {
        public static Dictionary<Player, Team> PlayersTeam = new();

        public static Player ScientistVip
        {
            get => scientistvip;
            set
            {
                scientistvip = value;
                foreach (Player plr in Scientists)
                {
                    plr.ShowHint($"{value.Nickname} is your new vip !");
                }
            }
        }
        private static Player classdvip;
        private static Player scientistvip;
        public static Player ClassDVip
        {
            get => classdvip;
            set
            {
                classdvip = value;
                foreach (Player plr in ClassDs)
                {
                    plr.ShowHint($"{value.Nickname} is your new vip !");
                }
            }
        }

        public static List<Player> ClassDs = new();

        public static List<Player> Scientists = new();

        public static bool IsStarted = false;
        public static bool DefaultLock;
        public static bool StartingInProgress;

        public static string Start()
        {
            if (Round.IsStarted || IsStarted || Plugin.AnotherGameHasAlreadyStarted)
                return "error, the GunGame has already started, or the round has already sstarted or an another event has already started.";
            Scientists.Clear();
            ClassDs.Clear();
            ClassDVip = null;
            ScientistVip = null;
            IsStarted = true;
            StartingInProgress = true;
            DefaultLock = Round.IsLocked;
            Round.IsLocked = true;
            Round.Start();
            Timing.CallDelayed(2f, () => StartingInProgress = false);
            return "game launched !";
        }
        public static void ChangeRole(ChangingRoleEventArgs ev)
        {
            if (!StartingInProgress)
                return;
            ev.NewRole = RoleTypeId.Tutorial;
            ev.Ammo.Clear();
            ev.Items.Clear();
            ev.Player.ShowHint("launching game <color=yellow>Vip</color>...",int.MaxValue);
            ev.Player.Spawn();
        }

        public static void ChargingJailBird(ChargingJailbirdEventArgs ev)
        {
            if (!ev.IsAllowed) return;
            if (!IsStarted) return;
            ev.IsAllowed = false;
            ev.Player.ShowHint("you cannot charging jailbird in Vip mode :/");
        }

        public static void Register()
        {
            Event.Player.ChangingRole += ChangeRole;
            Event.Item.ChargingJailbird += ChargingJailBird;
        }

        public static void UnRegister()
        {
            Event.Player.ChangingRole -= ChangeRole;
            Event.Item.ChargingJailbird -= ChargingJailBird;
        }
    }
}
