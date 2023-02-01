using EventManagerRework.Events.GunGame;
using Exiled.API.Features;
using Server = Exiled.Events.Handlers.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MEC;
using EventManagerRework.Events.Vip;
using Exiled.API.Enums;

namespace EventManagerRework
{
    public class Plugin : Plugin<Config>
    {
        public override string Name { get; } = "EventManagerRework";
        public override string Prefix { get; } = "EventMRework";
        public override string Author { get; } = "sky";
        public override Version Version { get; } = new Version(1,0,0);
        public override PluginPriority Priority { get; } = PluginPriority.First;

        public static Config StaticConfig;
        public static bool AnotherGameHasAlreadyStarted = false;
        
        public override void OnEnabled()
        {
            StaticConfig = Config;

            GunGame.Coroutine = Timing.RunCoroutine(GunGame.CheckLeaderPlayer());
            GunGame.Register();
            
            Server.WaitingForPlayers += WFP;
            
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            GunGame.UnRegister();
            Server.WaitingForPlayers -= WFP;
            StaticConfig = null;
            base.OnDisabled();
        }

        public void WFP()
        {
            AnotherGameHasAlreadyStarted = false;
            GunGame.IsStarted = false;
            Vip.IsStarted = false;
        }
    }
}
