using System.Collections.Generic;

namespace SerpentsHand
{
    internal static class Configs
    {
		internal static List<int> spawnItems;

		internal static int spawnChance;
		internal static int health;
		internal static int maxSquad;
		internal static int respawnDelay;

		internal static string entryAnnouncement;
		internal static string ciEntryAnnouncement;

		internal static bool friendlyFire;
		internal static bool teleportTo106;
		internal static bool scpsWinWithChaos;

        internal static void ReloadConfigs()
        {
			spawnItems = Plugin.Config.GetIntList("sh_spawn_items");
            if (spawnItems == null || spawnItems.Count == 0)
            {
				spawnItems = new List<int>() { 21, 26, 12, 14, 10 };
            }

			spawnChance = Plugin.Config.GetInt("sh_spawn_chance", 50);
			health = Plugin.Config.GetInt("sh_health", 120);
			maxSquad = Plugin.Config.GetInt("sh_max_squad", 8);
			respawnDelay = Plugin.Config.GetInt("sh_team_respawn_delay", 1);

			entryAnnouncement = Plugin.Config.GetString("sh_entry_announcement", "SERPENTS HAND HASENTERED");
			ciEntryAnnouncement = Plugin.Config.GetString("sh_ci_entry_announcement", "");

			friendlyFire = Plugin.Config.GetBool("sh_friendly_fire", false);
			teleportTo106 = Plugin.Config.GetBool("sh_teleport_to_106", true);
			scpsWinWithChaos = Plugin.Config.GetBool("sh_scps_win_with_chaos", true);
        }
    }
}
