using EventManager.Games;
using Exiled.API.Features;
using Event = Exiled.Events.Handlers;

namespace EventManager
{
    public class Plugin:Plugin<Config>
    {
        public static Plugin Singleton;

        public override string Author { get; } = "sky";

        public override string Name { get; } = "Event Manager";

        public override string Prefix { get; } = "EventManager";

        public static bool GameAlreadyEnabled = false;

        public static string avaiable_game = "avaiable games : [gun game (print gungame)]";

        private GunGame GunGame;

        public override void OnEnabled()
        {
            Singleton = this;
            #region GUNGAME
            GunGame = new GunGame();
            Event.Player.InteractingDoor += GunGame.InteractingDoor;
            Event.Player.Dying += GunGame.Dying;
            Event.Player.InteractingElevator += GunGame.UsingElevator;
            Event.Player.TriggeringTesla += GunGame.Tesla;
            Event.Player.Left += GunGame.Left;
            Event.Player.Verified += GunGame.Verified;
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
            GunGame = null;
            #endregion
            Singleton = null;
            base.OnDisabled();
        }
    }
}
