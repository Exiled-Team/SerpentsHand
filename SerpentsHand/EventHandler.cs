using System;
using Smod2;
using Smod2.Events;
using Smod2.EventSystem.Events;
using Smod2.EventHandlers;
using Smod2.API;
using System.Threading;

namespace SerpentsHand
{
	class EventHandler : IEventHandlerRoundStart, IEventHandlerTeamRespawn, IEventHandlerPocketDimensionEnter, IEventHandlerPocketDimensionDie, IEventHandlerPocketDimensionExit, IEventHandlerPlayerHurt, IEventHandlerPlayerDie, IEventHandlerCheckRoundEnd
	{
		private Plugin plugin;
		private Random rand = new Random();

		public EventHandler(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			SerpentsHand.shPlayers.Clear();
			SerpentsHand.shPlayersInPocket.Clear();
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (ev.SpawnChaos)
			{
				if (rand.Next(0, 101) <= plugin.GetConfigInt("sh_spawn_chance"))
				{
					foreach (Player player in ev.PlayerList)
					{
						Thread SpawnDelay = new Thread(new ThreadStart(() => new SpawnDelay(player, 100)));
						SpawnDelay.Start();
					}
					plugin.pluginManager.Server.Map.AnnounceCustomMessage(plugin.GetConfigString("sh_entry_announcement"));
				}
				else
				{
					plugin.pluginManager.Server.Map.AnnounceCustomMessage(plugin.GetConfigString("sh_ci_entry_announcement"));
				}
			}
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (((SerpentsHand.shPlayers.Contains(ev.Player.SteamId) && (ev.Attacker.TeamRole.Team == Team.SCP || ev.DamageType == DamageType.POCKET)) ||
				(SerpentsHand.shPlayers.Contains(ev.Attacker.SteamId) && ev.Player.TeamRole.Team == Team.SCP) ||
				(SerpentsHand.shPlayers.Contains(ev.Player.SteamId) && SerpentsHand.shPlayers.Contains(ev.Attacker.SteamId) &&
				ev.Player.SteamId != ev.Attacker.SteamId)) && !plugin.GetConfigBool("sh_friendly_fire"))
			{
				ev.Damage = 0;
			}
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (SerpentsHand.shPlayers.Contains(ev.Player.SteamId))
				SerpentsHand.shPlayers.Remove(ev.Player.SteamId);

			if (ev.Player.TeamRole.Role == Role.SCP_106 && !plugin.GetConfigBool("sh_friendly_fire"))
			{
				foreach (string str in SerpentsHand.shPlayersInPocket)
				{
					SerpentsHand.FindPlayer(str).Kill();
				}
			}
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			int MTFAlive = SerpentsHand.CountRoles(Team.NINETAILFOX);
			int CiAlive = SerpentsHand.CountRoles(Team.CHAOS_INSURGENCY);
			int ScpAlive = SerpentsHand.CountRoles(Team.SCP);
			int DClassAlive = SerpentsHand.CountRoles(Team.CLASSD);
			int ScientistsAlive = SerpentsHand.CountRoles(Team.SCIENTISTS);
			int SHAlive = SerpentsHand.shPlayers.Count;

			if (SHAlive > 0 && ScpAlive > 0 || (SHAlive > 0 && MTFAlive < 1 && CiAlive < 1 && ScpAlive < 1 && DClassAlive < 1 && ScientistsAlive < 1))
				ev.Status = ROUND_END_STATUS.SCP_VICTORY;
			if ((CiAlive > 0 && ScpAlive > 0 && !plugin.GetConfigBool("sh_ci_win_with_scp")) || (SHAlive > 0 && (MTFAlive > 0 || CiAlive > 0 || DClassAlive > 0 || ScientistsAlive > 0)))
				ev.Status = ROUND_END_STATUS.ON_GOING;
		}

		public void OnPocketDimensionEnter(PlayerPocketDimensionEnterEvent ev)
		{
			if (SerpentsHand.shPlayers.Contains(ev.Player.SteamId))
			{
				SerpentsHand.shPlayersInPocket.Add(ev.Player.SteamId);
			}
		}

		public void OnPocketDimensionDie(PlayerPocketDimensionDieEvent ev)
		{
			if (SerpentsHand.shPlayers.Contains(ev.Player.SteamId))
			{
				if (!plugin.GetConfigBool("sh_friendly_fire"))
					ev.Die = false;
				if (plugin.GetConfigBool("sh_teleport_to_106"))
					SerpentsHand.TeleportTo106(ev.Player);
			}
		}

		public void OnPocketDimensionExit(PlayerPocketDimensionExitEvent ev)
		{
			if (SerpentsHand.shPlayers.Contains(ev.Player.SteamId))
			{
				if (plugin.GetConfigBool("sh_teleport_to_106"))
				{
					SerpentsHand.TeleportTo106(ev.Player);
				}
				SerpentsHand.shPlayersInPocket.Remove(ev.Player.SteamId);
			}
		}
	}
}
