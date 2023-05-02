using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.CustomItems.API;
using MEC;
using PlayerRoles;
using Respawning;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SerpentsHand
{
    internal static class Extensions
    {
        private static Config config = SerpentsHand.Singleton.Config;
        private static SerpentsHand plugin = SerpentsHand.Singleton;

        public static List<Player> GetScp035s() => Player.List.Where(x => x.SessionVariables.ContainsKey("IsScp035")).ToList();
        public static int CountRoles(Team team) => Player.List.Count(x => x.Role.Team == team && !x.SessionVariables.ContainsKey("IsNPC"));

        public static void SpawnPlayer(Player player, bool full = true)
        {
            player.SessionVariables.Add("IsSH", null);
            player.Role.Set(RoleTypeId.Tutorial);
            player.Health = config.SerpentsHandModifiers.Health;
            player.MaxHealth = (int)config.SerpentsHandModifiers.Health;
            player.CustomInfo = config.SerpentsHandModifiers.RoleName;

            player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
            player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;

            player.Broadcast(config.SpawnManager.SpawnBroadcast);

            if (full)
            {
                Timing.CallDelayed(0.4f, () =>
                {
                    player.ResetInventory(config.SerpentsHandModifiers.SpawnItems);
                    foreach (var ammo in config.SerpentsHandModifiers.SpawnAmmo)
                        player.Ammo[ammo.Key.GetItemType()] = ammo.Value;
                });
            }
            Timing.CallDelayed(1.7f, () => player.Position = config.SpawnManager.SpawnPos);
        }

        public static void DestroySH(Player player)
        {
            player.SessionVariables.Remove("IsSH");
            player.MaxHealth = default;
            player.Health = default;
            player.CustomInfo = string.Empty;

            player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Nickname;
            player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
        }

        public static void SpawnSquad(uint size)
        {
            List<Player> spec = Player.List.Where(x => x.Role.Team == Team.Dead && !x.IsOverwatchEnabled).ToList();
            bool prioritySpawn = RespawnManager.Singleton._prioritySpawn;

            if (prioritySpawn)
                spec.OrderBy(x => (x.Role as SpectatorRole).DeathTime);

            int spawnCount = 1;
            while (spec.Count > 0 && spawnCount <= size)
            {
                int index = UnityEngine.Random.Range(0, spec.Count);
                if (spec[index] == null)
                    continue;

                SpawnPlayer(spec[index]);
                spec.RemoveAt(index);
                spawnCount++;
            }

            if (spawnCount > 0 && !string.IsNullOrEmpty(config.SpawnManager.EntryAnnoucement))
                Cassie.GlitchyMessage(config.SpawnManager.EntryAnnoucement, 0.05f, 0.05f);

            foreach (Player scp in Player.List.Where(x => x.Role.Team == Team.SCPs || x.SessionVariables.ContainsKey("IsScp035")))
                scp.Broadcast(config.SpawnManager.EntryBroadcast);
        }

        public static void SpawnSquad(List<Player> players)
        {
            foreach (Player player in players)
                SpawnPlayer(player);

            if (players.Count > 0)
                Cassie.GlitchyMessage(config.SpawnManager.EntryAnnoucement, 0.05f, 0.05f);

            foreach (Player scp in Player.List.Where(x => x.Role.Team == Team.SCPs || x.SessionVariables.ContainsKey("IsScp035")))
                scp.Broadcast(config.SpawnManager.EntryBroadcast);
        }

        public static Vector3 Get106Position()
        {
            Player scp106 = Player.List.FirstOrDefault(x => x.Role == RoleTypeId.Scp106);
            if (scp106 == null) return RoleTypeId.Scp106.GetRandomSpawnLocation().Position;
            return scp106.Position;
        }

        public static void CalculateChance()
        {
            int scp035num = 0;
            if (GetScp035s != null)
                scp035num = 1;

            plugin.IsSpawnable = UnityEngine.Random.Range(0, 101) <= config.SpawnManager.SpawnChance &&
                plugin.TeamRespawnCount >= config.SpawnManager.RespawnDelay &&
                plugin.SerpentsRespawnCount < config.SpawnManager.MaxSpawns &&
                !(!config.SpawnManager.CanSpawnWithoutScps && Player.Get(Team.SCPs).Count() + scp035num == 0);

            Log.Debug($"Is Serpent's Hand team now spawnable?: {plugin.IsSpawnable}");
        }
    }
}
