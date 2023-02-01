using EventManagerRework.Events.GunGame;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.CreditTags.Features;
using MapGeneration;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EventManagerRework.Features.Extensions.GunGameExtensions
{
    public static class PlayerExtensions
    {
        public static void GrantLevel(this Player plr)
        {
            int actualRank = GunGame.PlayersRank.ContainsKey(plr) ? GunGame.PlayersRank[plr] : -1;
            if (actualRank != -1)
            {
                GunGame.PlayersRank[plr]++;
                if (Plugin.StaticConfig.GunGameConfig.ItemRank.Length < GunGame.PlayersRank[plr])
                {
                    GunGame.FinishRound(plr);
                    return;
                }
                plr.ShowHint(Plugin.StaticConfig.GunGameConfig.GrantLevelMessage, Plugin.StaticConfig.GunGameConfig.RespawnTime);
                GrantItems(plr);
            }
        }

        public static void GrantItems(this Player plr)
        {
            int rank = GunGame.PlayersRank[plr];
            ItemType type = Plugin.StaticConfig.GunGameConfig.ItemRank[rank - 1];
            List<ItemType> items = new()
            {
                type,
                ItemType.Adrenaline,
                ItemType.Medkit,
            };
            AmmoType ammo = type.IsWeapon(false, false) ? type.GetFirearmType().GetWeaponAmmoType() : AmmoType.None;
            plr.Ammo.Clear();
            if (ammo is not AmmoType.None)
                plr.AddAmmo(ammo, (ushort)plr.GetAmmoLimit(ammo));
            plr.ResetInventory(items);
        }

        public static IEnumerator<float> Respawning(this Player plr)
        {
            for (int i = (int)Plugin.StaticConfig.GunGameConfig.RespawnTime; i > 0; i--)
            {
                string replace = Plugin.StaticConfig.GunGameConfig.RespawnMessage.Replace("{time}", i.ToString());
                plr.ShowHint(replace, 1.2f);
                yield return Timing.WaitForSeconds(1f);
            }
            plr.Respawn();
        }
        public static void Respawn(this Player plr)
        {
            plr.Role.Set(RoleTypeId.ClassD);
            Vector3 spawnLocation = plr.GetSpawn(Plugin.StaticConfig.GunGameConfig.ZoneSpawn);
            if (spawnLocation != Vector3.zero)
                plr.Position = plr.GetSpawn(Plugin.StaticConfig.GunGameConfig.ZoneSpawn);
            plr.GrantItems();
        }
        public static Vector3 GetSpawn(this Player _, ZoneType zone)
        {
            if (Plugin.StaticConfig.GunGameConfig.isCustomSpawn)
            {
                if (Plugin.StaticConfig.GunGameConfig.CustomSpawnLocation == new Vector3(0, 0, 0))
                    return Vector3.zero;
                else
                    return Plugin.StaticConfig.GunGameConfig.CustomSpawnLocation;
            }
            if (zone == ZoneType.Other || zone == ZoneType.Unspecified)
            {
                List<Room> rooms = new();
                rooms.AddRange(Room.Get(ZoneType.Entrance));
                rooms.AddRange(Room.Get(ZoneType.HeavyContainment));
                while (true)
                {
                    int random = Random.Range(0, rooms.Count);
                    Room room = rooms[random];
                    if (IsIllegalSpawn(room.Type))
                        continue;
                    return room.Position + new Vector3(0, 1, 0); 
                }
            }
            while (true)
            {
                Room room = Room.Random(zone);
                if (IsIllegalSpawn(room.Type))
                    continue;
                return room.Position + new Vector3(0, 1, 0);
            }
        }
        public static bool IsIllegalSpawn(RoomType room)
        {
            if (room == RoomType.LczCheckpointA || room == RoomType.LczCheckpointB || room == RoomType.HczEzCheckpointA || room == RoomType.HczEzCheckpointB || room == RoomType.HczArmory || room == RoomType.Hcz096 || room == RoomType.HczTestRoom || room == RoomType.Lcz914 || room == RoomType.LczArmory || room == RoomType.EzIntercom || room == RoomType.EzGateA || room == RoomType.EzGateB || room == RoomType.EzCheckpointHallway || room == RoomType.Pocket)
                return true;
            return false;
        }
    }
}
