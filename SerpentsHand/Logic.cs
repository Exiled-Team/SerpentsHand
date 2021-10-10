namespace SerpentsHand
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using MEC;
    using Respawning;
    using UnityEngine;

    /// <summary>
    /// EventHandlers and Logic which Serpents Hand use.
    /// </summary>
    public partial class EventHandlers
    {
        public static List<Player> GetScp035s()
        {
            return Player.List.Where(x => x.SessionVariables.ContainsKey("IsScp035")).ToList();
        }

        public static int CountRoles(Team team)
        {
            List<Player> scp035 = GetScp035s();

            int count = 0;
            foreach (Player player in Player.List)
            {
                if (player.SessionVariables.ContainsKey("IsNPC"))
                    continue;

                if (player.Team == team)
                {
                    if (scp035 != null && scp035.Contains(player))
                        continue;

                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Spawns <see cref="Player"/> as Serpents Hand.
        /// </summary>
        /// <param name="player"> The player to spawn.</param>
        /// <param name="full"> Should items and ammo be given to spawned <see cref="Player"/>.</param>
        internal static void SpawnPlayer(Player player, bool full = true)
        {
            player.SessionVariables.Add("IsSH", null);
            player.Role = RoleType.Tutorial;
            player.Broadcast(Config.SpawnManager.SpawnBroadcast);

            if (full)
            {
                Timing.CallDelayed(0.3f, () =>
                {
                    player.ResetInventory(Config.SerepentsHandModifiers.SpawnItems);

                    foreach (var ammo in Config.SerepentsHandModifiers.SpawnAmmo)
                    {
                        player.Ammo[ammo.Key.GetItemType()] = ammo.Value;
                    }
                });
            }

            player.Health = Config.SerepentsHandModifiers.Health;

            string roleName = string.Empty;

            if (!string.IsNullOrEmpty(Config.SerepentsHandModifiers.RoleColor))
                roleName += $"<color={Config.SerepentsHandModifiers.RoleColor}>";

            roleName += $"{player.Nickname}\n{Config.SerepentsHandModifiers.RoleName}";

            if (!string.IsNullOrEmpty(Config.SerepentsHandModifiers.RoleColor))
                roleName += "</color>";

            player.CustomInfo = roleName;
            player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
            player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;

            Timing.CallDelayed(1.5f, () => player.Position = Config.SpawnManager.SpawnPos);
        }

        /// <summary>
        /// Removes Serpents Hand role name and id from a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to remove.</param>
        internal static void DestroySH(Player player)
        {
            player.SessionVariables.Remove("IsSH");

            player.MaxHealth = default;

            player.CustomInfo = string.Empty;
            player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Nickname;
            player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
        }

        /// <summary>
        /// Spawns Serpents Hand squad.
        /// </summary>
        /// <param name="size"> The number of players in squad (this can be lower due to not enough Spectators).</param>
        internal static void SpawnSquad(uint size)
        {
            List<Player> spec = Player.List.Where(x => x.Team == Team.RIP && !x.IsOverwatchEnabled).ToList();

            bool prioritySpawn = RespawnManager.Singleton._prioritySpawn;

            if (prioritySpawn)
                spec.OrderBy((x) => x.ReferenceHub.characterClassManager.DeathTime);

            int spawnCount = 1;
            while (spec.Count > 0 && spawnCount <= size)
            {
                int index = Random.Range(0, spec.Count);
                if (spec[index] != null)
                {
                    Player player = prioritySpawn ? spec.First() : spec[Random.Range(0, spec.Count)];
                    SpawnPlayer(spec[index]);
                    spec.RemoveAt(index);
                    spawnCount++;
                }
            }

            if (spawnCount > 0 && !string.IsNullOrEmpty(Config.SpawnManager.EntryAnnouncement))
                Cassie.GlitchyMessage(Config.SpawnManager.EntryAnnouncement, 0.05f, 0.05f);

            foreach (Player scp in Player.List.Where(x => x.Team == Team.SCP || x.SessionVariables.ContainsKey("Is035")))
            {
                scp.Broadcast(Config.SpawnManager.EntryBroadcast);
            }
        }

        /// <summary>
        /// Spawns Serpents Hand squad.
        /// </summary>
        /// <param name="players"> List of players to spawn.</param>
        internal static void SpawnSquad(List<Player> players)
        {
            foreach (Player player in players)
            {
                SpawnPlayer(player);
            }

            if (players.Count > 0)
                Cassie.GlitchyMessage(Config.SpawnManager.EntryAnnouncement, 0.05f, 0.05f);

            foreach (Player scp in Player.List.Where(x => x.Team == Team.SCP || x.SessionVariables.ContainsKey("Is035")))
            {
                scp.Broadcast(Config.SpawnManager.EntryBroadcast);
            }
        }

        private static Vector3 Get106Position()
        {
            Player scp106 = Player.List.FirstOrDefault(x => x.Role == RoleType.Scp106);

            if (scp106 != null)
            {
                return scp106.Position;
            }
            else
            {
                return RoleType.Scp096.GetRandomSpawnProperties().Item1;
            }
        }
    }
}