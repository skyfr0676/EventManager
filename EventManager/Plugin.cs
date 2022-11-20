using EventManager.Games;
using Exiled.API.Features;
using System;
using Event = Exiled.Events.Handlers;

namespace EventManager
{
    public class Plugin:Plugin<Config>
    {
        public static Plugin singleton;

        public override string Author { get; } = "sky";
        public override string Name { get; } = "Event Manager";
        public override string Prefix { get; } = "EventManager";
        public override Version Version { get; } = new Version(0,1,0);
        public override Version RequiredExiledVersion { get; } = new Version(5,3,0);

        public static bool AnotherGamerHasEnabled = false;

        public static string avaiable_game = "avaiable games : [gun game (print \"gungame\")]";

        private GunGame GunGame;
        private Vip VipGame;

        public override void OnEnabled()
        {
            AnotherGamerHasEnabled = false;
            singleton = this;
            #region GUNGAME
            GunGame = new GunGame();
            GunGame.Init();
            GunGame.IsStarted = false;
            Event.Player.InteractingDoor += GunGame.InteractingDoor;
            Event.Player.Dying += GunGame.Dying;
            Event.Player.InteractingElevator += GunGame.UsingElevator;
            Event.Player.TriggeringTesla += GunGame.Tesla;
            Event.Player.Left += GunGame.Left;
            Event.Player.Verified += GunGame.Verified;
            Event.Player.PickingUpItem += GunGame.Pickup;
            Event.Player.PickingUpAmmo += GunGame.Pickup;
            Event.Player.PickingUpArmor += GunGame.Pickup;
            Event.Player.DroppingAmmo += GunGame.Dropping;
            Event.Player.DroppingItem += GunGame.Dropping;
            Event.Player.ThrowingItem += GunGame.Throwing;
            Event.Server.RestartingRound += GunGame.RoundRestart;
            Event.Server.ReloadedConfigs += GunGame.ReloadedConfigs;
            #endregion
            #region VIP
            VipGame = new Vip();
            Event.Player.Dying += VipGame.Dead;
            #endregion
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            #region GUNGAME
            Event.Player.InteractingDoor -= GunGame.InteractingDoor;
            Event.Player.Dying -= GunGame.Dying;
            Event.Player.InteractingElevator -= GunGame.UsingElevator;
            Event.Player.TriggeringTesla -= GunGame.Tesla;
            Event.Player.Left -= GunGame.Left;
            Event.Player.Verified -= GunGame.Verified;
            Event.Player.PickingUpItem += GunGame.Pickup;
            Event.Player.PickingUpAmmo += GunGame.Pickup;
            Event.Player.PickingUpArmor += GunGame.Pickup;
            Event.Player.DroppingAmmo += GunGame.Dropping;
            Event.Player.DroppingItem += GunGame.Dropping;
            Event.Player.ThrowingItem -= GunGame.Throwing;
            Event.Server.RestartingRound -= GunGame.RoundRestart;
            Event.Server.ReloadedConfigs-= GunGame.ReloadedConfigs;
            GunGame = null;
            #endregion
            #region VIP
            Event.Player.Dying -= VipGame.Dead;
            VipGame = null;
            #endregion
            singleton = null;
            base.OnDisabled();
        }

        public override void OnReloaded()
        {
            GunGame.Init();
            base.OnReloaded();
        }
    }
}
