using EventManagerRework.Features.Component;
using EventManagerRework.Features.Extensions.GunGameExtensions;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Event = Exiled.Events.Handlers;
using Object = UnityEngine.Object;

namespace EventManagerRework.Events.GunGame
{
    public class GunGame
    {
        public static bool IsStarted = false;
        public static bool StartingInProgress = false;
        public static Dictionary<Player, int> PlayersRank = new();
        public static bool DefaultFF;
        public static bool DefaultLock;
        public static CoroutineHandle Coroutine;

        public static void FinishRound(Player winner)
        {
            foreach (Player loser in Player.List.Where(x => x != winner))
            {
                loser.Role.Set(RoleTypeId.Spectator);
                string replace = Plugin.StaticConfig.GunGameConfig.LooseMessage.Replace("{winner}", winner.Nickname);
                loser.Broadcast(10, replace,shouldClearPrevious:true);
            }
            
            winner.Broadcast(10, Plugin.StaticConfig.GunGameConfig.WinMessage, shouldClearPrevious:true);
            IsStarted = false;
            Plugin.AnotherGameHasAlreadyStarted = false;
            foreach (KeyValuePair<ElevatorManager.ElevatorGroup, List<ElevatorDoor>> door in ElevatorDoor.AllElevatorDoors)
            {
                foreach (ElevatorDoor ED in door.Value)
                    ED.ServerChangeLock(DoorLockReason.AdminCommand, false);
            }
            Server.FriendlyFire = DefaultFF;
            Round.IsLocked = DefaultLock;
        }

        public static string Start()
        {
            if (IsStarted || Round.IsStarted || Plugin.AnotherGameHasAlreadyStarted)
                return "error, the GunGame has already started, or the round has already sstarted or an another event has already started.";

            foreach (KeyValuePair<ElevatorManager.ElevatorGroup, List<ElevatorDoor>> door in ElevatorDoor.AllElevatorDoors)
            {
                foreach (ElevatorDoor ED in door.Value)
                    ED.ServerChangeLock(DoorLockReason.AdminCommand, true);
            }
            DefaultFF = Server.FriendlyFire;
            DefaultLock = Round.IsLocked;
            Server.FriendlyFire = true;
            Round.IsLocked = true;

            PlayersRank.Clear();
            
            Round.Start();

            IsStarted = true;
            StartingInProgress = true;
            Plugin.AnotherGameHasAlreadyStarted = true;

            LoadMap();
            Timing.CallDelayed(2f, () =>
            {
                StartingInProgress = false;
                Started();
            });

            return "game GunGame launched !";
        }
        public static void ChangeRole(ChangingRoleEventArgs ev)
        {
            if (StartingInProgress)
            {
                ev.NewRole = RoleTypeId.Tutorial;
                ev.Items.Clear();
                ev.Ammo.Clear();
                ev.Player.ShowHint("starting game <color=yellow>GunGame</color>...", int.MaxValue);
            }
            if (IsStarted)
            {
                if (!Coroutine.IsRunning)
                    Coroutine = Timing.RunCoroutine(CheckLeaderPlayer());
            }
        }

        public static void Started()
        {
            foreach (Player plr in Player.List)
            {
                plr.ShowHint("", 1);
                PlayersRank.Add(plr, 1);
                plr.Respawn();
            }
        }

        public static void Dying(DyingEventArgs ev)
        {
            if (!IsStarted)
                return;

            if (!Coroutine.IsRunning)
                Coroutine = Timing.RunCoroutine(CheckLeaderPlayer());
            Timing.RunCoroutine(ev.Player.Respawning());
            ev.Player.ClearInventory(true);
            if (ev.Attacker is null)
                return;
            if (ev.Attacker == ev.Player)
                return;
            ev.Attacker.GrantLevel();
            ev.Attacker.Heal(100, false);
        }

        public static void PickupItem(PickingUpItemEventArgs ev)
        {
            if (!IsStarted) return;

            if (!Coroutine.IsRunning)
                Coroutine = Timing.RunCoroutine(CheckLeaderPlayer());

            ev.Player.ShowHint(Plugin.StaticConfig.GunGameConfig.NoPickupMessage);
            ev.IsAllowed = false;
        }

        public static void DropItem(DroppingItemEventArgs ev)
        {
            if (!IsStarted) return;

            if (!Coroutine.IsRunning)
                Coroutine = Timing.RunCoroutine(CheckLeaderPlayer());

            ev.Player.ShowHint(Plugin.StaticConfig.GunGameConfig.NoDropMessage);
            ev.IsAllowed = false;
        }

