using Smod2;
using Smod2.Events;
using Smod2.EventSystem.Events;
using Smod2.EventHandlers;
using Smod2.API;
using scp4aiur;
using System.Collections.Generic;
using System.Linq;

namespace SerpentsHand
{
	class EventHandler : IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerTeamRespawn, IEventHandlerPocketDimensionEnter, IEventHandlerPocketDimensionDie,
		IEventHandlerPocketDimensionExit, IEventHandlerPlayerHurt, IEventHandlerPlayerDie, IEventHandlerCheckRoundEnd, IEventHandlerWaitingForPlayers,
		IEventHandlerSetRole, IEventHandlerDisconnect, IEventHandlerContain106
	{
	    private bool refreshPlayers;
		private bool isRoundStarted = false;

		private int respawnCount = 0;

		public void SetConfigs()
		{
			SHPlugin.shItemList = new List<int>(SHPlugin.instance.GetConfigIntList("sh_spawn_items"));
			SHPlugin.shAnnouncement = SHPlugin.instance.GetConfigString("sh_entry_announcement");
			SHPlugin.ciAnnouncement = SHPlugin.instance.GetConfigString("sh_ci_entry_announcement");

			SHPlugin.spawnChance = SHPlugin.instance.GetConfigInt("sh_spawn_chance");
			SHPlugin.shMaxSquad = SHPlugin.instance.GetConfigInt("sh_max_squad");
			SHPlugin.shHealth = SHPlugin.instance.GetConfigInt("sh_health");
			SHPlugin.teamRespawnDelay = SHPlugin.instance.GetConfigInt("sh_team_respawn_delay");

			SHPlugin.friendlyFire = SHPlugin.instance.GetConfigBool("sh_friendly_fire");
			SHPlugin.ciWinWithSCP = SHPlugin.instance.GetConfigBool("sh_ci_win_with_scp");
			SHPlugin.teleportTo106 = SHPlugin.instance.GetConfigBool("sh_teleport_to_106");
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			SetConfigs();
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			SHPlugin.shPlayers.Clear();
			SHPlugin.shPlayersInPocket.Clear();
			isRoundStarted = true;
			respawnCount = 0;
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			isRoundStarted = false;
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (SHPlugin.shPlayers.Contains(ev.Player.SteamId))
			{
				if (ev.Player.TeamRole.Team == Smod2.API.Team.TUTORIAL)
				{
					ev.Items.Clear();
					foreach (int a in SHPlugin.shItemList) ev.Items.Add((ItemType)a);
				}
				else
				{
					SHPlugin.shPlayers.Remove(ev.Player.SteamId);
				}
			}
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (ev.SpawnChaos)
			{
				if (SHPlugin.rand.Next(1, 101) <= SHPlugin.spawnChance && ev.PlayerList.Count > 0 && respawnCount >= SHPlugin.teamRespawnDelay)
				{
					List<Player> SHPlayers = new List<Player>();
					List<Player> CIPlayers = ev.PlayerList;

					for (int i = 0; i < SHPlugin.shMaxSquad && CIPlayers.Count > 0; i++)
					{
						Player player = CIPlayers[SHPlugin.rand.Next(CIPlayers.Count)];
						SHPlayers.Add(player);
						CIPlayers.Remove(player);
					}

					ev.PlayerList = SHPlayers;

					Timing.InTicks(() =>
					{
						SHPlugin.SpawnSHSquad(ev.PlayerList);
					}, 4);
				}
				else
				{
					string ann = SHPlugin.ciAnnouncement;
					if (ann != "")
						PluginManager.Manager.Server.Map.AnnounceCustomMessage(ann);
				}
			}
			respawnCount++;
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			// Is attacker is server
			if (ev.Attacker.SteamId == "0" || !isRoundStarted) return;

			if (((SHPlugin.shPlayers.Contains(ev.Player.SteamId) && (ev.Attacker.TeamRole.Team == Smod2.API.Team.SCP || ev.DamageType == DamageType.POCKET)) ||
				(SHPlugin.shPlayers.Contains(ev.Attacker.SteamId) && ev.Player.TeamRole.Team == Smod2.API.Team.SCP) ||
				(SHPlugin.shPlayers.Contains(ev.Player.SteamId) && SHPlugin.shPlayers.Contains(ev.Attacker.SteamId) &&
				ev.Player.SteamId != ev.Attacker.SteamId)) && !SHPlugin.friendlyFire)
			{
				ev.Damage = 0;
			}
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (SHPlugin.shPlayers.Contains(ev.Player.SteamId))
				SHPlugin.shPlayers.Remove(ev.Player.SteamId);

			if (ev.Player.TeamRole.Role == Role.SCP_106 && !SHPlugin.friendlyFire)
			{
				foreach (Player pl in PluginManager.Manager.Server.GetPlayers()
					.Where(p => SHPlugin.shPlayersInPocket.Contains(p.SteamId)))
				{
					pl.Kill();
				}
			}
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
		    if (refreshPlayers)
		    {
		        refreshPlayers = false;

		        string[] curPlayers = PlayerManager.singleton.players
		            .Select(x => x.GetComponent<CharacterClassManager>().SteamId).ToArray();
		        SHPlugin.shPlayers.RemoveAll(x => !curPlayers.Contains(x));
		    }

			bool MTFAlive = SHPlugin.CountRoles(Smod2.API.Team.NINETAILFOX) > 0;
			bool CiAlive = SHPlugin.CountRoles(Smod2.API.Team.CHAOS_INSURGENCY) > 0;
			bool ScpAlive = SHPlugin.CountRoles(Smod2.API.Team.SCP) > 0;
			bool DClassAlive = SHPlugin.CountRoles(Smod2.API.Team.CLASSD) > 0;
			bool ScientistsAlive = SHPlugin.CountRoles(Smod2.API.Team.SCIENTIST) > 0;
			bool SHAlive = SHPlugin.shPlayers.Count > 0;

			if (MTFAlive && (CiAlive || ScpAlive || DClassAlive || SHAlive))
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (CiAlive && (MTFAlive || (DClassAlive && ScpAlive) || ScientistsAlive || SHAlive))
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (ScpAlive && (MTFAlive || DClassAlive || ScientistsAlive))
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (SHAlive && ScpAlive && !MTFAlive && !CiAlive && !DClassAlive && !ScientistsAlive)
				ev.Status = ROUND_END_STATUS.SCP_VICTORY;
			else if (CiAlive && ScpAlive && !SHPlugin.ciWinWithSCP)
				ev.Status = ROUND_END_STATUS.ON_GOING;
		}

		public void OnPocketDimensionEnter(PlayerPocketDimensionEnterEvent ev)
		{
			if (SHPlugin.shPlayers.Contains(ev.Player.SteamId))
				SHPlugin.shPlayersInPocket.Add(ev.Player.SteamId);
		}

		public void OnPocketDimensionDie(PlayerPocketDimensionDieEvent ev)
		{
			if (SHPlugin.shPlayers.Contains(ev.Player.SteamId))
			{
				if (!SHPlugin.friendlyFire) ev.Die = false;
				if (SHPlugin.teleportTo106) SHPlugin.TeleportTo106(ev.Player);
				SHPlugin.shPlayersInPocket.Remove(ev.Player.SteamId);
			}
		}

		public void OnPocketDimensionExit(PlayerPocketDimensionExitEvent ev)
		{
			if (SHPlugin.shPlayers.Contains(ev.Player.SteamId))
			{
				if (SHPlugin.teleportTo106) SHPlugin.TeleportTo106(ev.Player);
				SHPlugin.shPlayersInPocket.Remove(ev.Player.SteamId);
			}
		}

	    public void OnDisconnect(DisconnectEvent ev)
	    {
	        refreshPlayers = true;
	    }

		public void OnContain106(PlayerContain106Event ev)
		{
			if (SHPlugin.shPlayers.Contains(ev.Player.SteamId))
			{
				ev.ActivateContainment = false;
			}
		}
	}
}
