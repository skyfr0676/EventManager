using Exiled.Events.EventArgs;
using Exiled.API.Features;
using System.Collections.Generic;
using MEC;
using Exiled.API.Features.Items;
using Exiled.API.Extensions;
using UnityEngine;
using Exiled.API.Enums;
using System.Linq;
using Respawning;
using Exiled.Events.Commands.Reload;

namespace EventManager.Games
{
    public class GunGame
    {
        public static bool IsStarted;

        public static Dictionary<Player, int> PlayersGunRank = new Dictionary<Player, int>();

        public static void Start(Player Starter)
        {
            if (!Round.IsStarted || IsStarted || Plugin.GameAlreadyEnabled)
            {
                Server.FriendlyFire = true;
                Server.SpawnProtectTime = Plugin.Singleton.Config.GunGameSpawnProtectTime;
                Log.Debug($"GunGame mode has been started by {Starter.Nickname} !",Plugin.Singleton.Config.Debug);
                LightContainmentZoneDecontamination.DecontaminationController.Singleton.disableDecontamination = true;
                Round.Start();
                Round.IsLocked = true;
                IsStarted = true;
                Plugin.GameAlreadyEnabled = true;

                foreach (Player plr in Player.List)
                {
                    PlayersGunRank.Add(plr, 1);
                    plr.SetRole(RoleType.ClassD,SpawnReason.RoundStart);
                    plr.ShowHint("starting game : <color=yellow>gungame</color>...",int.MaxValue);
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
            foreach (Player plr in PlayersGunRank.Keys)
            {
                bool ro = true;
                Room room = Room.Random(ZoneType.HeavyContainment);
                while (ro)
                {
                    if (room.Type == RoomType.Pocket)
                    {
                        room = Room.Random(ZoneType.HeavyContainment);
                    }
                    else
                    {
                        break;
                    }
                }
                plr.Position = room.Position + new Vector3(0,1,0);
                ChangeItem(plr, PlayersGunRank[plr]);
            }
        }

        public static void AddLevel(Player AddRank)
        {
            if (PlayersGunRank.ContainsKey(AddRank))
            {
                AddRank.ShowHint(Plugin.Singleton.Config.GunGameNextLevel);
                PlayersGunRank[AddRank] += 1;
                ChangeItem(AddRank, PlayersGunRank[AddRank]);
            }
        }

        public static void LostLevel(Player AddRank)
        {
            if (PlayersGunRank.ContainsKey(AddRank))
            {
                AddRank.ShowHint(Plugin.Singleton.Config.GunGameLostLevel);
                PlayersGunRank[AddRank] -= 1;
                ChangeItem(AddRank, PlayersGunRank[AddRank]);
            }
        }

        //Current order : Micro-HID, 3-X Particle Disrupter, Logicer, Shotgun, AK, MTF-E11-SR, Crossvec, FSP-9, .44 Revolver, COM-18, COM-15, Grenade
        public static void ChangeItem(Player plr,int NewRank)
        {
            plr.ClearInventory();

            if (NewRank == 1)
            {
                plr.AddItem(ItemType.MicroHID);
                plr.AddItem(ItemType.MicroHID);
                plr.AddItem(ItemType.MicroHID);
                plr.AddItem(ItemType.Adrenaline);
            }
            if (NewRank == 2)
            {
                plr.AddItem(ItemType.ParticleDisruptor);
                plr.AddItem(ItemType.ParticleDisruptor);
                plr.AddItem(ItemType.ParticleDisruptor);
                plr.AddItem(ItemType.Adrenaline);
            }
            if (NewRank == 3)
            {
                plr.AddItem(ItemType.GunLogicer);
                plr.AddAmmo(ItemType.GunLogicer.GetWeaponAmmoType(), 500);
                plr.AddItem(ItemType.Adrenaline);
            }
            if (NewRank == 4)
            {
                plr.AddItem(ItemType.GunShotgun);
                plr.SetAmmo(ItemType.GunLogicer.GetWeaponAmmoType(), 0);
                plr.AddAmmo(ItemType.GunShotgun.GetWeaponAmmoType(), 500);                
                plr.AddItem(ItemType.Adrenaline);
            }
            if (NewRank == 5)
            {
                plr.AddItem(ItemType.GunAK);
                plr.SetAmmo(ItemType.GunShotgun.GetWeaponAmmoType(), 0);
                plr.AddAmmo(ItemType.GunAK.GetWeaponAmmoType(), 500);                
                plr.AddItem(ItemType.Adrenaline);
            }
            if (NewRank == 6)
            {
                plr.AddItem(ItemType.GunE11SR);
                plr.SetAmmo(ItemType.GunAK.GetWeaponAmmoType(), 0);
                plr.AddAmmo(ItemType.GunE11SR.GetWeaponAmmoType(), 500);                
                plr.AddItem(ItemType.Adrenaline);
            }
            if (NewRank == 7)
            {
                plr.AddItem(ItemType.GunCrossvec);
                plr.SetAmmo(ItemType.GunE11SR.GetWeaponAmmoType(), 0);
                plr.AddAmmo(ItemType.GunCrossvec.GetWeaponAmmoType(), 500);                
                plr.AddItem(ItemType.Adrenaline);
            }
            if (NewRank == 8)
            {
                plr.AddItem(ItemType.GunFSP9);
                plr.SetAmmo(ItemType.GunCrossvec.GetWeaponAmmoType(), 0);
                plr.AddAmmo(ItemType.GunFSP9.GetWeaponAmmoType(), 500);                
                plr.AddItem(ItemType.Adrenaline);
            }
            if (NewRank == 9)
            {
                plr.AddItem(ItemType.GunRevolver);
                plr.SetAmmo(ItemType.GunFSP9.GetWeaponAmmoType(), 0);
                plr.AddAmmo(ItemType.GunRevolver.GetWeaponAmmoType(), 500);                
                plr.AddItem(ItemType.Adrenaline);
            }
            if (NewRank == 10)
            {
                plr.AddItem(ItemType.GunCOM18);
                plr.SetAmmo(ItemType.GunRevolver.GetWeaponAmmoType(), 0);
                plr.AddAmmo(ItemType.GunCOM18.GetWeaponAmmoType(), 500);                
                plr.AddItem(ItemType.Adrenaline);
            }
            if (NewRank == 11)
            {
                plr.AddItem(ItemType.GunCOM15);
                plr.SetAmmo(ItemType.GunCOM18.GetWeaponAmmoType(), 0);
                plr.AddAmmo(ItemType.GunCOM15.GetWeaponAmmoType(), 500);                
                plr.AddItem(ItemType.Adrenaline);
            }
            if (NewRank == 12)
            {
                plr.SetAmmo(ItemType.GunCOM15.GetWeaponAmmoType(), 0);
                plr.AddItem(ItemType.GrenadeHE);
                plr.AddItem(ItemType.GrenadeHE);
                plr.AddItem(ItemType.GrenadeHE);
                plr.AddItem(ItemType.Adrenaline);
            }
            if (NewRank >= 13)
            {
                plr.ClearInventory();
                plr.AddItem(ItemType.ParticleDisruptor);
                plr.AddItem(ItemType.ParticleDisruptor);
                plr.AddItem(ItemType.ParticleDisruptor);
                foreach (Player Left in Player.List.Where(x => x.UserId != plr.UserId))
                {
                    Left.SetRole(RoleType.Spectator,SpawnReason.Overwatch);
                    string Winner = Plugin.Singleton.Config.GunGameWinnerMsgForLost.Replace("{winner}", plr.Nickname);
                    Left.Broadcast(Plugin.Singleton.Config.GunGameWinnerDuration, Winner);
                }
                plr.Broadcast(Plugin.Singleton.Config.GunGameWinnerDuration, Plugin.Singleton.Config.GunGameWinnerMsgForWinner);
                IsStarted = false;
                Plugin.GameAlreadyEnabled = false;
                Round.EndRound(true);
                Round.IsLocked = false;
                return;
            }
        }

        public void UsingElevator(InteractingElevatorEventArgs ev)
        {
            if (IsStarted)
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint(Plugin.Singleton.Config.GunGameElevunauthorized);
            }
        }

        public void Tesla(TriggeringTeslaEventArgs ev)
        {
            if (IsStarted)
            {
                ev.Tesla.IsIdling = false;
                ev.IsTriggerable = false;
            }
            else
            {
                ev.IsTriggerable = true;
            }
        }

        public void InteractingDoor(InteractingDoorEventArgs ev)
        {
            if (IsStarted && ev.Door.IsKeycardDoor)
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint(Plugin.Singleton.Config.GunGameDoorunauthorized);
            }
        }

