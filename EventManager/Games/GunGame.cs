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
using Dissonance;
using EventManager.Extensions;

namespace EventManager.Games
{
    public class GunGame
    {
        public static bool IsStarted;
        public static float DefaultSpawnProtectTime;

        public static ZoneType StartZone;

        public static Dictionary<Player, int> PlayersGunRank = new Dictionary<Player, int>();

        public void Init()
        {
            if (!Plugin.singleton.Config.GunGameCustomSpawn)
            {
                switch (Plugin.singleton.Config.GunGameZone)
                {
                    case ZoneType.Surface:
                    case ZoneType.Entrance:
                    case ZoneType.HeavyContainment:
                    case ZoneType.LightContainment:
                        StartZone = Plugin.singleton.Config.GunGameZone;
                        break;
                    default:
                        Log.Warn($"[GUNGAME] config gun_game_zone has not a ZoneType (Surface,Entrance, HeavyContainment or LightContainment). I choose default zone, {ZoneType.HeavyContainment}.");
                        StartZone = ZoneType.HeavyContainment;
                        break;
                }
            }
            else
            {
                StartZone = ZoneType.Unspecified;
            }
        }

        public void ReloadedConfigs()
        {
            Init();
        }

        public static void Start(Player Starter)
        {
            if (!Round.IsStarted || !IsStarted || !Plugin.AnotherGamerHasEnabled)
            {
                DefaultSpawnProtectTime = Server.SpawnProtectTime;
                Server.FriendlyFire = true;
                Server.SpawnProtectTime = Plugin.singleton.Config.GunGameSpawnProtectTime;
                Log.Debug($"GunGame mode has been started by {Starter.Nickname} !",Plugin.singleton.Config.Debug);
                LightContainmentZoneDecontamination.DecontaminationController.Singleton.disableDecontamination = true;
                Round.Start();
                Round.IsLocked = true;
                IsStarted = true;
                Plugin.AnotherGamerHasEnabled = true;

                foreach (Player plr in Player.List)
                {
                    PlayersGunRank.Add(plr, 1);
                    Timing.CallDelayed(2f, () =>
                    {
                        plr.SetRole(RoleType.Tutorial,SpawnReason.RoundStart);
                    });
                    plr.ShowHint("starting game : <color=yellow>gungame</color>...",int.MaxValue);
                }
                Timing.CallDelayed(Plugin.singleton.Config.WaitTimeToStartGame, () =>
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
                plr.SetRole(RoleType.ClassD, SpawnReason.RoundStart);
                if (Plugin.singleton.Config.GunGameCustomSpawn)
                {
                    Vector3 CustomSpawn = Plugin.singleton.Config.GunGameCustomSpawnLocation;
                    plr.Position = CustomSpawn + new Vector3(0, 1, 0);
                }
                else
                {
                    plr.Respawn(typeof(GunGame));
                }
                string StartMsg = Plugin.singleton.Config.GunGameStartMsg.Replace("{time}",Plugin.singleton.Config.GunGameSpawnProtectTime.ToString());
                plr.ShowHint(StartMsg,Plugin.singleton.Config.GunGameSpawnProtectTime);
                GiveItem(plr);
            }
        }

        public static void AddLevel(Player AddRank)
        {
            if (PlayersGunRank.ContainsKey(AddRank))
            {
                AddRank.ShowHint(Plugin.singleton.Config.GunGameNextLevel);
                PlayersGunRank[AddRank] += 1;
                GiveItem(AddRank);
            }
        }

        public static void LostLevel(Player RemoveRank)
        {
            if (PlayersGunRank.ContainsKey(RemoveRank))
            {
                RemoveRank.ShowHint(Plugin.singleton.Config.GunGameLostLevel);
                PlayersGunRank[RemoveRank] -= 1;
                GiveItem(RemoveRank);
            }
        }

        //default order : Micro-HID, 3-X Particle Disrupter, Logicer, Shotgun, AK, MTF-E11-SR, Crossvec, FSP-9, .44 Revolver, COM-18, COM-15, Grenade
        public static void GiveItem(Player plr)
        {
            plr.ClearInventory();

            int Rank = PlayersGunRank[plr]-1;

            ItemType[] ItemToAdd = Plugin.singleton.Config.GunGameOrder;
            
            if (Rank < ItemToAdd.Count())
            {
                if (!ItemToAdd[Rank].IsWeapon(false, false))
                {
                    plr.AddItem(ItemToAdd[Rank]);
                    plr.AddItem(ItemToAdd[Rank]);
                    plr.AddItem(ItemToAdd[Rank]);
                    plr.AddItem(ItemType.Adrenaline);
                }
                else
                {
                    plr.AddItem(ItemToAdd[Rank]);
                    plr.SetAmmo(ItemToAdd[Rank].GetWeaponAmmoType(), 500);
                    if (ItemToAdd[Rank - 1].IsWeapon(false, false))
                        plr.SetAmmo(ItemToAdd[Rank - 1].GetWeaponAmmoType(), 0);
                    plr.AddItem(ItemType.Adrenaline);
                }
            }
            if (Rank >= ItemToAdd.Count())
            {
                plr.ClearInventory();
                foreach (Player Left in Player.List.Where(x => x.UserId != plr.UserId))
                {
                    Left.SetRole(RoleType.Spectator,SpawnReason.Overwatch);
                    string Winner = Plugin.singleton.Config.GunGameWinnerMsgForLost.Replace("{winner}", plr.Nickname);
                    Left.Broadcast(Plugin.singleton.Config.GunGameWinnerDuration, Winner);
                }
                plr.Broadcast(Plugin.singleton.Config.GunGameWinnerDuration, Plugin.singleton.Config.GunGameWinnerMsgForWinner);
                IsStarted = false;
                Plugin.AnotherGamerHasEnabled = false;
                Server.SpawnProtectTime = DefaultSpawnProtectTime;
                Server.FriendlyFire = false;
                Round.EndRound(true);
                Round.IsLocked = false;
                return;
            }
        }

        public void Throwing(ThrowingItemEventArgs ev)
        {
            if (IsStarted && ev.Item.Type != ItemType.GrenadeHE)
            {
                ev.IsAllowed = false;
            }
            else
            {
                ev.IsAllowed = true;
            }
        }

        public void UsingElevator(InteractingElevatorEventArgs ev)
        {
            if (IsStarted)
            {
                ev.IsAllowed = false;
                ev.Player.ShowHint(Plugin.singleton.Config.GunGameElevunauthorized);
            }
        }

        public void Tesla(TriggeringTeslaEventArgs ev)
        {
            if (IsStarted)
            {
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
                ev.Player.ShowHint(Plugin.singleton.Config.GunGameDoorunauthorized);
            }
        }

        public void Dying(DyingEventArgs ev)
        {
            if (IsStarted)
            {
                if (ev.Killer != null)
                {
                    if (ev.Killer == ev.Target)
                    {
                        LostLevel(ev.Killer);
                        ev.Target.ShowHint(Plugin.singleton.Config.GunGameLostLevel, Plugin.singleton.Config.GunGameTimeBeforeRespawning);
                    }
                    else
                    {
                        AddLevel(ev.Killer);
                        string DeadMsg = Plugin.singleton.Config.GunGameDeadMsg.Replace("{time}", Plugin.singleton.Config.GunGameTimeBeforeRespawning.ToString());
                        ev.Target.ShowHint(DeadMsg, Plugin.singleton.Config.GunGameTimeBeforeRespawning);
                    }
                }
                if (ev.Killer == null)
                {
                    string DeadMsg = Plugin.singleton.Config.GunGameDeadMsg.Replace("{time}", Plugin.singleton.Config.GunGameTimeBeforeRespawning.ToString());
                    ev.Target.ShowHint(DeadMsg, Plugin.singleton.Config.GunGameTimeBeforeRespawning);
                }

                Timing.CallDelayed(Plugin.singleton.Config.GunGameTimeBeforeRespawning, () => {Spawn(ev.Target); });
            }
        }

        public static void Spawn(Player Spawner)
        {
            Spawner.SetRole(RoleType.ClassD, SpawnReason.Revived);
            Timing.CallDelayed(2f, () =>
            {
                if (Plugin.singleton.Config.GunGameCustomSpawn)
                {
                    Vector3 CustomSpawn = Plugin.singleton.Config.GunGameCustomSpawnLocation;
                    Spawner.Position = CustomSpawn + new Vector3(0, 1, 0);
                }
                else
                {
                    Spawner.Respawn(typeof(GunGame));
                }
                GiveItem(Spawner);
            });
        }

        public void Pickup(PickingUpItemEventArgs ev)
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

        public void Pickup(PickingUpAmmoEventArgs ev)
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

        public void Pickup(PickingUpArmorEventArgs ev)
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

        public void Dropping(DroppingAmmoEventArgs ev)
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

        public void Dropping(DroppingItemEventArgs ev)
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

        public void RoundRestart()
        {
            if (IsStarted)
            {
                Plugin.AnotherGamerHasEnabled = false;
                IsStarted = false;
                PlayersGunRank.Clear();
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
                float time = Plugin.singleton.Config.GunGameTimeBeforeRespawning > 2f ? Plugin.singleton.Config.GunGameTimeBeforeRespawning : 2;
                string SpawnMsg = Plugin.singleton.Config.GunGameLateJoinMsg.Replace("{time}", time.ToString());
                ev.Player.ShowHint(SpawnMsg, time);
                Timing.CallDelayed(Plugin.singleton.Config.GunGameTimeBeforeRespawning, () => { Spawn(ev.Player); });
            }
        }
    }
}