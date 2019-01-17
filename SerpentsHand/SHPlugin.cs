using Smod2;
using Smod2.Attributes;
using System;
using Smod2.API;
using System.Collections.Generic;
using scp4aiur;
using System.Linq;

namespace SerpentsHand
{
	[PluginDetails(
	author = "Cyanox",
	name = "Serpents Hand",
	description = "A new class for SCP:SL",
	id = "cyan.serpents.hand",
	version = "0.6",
	SmodMajor = 3,
	SmodMinor = 0,
	SmodRevision = 0
	)]
	public class SHPlugin : Plugin
    {
		public static SHPlugin instance;

		public static Random rand = new Random();

		public static List<string> shPlayersInPocket = new List<string>();
		public static List<string> shPlayers = new List<string>();
		public static List<int> shItemList = new List<int>();

		private static Vector shSpawnPos = new Vector(0, 1001, 8);

		public static string ciAnnouncement;
		public static string shAnnouncement;

		public static int spawnChance;
		public static int shMaxSquad;
		public static int shHealth;

		public static bool friendlyFire;
		public static bool ciWinWithSCP;
		public static bool teleportTo106;

		public override void OnEnable() => Info(Details.name + " Activated!");

		public override void OnDisable() {}

		public override void Register()
		{
			instance = this;

			AddEventHandlers(new EventHandler());

			Timing.Init(this);

			AddConfig(new Smod2.Config.ConfigSetting("sh_spawn_chance", 50, Smod2.Config.SettingType.NUMERIC, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_entry_announcement", "serpents hand entered", Smod2.Config.SettingType.STRING, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_spawn_items", new[] 
			{
				20,
				26,
				12,
				14,
				10
			}, Smod2.Config.SettingType.NUMERIC_LIST, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_ci_entry_announcement", "", Smod2.Config.SettingType.STRING, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_friendly_fire", false, Smod2.Config.SettingType.BOOL, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_teleport_to_106", true, Smod2.Config.SettingType.BOOL, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_ci_win_with_scp", false, Smod2.Config.SettingType.BOOL, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_health", 120, Smod2.Config.SettingType.NUMERIC, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_max_squad", 8, Smod2.Config.SettingType.NUMERIC, true, ""));

			AddCommands(new string[] { "spawnsh" }, new SpawnCommand());
			AddCommands(new string[] { "spawnshsquad" }, new SpawnSquad());
		}

		public static Player FindPlayer(string identifier)
		{
			return PluginManager.Manager.Server.GetPlayers(identifier).FirstOrDefault();
		}

		public static void TeleportTo106(Player Player)
		{
			Player player = PluginManager.Manager.Server.GetPlayers().Where(x => x.TeamRole.Role == Role.SCP_106).FirstOrDefault();
			if (player != null) {
				Timing.Next(() =>
				{
					Player.Teleport(player.GetPosition());
				});
			}
		}

		public static void SpawnPlayer(Player player)
		{
			shPlayers.Add(player.SteamId);
			player.ChangeRole(Role.TUTORIAL, false);
			player.SetAmmo(AmmoType.DROPPED_5, 250);
			player.SetAmmo(AmmoType.DROPPED_7, 250);
			player.SetAmmo(AmmoType.DROPPED_9, 250);
			player.SetHealth(shHealth);
			player.Teleport(shSpawnPos);
		}

		public static void SpawnSHSquad(List<Player> Playerlist)
		{ 
			foreach (Player player in Playerlist)
				SpawnPlayer(player);

			PluginManager.Manager.Server.Map.AnnounceCustomMessage(shAnnouncement);
		}

		public static int CountRoles(Role role)
		{
			int count = 0;
			foreach (Player pl in PluginManager.Manager.Server.GetPlayers())
				if (pl.TeamRole.Role == role)
					count++;
			return count;
		}

		public static int CountRoles(Smod2.API.Team team)
		{
			int count = 0;
			foreach (Player pl in PluginManager.Manager.Server.GetPlayers())
				if (pl.TeamRole.Team == team)
					count++;
			return count;
		}

		public static void SpawnSquad(int size)
		{
			List<Player> spec = new List<Player>();
			List<Player> PlayerList = PluginManager.Manager.Server.GetPlayers();

			foreach (Player player in PluginManager.Manager.Server.GetPlayers())
				if (player.TeamRole.Team == Smod2.API.Team.SPECTATOR)
					spec.Add(player);

			int spawnCount = 1;
			while (spec.Count > 0 && spawnCount <= size)
			{
				int index = rand.Next(0, spec.Count);
				if (spec[index] != null)
				{
					SpawnPlayer(spec[index]);
					spec.RemoveAt(index);
					spawnCount++;
				}
			}
		}

		public static int GetItemCount(Player player)
		{
			int count = 0;
			foreach (Smod2.API.Item item in player.GetInventory())
				count++;
			return count;
		}
	}
}
