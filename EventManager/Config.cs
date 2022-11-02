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

        public string GunGameDoorunauthorized { get; set; } = "<color=red><size=150%><b>You can't use checkpoint or armory in gun game mode !</b></size></color>";

        public string GunGameElevunauthorized { get; set; } = "<color=red><size=150%><b>You can't use elevators in gun game mode !</b></size></color>";

        [Description("DON'T DELETE {winner} PLEASE !!!")]
        public string GunGameWinnerMsgForLost { get; set; } = "<color=yellow>{winner} has the winner of the game !</color>";

        public string GunGameWinnerMsgForWinner { get; set; } = "you are the winner of the game !";

        [Description("the time the message is displayed")]
        public ushort GunGameWinnerDuration { get; set; } = 10;

        public float GunGameTimeBeforeRespawning { get; set; } = 5f;

        public string GunGameDeadMsg { get; set; } = "you are dead ! you will be respawn in {time} seconds !";

        public string GunGameLateJoinMsg { get; set; } = "hello ! you will be spawn in {time} secondes ! good luck !";

        public string GunGameNextLevel { get; set; } = "<color=yellow>you grant a level !</color>";

        public string GunGameLostLevel { get; set; } = "you lost level because you kill you";

        public float GunGameSpawnProtectTime { get; set; } = 5f;

        public string GunGameStartMsg { get; set; } = "the game start in {time} seconds !";

        //public List<ItemType> GunGameOrder { get; set; } = new List<ItemType>(){};
    }
}
