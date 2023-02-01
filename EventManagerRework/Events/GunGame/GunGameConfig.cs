using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EventManagerRework.Events.GunGame
{
    public class GunGameConfig
    {
        [Description("all items you need to give to player.")]
        public ItemType[] ItemRank { get; set; } =
        {
            ItemType.ParticleDisruptor,
            ItemType.GunAK,
            ItemType.GunE11SR,
            ItemType.GunShotgun,
            ItemType.GunCOM15,
            ItemType.GunRevolver,
            ItemType.GunCOM18,
            ItemType.GunCrossvec,
            ItemType.GunCOM18,
            ItemType.GunRevolver,
            ItemType.GunFSP9,
            ItemType.GunLogicer,
        };

        [Description("the respawn message.")]
        public string RespawnMessage { get; set; } = "you will be respawn in {time} seconds !";
        [Description("the respaw time.")]
        public float RespawnTime { get; set; } = 5;
        [Description("when a player kill other player.")]
        public string GrantLevelMessage { get; set; } = "<color=yellow>You grant a level !</color>";
        [Description("this message is for player who win.")]
        public string WinMessage { get; set; } = "you win !";
        [Description("this message is for player who loose.")]
        public string LooseMessage { get; set; } = "{winner} has win the game !";
        [Description("Zone who player spawn (types : HeavyContainment, Entrance,LightContainment, Surface) (use Other or Unspecified to use Entrance and Heavy containment zone).")]
        public ZoneType ZoneSpawn { get; set; } = ZoneType.HeavyContainment;
        [Description("when a player has designated, broadcast this to all players.")]
        public string LeaderMessage { get; set; } = "Leader: {player} with {point} point.";
        [Description("when Leader has not designated yet (designated has 3 points).")]
        public string NoLeaderMessage { get; set; } = "<color=red>Leader: No player yet.</color>";
        [Description("when gungame mode was started and player trying to drop item.")]
        public string NoDropMessage { get; set; } = "<color=red>you cannot drop items in GunGame mode :/</color>";
        [Description("when gungame mode was started and player trying to pickup item.")]
        public string NoPickupMessage { get; set; } = "<color=red>you cannot pickup items in GunGame mode :/</color>";

        [Description("if you have custom spawn, you can use this with writting true. else, write false.")]
        public bool isCustomSpawn { get; set; } = false;
        [Description("use x:0, y:0 and z:0 to use your own custom spawn location.")]
        public Vector3 CustomSpawnLocation { get; set; } = new Vector3(0, 0, 0);
    }
}