        public static void Verified(VerifiedEventArgs ev)
        {
            if (!IsStarted) return;

            if (!Coroutine.IsRunning)
                Coroutine = Timing.RunCoroutine(CheckLeaderPlayer());

            PlayersRank.Add(ev.Player, 1);
            ServerConsole.AddLog("[EventManagerRework] player " + ev.Player.Nickname + " was join event GunGame", ConsoleColor.Yellow);
            Timing.RunCoroutine(ev.Player.Respawning());
        }

        public static void TriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (!IsStarted) return;

            if (!Coroutine.IsRunning)
                Coroutine = Timing.RunCoroutine(CheckLeaderPlayer());

            ev.IsAllowed = false;
        }
        
        public static void Left(LeftEventArgs ev)
        {
            if (!IsStarted) return;

            if (!Coroutine.IsRunning)
                Coroutine = Timing.RunCoroutine(CheckLeaderPlayer());

            if (!PlayersRank.ContainsKey(ev.Player))
                return;
            ServerConsole.AddLog("[EventManagerRework] player " + ev.Player.Nickname + " was left event GunGame", ConsoleColor.Yellow);
            PlayersRank.Remove(ev.Player);
        }

        public static void LoadMap()
        {
            Door a = Door.Get(DoorType.CheckpointLczA);
            Door b = Door.Get(DoorType.CheckpointLczB);
            Door c = Door.Get(DoorType.CheckpointArmoryA);
            Door d = Door.Get(DoorType.CheckpointArmoryB);
            Door e = Door.Get(DoorType.CheckpointEzHczA);
            Door f = Door.Get(DoorType.CheckpointEzHczB);
            foreach (Door door in Door.List)
            {
                if (door.Type == DoorType.CheckpointGate)
                    door.IsOpen = true;

                if (door.Zone != Plugin.StaticConfig.GunGameConfig.ZoneSpawn)
                {
                    door.ChangeLock(DoorLockType.Regular079);
                }
                if (Plugin.StaticConfig.GunGameConfig.ZoneSpawn == ZoneType.Other || Plugin.StaticConfig.GunGameConfig.ZoneSpawn == ZoneType.Unspecified)
                {
                    door.IsOpen = true;
                }
            }
            a.BreakDoor();
            b.BreakDoor();
            c.BreakDoor();
            d.BreakDoor();
            e.BreakDoor();
            f.BreakDoor();
        }

        public static void Register()
        {
            Event.Player.ChangingRole += ChangeRole;
            Event.Player.Dying += Dying;
            Event.Player.TriggeringTesla += TriggeringTesla;
            Event.Player.Verified += Verified;
            Event.Player.Left += Left;
            Event.Player.PickingUpItem += PickupItem;
            Event.Player.DroppingItem += DropItem;
        }

        public static void UnRegister()
        {
            Event.Player.ChangingRole -= ChangeRole;
            Event.Player.Dying -= Dying;
            Event.Player.TriggeringTesla -= TriggeringTesla;
            Event.Player.Verified -= Verified;
            Event.Player.Left -= Left;
            Event.Player.PickingUpItem -= PickupItem;
            Event.Player.DroppingItem -= DropItem;
        }

        public static IEnumerator<float> CheckLeaderPlayer()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(1f);
                if (!IsStarted) continue;
                KeyValuePair<Player, int> Leader = new(null,-1);
                foreach (KeyValuePair<Player, int> player in PlayersRank)
                {
                    if (player.Value < 3)
                        continue;
                    if (Leader.Key is null)
                        Leader = player;
                    if (Leader.Value == player.Value)
                    {
                        if (!player.Key.GameObject.TryGetComponent(out LightComponent _))
                            player.Key.GameObject.AddComponent<LightComponent>();
                    }
                    if (Leader.Value < player.Value && Leader.Value != player.Value && Leader.Key != player.Key)
                    {
                        if (!player.Key.GameObject.TryGetComponent(out LightComponent _))
                            player.Key.GameObject.AddComponent<LightComponent>();
                        if (Leader.Key.GameObject.TryGetComponent(out LightComponent t))
                            Object.Destroy(t);

                        ServerConsole.AddLog("[EventManagerRework] New leader has been designated : " + player.Key.Nickname + ".", ConsoleColor.Yellow);
                        Leader = player;
                    }
                }
                if (Leader.Key is not null)
                {
                    string message = Plugin.StaticConfig.GunGameConfig.LeaderMessage.Replace("{player}", Leader.Key.Nickname).Replace("{point}", Leader.Value.ToString());
                    foreach (Player plr in Player.List)
                        plr.Broadcast(2, message, shouldClearPrevious:true);
                }
                else
                {
                    foreach (Player plr in Player.List)
                        plr.Broadcast(2, Plugin.StaticConfig.GunGameConfig.NoLeaderMessage, shouldClearPrevious: true);
                }
            }
        }
    }
}
