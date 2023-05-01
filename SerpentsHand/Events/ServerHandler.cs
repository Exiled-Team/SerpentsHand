using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.Events.EventArgs.Server;
using MEC;
using PlayerRoles;
using Respawning;
using System.Collections.Generic;
using System.Linq;

namespace SerpentsHand.Events
{
    internal sealed class ServerHandler
    {
        private Config config = SerpentsHand.Singleton.Config;

        private SerpentsHand plugin = SerpentsHand.Singleton;

        public void OnWaitingForPlayers()
        {
            plugin.TeamRespawnCount = 0;
            plugin.SerpentsRespawnCount = 0;
        }

        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            plugin.TeamRespawnCount++;

            if (ev.NextKnownTeam != SpawnableTeamType.ChaosInsurgency)
                return;

            Extensions.CalculateChance();

            if (!plugin.IsSpawnable)
            {
                if(!string.IsNullOrEmpty(config.SpawnManager.ChaosEntryAnnoucement))
                    Cassie.GlitchyMessage(config.SpawnManager.ChaosEntryAnnoucement, 0.05f, 0.05f);
                return;
            }

            ev.IsAllowed = false;

            List<Player> sHPlayers = new List<Player>();
            for (int i = 0; i < config.SpawnManager.MaxSquad && ev.Players.Count > 0; i++)
            {
                Player player = ev.Players[UnityEngine.Random.Range(0, ev.Players.Count)];
                sHPlayers.Add(player);
                ev.Players.Remove(player);
            }

            Timing.CallDelayed(0.1f, () => API.SpawnSquad(sHPlayers));

            if (config.SpawnManager.MaxSpawns > 0)
                plugin.SerpentsRespawnCount++;

            plugin.IsSpawnable = false;
            ev.NextKnownTeam = SpawnableTeamType.None;
        }

        public void OnEndingRound(EndingRoundEventArgs ev)
        {
            bool mtfAlive = Extensions.CountRoles(Team.FoundationForces) > 0;
            bool ciAlive = Extensions.CountRoles(Team.ChaosInsurgency) > 0;
            bool scpAlive = Extensions.CountRoles(Team.SCPs) + Extensions.GetScp035s().Count > 0;
            bool dclassAlive = Extensions.CountRoles(Team.ClassD) > 0;
            bool scientistsAlive = Extensions.CountRoles(Team.Scientists) > 0;
            bool shAlive = API.GetSHPlayers().Count > 0;

            if (shAlive && ((ciAlive && !config.SerpentsHandModifiers.ScpsWinWithChaos) || dclassAlive || mtfAlive || scientistsAlive))
            {
                ev.IsRoundEnded = false;
            }
            else if (shAlive && scpAlive && !mtfAlive && !dclassAlive && !scientistsAlive)
            {
                if (!config.SerpentsHandModifiers.ScpsWinWithChaos)
                {
                    if (!ciAlive)
                    {
                        ev.LeadingTeam = LeadingTeam.Anomalies;
                        ev.IsRoundEnded = true;
                    }
                }
                else
                {
                    ev.LeadingTeam = LeadingTeam.Anomalies;
                    ev.IsRoundEnded = true;
                }
            }
        }
    }
}