        public void Dying(DyingEventArgs ev)
        {
            if (IsStarted)
            {
                if (ev.Killer != null)
                {
                    if (ev.Killer == ev.Target)
                        LostLevel(ev.Killer);
                    else
                        AddLevel(ev.Killer);
                }

                string DeadMsg = Plugin.Singleton.Config.GunGameDeadMsg.Replace("{time}",Plugin.Singleton.Config.GunGameTimeBeforeRespawning.ToString());
                ev.Target.ShowHint(DeadMsg, Plugin.Singleton.Config.GunGameTimeBeforeRespawning);
                Timing.CallDelayed(Plugin.Singleton.Config.GunGameTimeBeforeRespawning, () => {Spawn(ev.Target); });
            }
        }

        public static void Spawn(Player Spawner)
        {
            Spawner.SetRole(RoleType.ClassD, SpawnReason.Revived);
            Timing.CallDelayed(2f, () =>
            {
                Room room = Room.Random(ZoneType.HeavyContainment);
                Spawner.Position = room.Position + new Vector3(0, 1, 0);
                ChangeItem(Spawner, PlayersGunRank[Spawner]);
            });
        }

        public void PickupItem(PickingUpItemEventArgs ev)
        {
            if (IsStarted)
            {
                ev.IsAllowed = false;
            }
            else
            {
                ev.IsAllowed = true;
            }
        }

        public void Left(LeftEventArgs ev)
        {
            if (IsStarted)
            {
                if (PlayersGunRank.ContainsKey(ev.Player))
                {
                    PlayersGunRank.Remove(ev.Player);
                }
            }
        }

        public void Verified(VerifiedEventArgs ev)
        {
            if (IsStarted)
            {
                PlayersGunRank.Add(ev.Player, 1);
                string SpawnMsg = Plugin.Singleton.Config.GunGameLateJoinMsg.Replace("{time}", Plugin.Singleton.Config.GunGameTimeBeforeRespawning.ToString());
                ev.Player.ShowHint(SpawnMsg, Plugin.Singleton.Config.GunGameTimeBeforeRespawning);
                Timing.CallDelayed(5f, () => { Spawn(ev.Player); });
            }
        }
    }
}