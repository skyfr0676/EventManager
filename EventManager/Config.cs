using Exiled.API.Enums;
using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace EventManager
{
    public class Config:IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        [Description("the areas where the fight takes place (Surface,Entrance, HeavyContainment and LightContainment)")]
        public ZoneType GunGameZone { get; set; } = ZoneType.HeavyContainment;

        [Description("If CustomSpawn = false, The configuration \"Zone\" will be used")]
        public bool GunGameCustomSpawn { get; set; } = false;

        [Description("used only if CustomSpawn = true. Teleport the player to the spawn. useful if you have custom Map")]
        public Vector3 GunGameCustomSpawnLocation { get; set; } = new Vector3(0, 1, 0);

        [Description("When the game was started and a player went to open a keycard door.")]
        public string GunGameDoorunauthorized { get; set; } = "<color=red><size=150%><b>You can't use checkpoint or armory in gun game mode !</b></size></color>";

        [Description("When the game was started and a player went to call elevator")]
        public string GunGameElevunauthorized { get; set; } = "<color=red><size=150%><b>You can't use elevators in gun game mode !</b></size></color>";

        [Description("the message displayed for all losers person. ({winner} = the player who win the game).")]
        public string GunGameWinnerMsgForLost { get; set; } = "<color=yellow>{winner} has the winner of the game !</color>";

        [Description("the message displayed for the winner when the game was finished")]
        public string GunGameWinnerMsgForWinner { get; set; } = "you are the winner of the game !";

        [Description("the time the message is displayed")]
        public ushort GunGameWinnerDuration { get; set; } = 10;

        [Description("when the player die and time before respawning")]
        public float GunGameTimeBeforeRespawning { get; set; } = 5f;

        [Description("when a player was dead. {time} = gun_game_time_before_respawning")]
        public string GunGameDeadMsg { get; set; } = "you are dead ! you will be respawn in {time} seconds !";

        [Description("when the game was started and the player join later. ({time} = gun_game_time_before_respawning)")]
        public string GunGameLateJoinMsg { get; set; } = "hello ! you will be spawn in {time} secondes ! good luck !";

        [Description("when the player kill a other player")]
        public string GunGameNextLevel { get; set; } = "<color=yellow>you grant a level !</color>";

        [Description("when the player kill himself")]
        public string GunGameLostLevel { get; set; } = "you lost level because you kill you";

        [Description("the player will not die as long as the spawn protect is active")]
        public float GunGameSpawnProtectTime { get; set; } = 5f;

        [Description("the message who all players displayed. equivalent to spawn protect message time. ({time} = gun_game_spawn_protect_time)")]
        public string GunGameStartMsg { get; set; } = "the game start in {time} seconds !";

        //public List<ItemType> GunGameOrder { get; set; } = new List<ItemType>(){};
    }
}
