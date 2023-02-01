using EventManagerRework.Events.Vip;
using EventManagerRework.Features.Added.Vip;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.CustomItems.API;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using Team = EventManagerRework.Features.Added.Vip.Team;

namespace EventManagerRework.Features.Extensions.VipExtensions
{
    public static class PlayerExtensions
    {
        public static void Spawn(this Player plr)
        {
            Team team = plr.GetTeam();
            plr.Role.Set(team.Role);
            Vector3 spawn = team.Role.GetSpawn();
            plr.Teleport(spawn);
        }
        public static Team GetTeam(this Player plr)
        {
            if (Vip.PlayersTeam.ContainsKey(plr))
            {
                return Vip.PlayersTeam[plr];
            }
            else
            {
                return plr.GetRandomTeam();
            }
        }
        public static Team GetRandomTeam(this Player plr)
        {
            if (Vip.ClassDs.Count > Vip.Scientists.Count)
            {
                Vip.Scientists.Add(plr);
                Team team = new Team(plr, RoleTypeId.Scientist, false);
                Vip.PlayersTeam.Add(plr, team);
                plr.ShowHint("you are Scientist !");
                return team;
            }
            else if (Vip.Scientists.Count > Vip.ClassDs.Count)
            {
                Vip.ClassDs.Add(plr);
                Team team = new Team(plr, RoleTypeId.ClassD, false);
                Vip.PlayersTeam.Add(plr, team);
                plr.ShowHint("you are Dboy !");
                return team;
            }
            else
            {
                int d = Random.Range(0, 101);
                if (d switch
                {
                    <= 50 => true,
                    _ => false,
                })
                {
                    Vip.ClassDs.Add(plr);
                    Team team = new Team(plr, RoleTypeId.ClassD, false);
                    Vip.PlayersTeam.Add(plr, team);
                    plr.ShowHint("you are Dboy !");
                    return team;
                }
                else
                {
                    Vip.Scientists.Add(plr);
                    Team team = new Team(plr, RoleTypeId.Scientist, false);
                    Vip.PlayersTeam.Add(plr, team);
                    plr.ShowHint("you are Scientist !");
                    return team;
                }
            }
        }
        public static Vector3 GetSpawn(this RoleTypeId role)
        {
            ZoneType zone = ZoneType.HeavyContainment;
            Room room;
            while (true)
            {
                room = Room.Random(zone);
                if (GunGameExtensions.PlayerExtensions.IsIllegalSpawn(room.Type))
                    continue;
                else
                    return room.Position + new Vector3(0, 1, 0);
            }
        }
        public static Team GetRandomVip(this RoleTypeId role)
        {
            if (role == RoleTypeId.ClassD)
            {
                int random = Random.Range(0, Vip.ClassDs.Count);
                Player plr  = Vip.ClassDs[random];
                return plr.SetVip(RoleTypeId.ClassD);
            }
            else
            {
                int random = Random.Range(0, Vip.Scientists.Count);
                Player plr = Vip.Scientists[random];
                return plr.SetVip(RoleTypeId.Scientist);
            }
        }

        public static Team SetVip(this Player plr, RoleTypeId vip)
        {
            if (vip == RoleTypeId.ClassD)
            {
                Vip.PlayersTeam[plr].IsVip = true;
                Vip.ClassDVip = plr;
                plr.Role.Set(RoleTypeId.ChaosConscript, SpawnReason.Revived, RoleSpawnFlags.None);
                plr.GrantItems();
                return Vip.PlayersTeam[plr];
            }
            else
            {
                Vip.PlayersTeam[plr].IsVip = true;
                Vip.ScientistVip = plr;
                plr.Role.Set(RoleTypeId.NtfSpecialist, SpawnReason.Revived, RoleSpawnFlags.None);
                plr.GrantItems();
                return Vip.PlayersTeam[plr];
            }
        }

        public static void GrantItems(this Player plr)
        {
            if (!Vip.PlayersTeam.ContainsKey(plr))
                return;
            plr.MaxHealth = 50;
            plr.Health = 50;
            plr.AddAhp(0, 75, 1000, 0, 0, false);
            Team team = Vip.PlayersTeam[plr];
            if (team.IsVip)
            {
                plr.MaxHealth = 100;
                plr.Health = 100;
                plr.AddAhp(75, decay:0, efficacy:1, persistant:true);
                List<ItemType> items = new()
                {
                    ItemType.GunShotgun,
                    ItemType.Medkit,
                    ItemType.Adrenaline,
                };
                plr.AddAmmo(AmmoType.Ammo12Gauge, (ushort)plr.GetAmmoLimit(AmmoType.Ammo12Gauge));
                plr.ResetInventory(items);
            }
            else
            {
                List<ItemType> items = new()
                {
                    ItemType.Jailbird,
                    ItemType.Medkit,
                    ItemType.Adrenaline,
                };
                plr.ResetInventory(items);
            }
        }
    }
}
