using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
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
                List<Player> players = new List<Player>();
                if (ev.Players.Count > plugin.Config.SerpentsHand.MaxSquad)
                    players = ev.Players.GetRange(0, plugin.Config.SerpentsHand.MaxSquad);
                else
                    players = ev.Players.GetRange(0, ev.Players.Count);

                foreach(Player player in ev.Players)
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
    }
}
