using System;
using Smod2;
using Smod2.Events;
using Smod2.EventSystem.Events;
using Smod2.EventHandlers;
using Smod2.API;
using System.Threading;
using System.Collections.Generic;
using scp4aiur;

namespace SerpentsHand
{
	class EventHandler : IEventHandlerRoundStart, IEventHandlerTeamRespawn, IEventHandlerPocketDimensionEnter, IEventHandlerPocketDimensionDie,
		IEventHandlerPocketDimensionExit, IEventHandlerPlayerHurt, IEventHandlerPlayerDie, IEventHandlerCheckRoundEnd, IEventHandlerWaitingForPlayers
	{
		public void SetConfigs()
		{
			Plugin.spawnItems = Plugin.instance.GetConfigString("sh_spawn_items");
			Plugin.shAnnouncement = Plugin.instance.GetConfigString("sh_entry_announcement");
			Plugin.ciAnnouncement = Plugin.instance.GetConfigString("sh_ci_entry_announcement");

			Plugin.spawnChance = Plugin.instance.GetConfigInt("sh_spawn_chance");
			Plugin.shMaxSquad = Plugin.instance.GetConfigInt("sh_max_squad");
			Plugin.shHealth = Plugin.instance.GetConfigInt("sh_health");

			Plugin.friendlyFire = Plugin.instance.GetConfigBool("sh_friendly_fire");
			Plugin.ciWinWithSCP = Plugin.instance.GetConfigBool("sh_ci_win_with_scp");
			Plugin.teleportTo106 = Plugin.instance.GetConfigBool("sh_teleport_to_106");
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			SetConfigs();
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			Plugin.shPlayers.Clear();
			Plugin.shPlayersInPocket.Clear();

			foreach (string str in Plugin.spawnItems.Split(','))
				if (!int.TryParse(str, out int a))
					PluginManager.Manager.Logger.Info(Plugin.instance.Details.id, "Error: '" + str + "' is not a valid item id.");
				else
					Plugin.shItemList.Add(a);
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (ev.SpawnChaos)
			{
				if (Plugin.rand.Next(1, 101) <= Plugin.spawnChance && ev.PlayerList.Count > 0)
				{
					Timing.InTicks(() =>
					{
						Plugin.SpawnSHSquad(ev.PlayerList);
					}, 4);
				}
				else
				{
					string ann = Plugin.ciAnnouncement;
					if (ann != "")
						PluginManager.Manager.Server.Map.AnnounceCustomMessage(ann);
				}
			}
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (((Plugin.shPlayers.Contains(ev.Player.SteamId) && (ev.Attacker.TeamRole.Team == Smod2.API.Team.SCP || ev.DamageType == DamageType.POCKET)) ||
				(Plugin.shPlayers.Contains(ev.Attacker.SteamId) && ev.Player.TeamRole.Team == Smod2.API.Team.SCP) ||
				(Plugin.shPlayers.Contains(ev.Player.SteamId) && Plugin.shPlayers.Contains(ev.Attacker.SteamId) &&
				ev.Player.SteamId != ev.Attacker.SteamId)) && !Plugin.friendlyFire)
			{
				ev.Damage = 0;
			}
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (Plugin.shPlayers.Contains(ev.Player.SteamId))
				Plugin.shPlayers.Remove(ev.Player.SteamId);

			if (ev.Player.TeamRole.Role == Role.SCP_106 && !Plugin.friendlyFire)
			{
				foreach (string str in Plugin.shPlayersInPocket)
				{
					Plugin.FindPlayer(str).Kill();
				}
			}
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			bool MTFAlive = Plugin.CountRoles(Smod2.API.Team.NINETAILFOX) > 0;
			bool CiAlive = Plugin.CountRoles(Smod2.API.Team.CHAOS_INSURGENCY) > 0;
			bool ScpAlive = Plugin.CountRoles(Smod2.API.Team.SCP) > 0;
			bool DClassAlive = Plugin.CountRoles(Smod2.API.Team.CLASSD) > 0;
			bool ScientistsAlive = Plugin.CountRoles(Smod2.API.Team.SCIENTISTS) > 0;
			bool SHAlive = Plugin.shPlayers.Count > 0;

			if (MTFAlive && (CiAlive || ScpAlive || DClassAlive || SHAlive))
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (CiAlive && (MTFAlive || (DClassAlive && ScpAlive) || ScientistsAlive || SHAlive))
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (ScpAlive && (MTFAlive || DClassAlive || ScientistsAlive))
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (SHAlive && ScpAlive && !MTFAlive && !CiAlive && !DClassAlive && !ScientistsAlive)
				ev.Status = ROUND_END_STATUS.SCP_VICTORY;
			else if (CiAlive && ScpAlive && !Plugin.ciWinWithSCP)
				ev.Status = ROUND_END_STATUS.ON_GOING;
		}

		public void OnPocketDimensionEnter(PlayerPocketDimensionEnterEvent ev)
		{
			if (Plugin.shPlayers.Contains(ev.Player.SteamId))
				Plugin.shPlayersInPocket.Add(ev.Player.SteamId);
		}

		public void OnPocketDimensionDie(PlayerPocketDimensionDieEvent ev)
		{
			if (Plugin.shPlayers.Contains(ev.Player.SteamId))
			{
				if (!Plugin.friendlyFire)
					ev.Die = false;
				if (Plugin.teleportTo106)
					Plugin.TeleportTo106(ev.Player);
			}
		}

		public void OnPocketDimensionExit(PlayerPocketDimensionExitEvent ev)
		{
			if (Plugin.shPlayers.Contains(ev.Player.SteamId))
			{
				if (Plugin.teleportTo106)
					Plugin.TeleportTo106(ev.Player);
				Plugin.shPlayersInPocket.Remove(ev.Player.SteamId);
			}
		}
	}
}
