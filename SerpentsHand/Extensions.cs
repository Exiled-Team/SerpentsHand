using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.CustomItems.API;
using MEC;
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
        public static int CountRoles(Team team) => Player.List.Where(x => x.Team == team && !x.SessionVariables.ContainsKey("IsNPC")).Count();

        public static void SpawnPlayer(Player player, bool full = true)
        {
            player.SessionVariables.Add("IsSH", null);
            player.Role = RoleType.Tutorial;
            player.Health = config.SerpentsHandModifiers.Health;
            player.MaxHealth = (int)config.SerpentsHandModifiers.Health;
            player.UnitName = config.SpawnManager.UnitNames[UnityEngine.Random.Range(0, config.SpawnManager.UnitNames.Count)];
            player.CustomInfo = config.SerpentsHandModifiers.RoleName;

            player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
            player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.CustomInfo;
            player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;
            player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.UnitName;

            player.Broadcast(config.SpawnManager.SpawnBroadcast);

            if (full)
            {
                Timing.CallDelayed(0.3f, () =>
                {
                    player.ResetInventory(config.SerpentsHandModifiers.SpawnItems);
                    foreach (var ammo in config.SerpentsHandModifiers.SpawnAmmo)
                        player.Ammo[ammo.Key.GetItemType()] = ammo.Value;
                });
            }
            Timing.CallDelayed(0.9f, () => player.Position = config.SpawnManager.SpawnPos);
        }

        public static void DestroySH(Player player)
        {
            player.SessionVariables.Remove("IsSH");
            player.MaxHealth = default;
            player.Health = default;
            player.CustomInfo = string.Empty;
            player.UnitName = string.Empty;

            player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Nickname;
            player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.CustomInfo;
            player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
            player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.UnitName;
        }

        public static void SpawnSquad(uint size)
        {
            List<Player> spec = Player.List.Where(x => x.Team == Team.RIP && !x.IsOverwatchEnabled).ToList();
            bool prioritySpawn = RespawnManager.Singleton._prioritySpawn;

            if (prioritySpawn)
                spec.OrderBy(x => x.ReferenceHub.characterClassManager.DeathTime);

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

            foreach (Player scp in Player.List.Where(x => x.Team == Team.SCP || x.SessionVariables.ContainsKey("IsScp035")))
                scp.Broadcast(config.SpawnManager.EntryBroadcast);
        }

        public static void SpawnSquad(List<Player> players)
        {
            foreach (Player player in players)
                SpawnPlayer(player);

            if (players.Count > 0)
                Cassie.GlitchyMessage(config.SpawnManager.EntryAnnoucement, 0.05f, 0.05f);

            foreach (Player scp in Player.List.Where(x => x.Team == Team.SCP || x.SessionVariables.ContainsKey("IsScp035")))
                scp.Broadcast(config.SpawnManager.EntryBroadcast);
        }

        public static Vector3 Get106Position()
        {
            Player scp106 = Player.List.FirstOrDefault(x => x.Role == RoleType.Scp106);
            if (scp106 == null) return RoleType.Scp096.GetRandomSpawnProperties().Item1;
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
                !(!config.SpawnManager.CanSpawnWithoutScps && Player.Get(Team.SCP).Count() + scp035num == 0);

            Log.Debug($"Is Serpent's Hand team now spawnable?: {plugin.IsSpawnable}", config.Debug);
        }
    }
}
