using CommandSystem;
using EventManager.Enums;
using EventManager.Extensions;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.CustomItems.API;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs;
using Exiled.Permissions.Extensions;
using InventorySystem;
using MEC;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EventManager.Games
{
    public class Vip
    {
        public static bool IsStarted;

        public static Player _DBoyVip = null;
        public static Player _ScientistVip = null;

        private static List<Player> AlivePlayers = new List<Player>();
        private static List<Player> Players = new List<Player>();

        private static List<Player> DBoys = new List<Player>();
        private static List<Player> Scientists = new List<Player>();

        private static Player DBoyVip
        {
            get => _DBoyVip;
            set
            {
                _DBoyVip = value;
                value.PlayBeepSound();
                new AmbientSoundPlayer().RpcPlaySound(5);
                value.ShowHint("tu es le Dboy VIP ! tu a des meilleurs armes MAIS, si tu meure, tu meure (tu ne respawnera pas) ! bonne chance !");
            }
        }

        private static Player ScientistVip
        {
            get => _DBoyVip;
            set
            {
                _ScientistVip = value;
                value.PlayBeepSound();
                new AmbientSoundPlayer().RpcPlaySound(5);
                value.ShowHint("tu es le VIP des scientos ! tu a des meilleurs armes MAIS, si tu meure, tu meure (tu ne respawnera pas) ! bonne chance !");
            }
        }

        public static int VipDBoyKilled = 0;
        public static int VipScientistKilled = 0;

        public static void Start(Player Starter)
        {
            if (!Round.IsStarted || !IsStarted || !Plugin.AnotherGamerHasEnabled)
            {
                Server.FriendlyFire = false;
                Round.Start();
                Round.IsLocked = true;
                Log.Debug($"Vip mode has been started by {Starter.Nickname} !", Plugin.singleton.Config.Debug);
                LightContainmentZoneDecontamination.DecontaminationController.Singleton.disableDecontamination = true;
                Plugin.AnotherGamerHasEnabled = true;

                foreach (Player plr in Player.List)
                {
                    AlivePlayers.Add(plr);
                    Players.Add(plr);
                    plr.SetRole(RoleType.Tutorial, SpawnReason.RoundStart);
                    plr.ShowHint("starting game : <color=yellow>Vip Game</color>...", int.MaxValue);
                }

                Timing.CallDelayed(2f, () =>
                {
                    Suite();
                });
            }
            else
            {
                Starter.RemoteAdminMessage("error, the game or a another game or Round has already started.", false, "GAME CONTROLLER");
            }
        }

        public static void Suite()
        {
            foreach (Player plr in Players) {
                Log.Info(plr);
            }
            for (int i = 0; i < Players.Count / 2; i++)
            {
                Scientists.Add(Players[i]);
                Players.Remove(Players[i]);
                Respawn(Players[i]);
            }

            for (int i = 0; i < Players.Count / 2; i++)
            {
                DBoys.Add(Players[i]);
                Players.Remove(Players[i]);
                Respawn(Players[i]);
            }

        }

        public static void Respawn(Player plr)
        {
            bool IsDBoy = DBoys.Contains(plr);
            bool IsScientist = Scientists.Contains(plr);

            if (IsDBoy)
            {
                plr.SetRole(RoleType.ClassD, SpawnReason.Respawn);
                plr.ShowHint("you have been revived !");

                Timing.CallDelayed(2f, () =>
                {
                    Room room = Room.Random(ZoneType.HeavyContainment);
                    plr.Position = room.Position + new Vector3(0, 1, 0);
                    plr.AddItem(ItemType.GunCOM15);
                    plr.SetAmmo(AmmoType.Nato9, (ushort)plr.GetAmmoLimit(AmmoType.Nato9));
                    plr.Health = 50;
                    plr.RunningSpeed= 2;
                    plr.WalkingSpeed= 2;
                    plr.MaxHealth = 50;
                });
            }
            else if (IsScientist)
            {
                plr.SetRole(RoleType.Scientist, SpawnReason.Respawn);
                plr.ShowHint("you have been revived !");

                Timing.CallDelayed(2f, () =>
                {
                    plr.Respawn(typeof(Vip));
                    plr.AddItem(ItemType.GunCOM15);
                    plr.SetAmmo(AmmoType.Nato9, (ushort)plr.GetAmmoLimit(AmmoType.Nato9));
                    plr.Health = 50;
                    plr.RunningSpeed = 2;
                    plr.WalkingSpeed = 2;
                    plr.MaxHealth = 50;
                });
            }
        }

        public static void Win(VipWinner Winner)
        {
            if (Winner == VipWinner.ClassD)
            {
                foreach (Player DBoy in DBoys)
                    DBoy.ShowHint($"vous avez gagné ! (<color=#FF8E00>{VipScientistKilled}</color> VIP Kills/<color=#FFFF7C>{VipDBoyKilled}</color> VIP Kills)");
                foreach (Player loose in Scientists)
                    loose.ShowHint($"vous avez perdu :( (<color=#FF8E00>{VipScientistKilled}</color> VIP Kills/<color=#FFFF7C>{VipDBoyKilled}</color> VIP Kills)");
            }
            else if (Winner == VipWinner.Scientist)
            {
                foreach (Player Scientists in Scientists)
                    Scientists.ShowHint($"vous avez gagné ! (<color=#FFFF7C>{VipScientistKilled}</color> VIP Kills/<color=#FF8E00>{VipScientistKilled}</color> VIP Kills)");
                foreach (Player loose in Scientists)
                    loose.ShowHint($"vous avez perdu :( (<color=#FFFF7C>{VipDBoyKilled}</color> VIP Kills/<color=#FF8E00>{VipScientistKilled}</color> VIP Kills)");
            }
            IsStarted = false;
            Plugin.AnotherGamerHasEnabled = false;
            Round.IsLocked = false;
        }

        public static void ChooseDBoyVip()
        {
            if (DBoys.Count() == 0)
            {
                Win(VipWinner.Scientist);
                return;
            }

            int random = new System.Random().Next(DBoys.Count());
            Player plr = DBoys[random];
            plr.SetRole(RoleType.ChaosMarauder, SpawnReason.Escaped, true);
            plr.ClearInventory();
            DBoyVip = plr;
            plr.AddItem(ItemType.GunShotgun);
            plr.SetAmmo(AmmoType.Ammo12Gauge, (ushort)plr.GetAmmoLimit(AmmoType.Nato9));
            plr.RunningSpeed = 0.7f;
            plr.WalkingSpeed = 0.7f;
            plr.MaxHealth = 175;
            plr.Health = 175;
            foreach (Player DBoy in DBoys.Where(x=>x.UserId != plr.UserId))
                DBoy.ShowHint($"{plr.Nickname} is your new VIP !");
        }

        public void Hurting(HurtingEventArgs ev)
        {
            if (ev.Target == DBoyVip || ev.Target == ScientistVip || ev.Attacker == null)
                return;

            ev.Amount = 500;
            if (ev.Attacker.CurrentItem.Type == ItemType.GunCOM15)
            {
                ev.Attacker.ReloadWeapon();
            }
        }

        public void Shooting(ShootingEventArgs ev)
        {
            if (ev.Shooter == null) return;
            ev.Shooter.ReloadWeapon();
            Timing.CallDelayed(3f, () => { ev.Shooter.SetAmmo(AmmoType.Nato9, (ushort)ev.Shooter.GetAmmoLimit(AmmoType.Nato9)); });
        }

        public void Dead(DyingEventArgs ev)
        {
            if (ev.Killer != null)
            {
                ev.Killer.Heal(200);
                if (DBoyVip == ev.Target || ScientistVip == ev.Target)
                {
                    string DBoyorScientist = DBoyVip == ev.Target ? "DBoy" : "Scientist";
                    string KillColor = DBoyVip == ev.Target ? "FF8E00" : "FFFF7C";
                    DBoyVip = DBoyVip == ev.Target ? null : DBoyVip;
                    ScientistVip = ScientistVip == ev.Target ? null : ScientistVip;
                    ev.Killer.ShowHint($"you have killer {ev.Target.Nickname} how was the <color=#{KillColor}>{DBoyorScientist}</color> VIP !");
                    ev.Target.ShowHint("you are dead but you are a VIP donc you cannot respawn !");
                    if (DBoyorScientist == "DBoy")
                    {
                        DBoys.Remove(ev.Target);
                        ChooseDBoyVip();
                        VipDBoyKilled += 1;
                    }
                    else
                    {
                        VipScientistKilled += 1;
                        Scientists.Remove(ev.Target);
                    }
                }
                else
                {
                    string DBoyOrScientist = DBoys.Contains(ev.Killer) ? "Dboy" : "Scientist";
                    string KillColor = DBoys.Contains(ev.Killer) ? "FF8E00" : "FFFF7C";
                    ev.Target.ShowHint($"you have been killed by {ev.Killer.Nickname} how was a <color=#{KillColor}>{DBoyOrScientist}</color> :(");
                    Timing.CallDelayed(2f, () => { Respawn(ev.Target); });
                }
            }
        }
    }
}
