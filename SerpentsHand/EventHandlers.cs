using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using Exiled.Loader;
using PlayerRoles;
using Respawning;
using System.Collections.Generic;
using System.Linq;

namespace SerpentsHand
{
    internal sealed class EventHandlers
    {
        private Plugin plugin;
        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        private int Respawns = 0;
        private int SHRespawns = 0;

        public void OnRoundStarted()
        {
            Respawns = 0;
            SHRespawns = 0;
        }

        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (Loader.Random.Next(100) <= plugin.Config.SerpentsHand.SpawnChance &&
                Respawns >= plugin.Config.SerpentsHand.RespawnDelay &&
                SHRespawns < plugin.Config.SerpentsHand.MaxSpawns &&
                ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency)
            {
                bool scpAlive = Player.List.Count(x => x.Role.Team == Team.SCPs) > 0;
                if (!scpAlive && !plugin.Config.SerpentsHand.CanSpawnWithoutScps)
                    return;

                List<Player> players = new List<Player>();
                if (ev.Players.Count > plugin.Config.SerpentsHand.MaxSquad)
                    players = ev.Players.GetRange(0, plugin.Config.SerpentsHand.MaxSquad);
                else
                    players = ev.Players.GetRange(0, ev.Players.Count);

                foreach (Player player in ev.Players)
                {
                    if (player is null)
                        continue;
                    plugin.Config.SerpentsHand.AddRole(player);
                }
                SHRespawns++;

                if (!string.IsNullOrEmpty(plugin.Config.SerpentsHand.EntryAnnoucement))
                    Cassie.GlitchyMessage(plugin.Config.SerpentsHand.EntryAnnoucement, 0.05f, 0.05f);

                if (plugin.Config.SerpentsHand.EntryBroadcast.Duration > 0 || !string.IsNullOrEmpty(plugin.Config.SerpentsHand.EntryBroadcast.Content))
                    foreach (Player player in Player.List.Where(x => x.Role.Team == Team.SCPs))
                        player.Broadcast(plugin.Config.SerpentsHand.EntryBroadcast);

                ev.IsAllowed = false;
                ev.NextKnownTeam = SpawnableTeamType.None;
            }

            Respawns++;
        }

        public void OnEndingRound(EndingRoundEventArgs ev)
        {
            bool mtfAlive = false;
            bool ciAlive = false;
            bool scpAlive = false;
            bool dclassAlive = false;
            bool scientistsAlive = false;
            bool shAlive = plugin.Config.SerpentsHand.TrackedPlayers.Count > 0;

            foreach (Player player in Player.List)
            {
                switch (player.Role.Team)
                {
                    case Team.FoundationForces:
                        mtfAlive = true;
                        break;
                    case Team.ChaosInsurgency:
                        ciAlive = true;
                        break;
                    case Team.SCPs:
                        scpAlive = true;
                        break;
                    case Team.ClassD:
                        dclassAlive = true;
                        break;
                    case Team.Scientists:
                        scientistsAlive = true;
                        break;
                }
            }

            if (shAlive && ((ciAlive && !plugin.Config.SerpentsHand.ScpsWinWithChaos) || dclassAlive || mtfAlive || scientistsAlive))
                ev.IsRoundEnded = false;
            else if (shAlive && scpAlive && !mtfAlive && !dclassAlive && !scientistsAlive)
            {
                if (!plugin.Config.SerpentsHand.ScpsWinWithChaos)
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
            else if ((shAlive || scpAlive) && ciAlive && !plugin.Config.SerpentsHand.ScpsWinWithChaos)
                ev.IsRoundEnded = false;
        }
    }
}
