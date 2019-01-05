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
	version = "0.5.1",
	SmodMajor = 3,
	SmodMinor = 0,
	SmodRevision = 0
	)]
	public class Plugin : Smod2.Plugin
    {
		public static Smod2.Plugin instance;

		public static System.Random rand = new System.Random();

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

		public override void OnEnable() {}

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

		public static int LevenshteinDistance(string s, string t)
		{
			int n = s.Length;
			int m = t.Length;
			int[,] d = new int[n + 1, m + 1];

			if (n == 0)
			{
				return m;
			}

			if (m == 0)
			{
				return n;
			}

			for (int i = 0; i <= n; d[i, 0] = i++)
			{
			}

			for (int j = 0; j <= m; d[0, j] = j++)
			{
			}

			for (int i = 1; i <= n; i++)
			{
				for (int j = 1; j <= m; j++)
				{
					int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

					d[i, j] = Math.Min(
						Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
						d[i - 1, j - 1] + cost);
				}
			}
			return d[n, m];
		}

		public static Player GetPlayer(string args, out Player playerOut)
		{
			int maxNameLength = 31, LastnameDifference = 31;
			Player plyer = null;
			string str1 = args.ToLower();
			foreach (Player pl in PluginManager.Manager.Server.GetPlayers(str1))
			{
				if (!pl.Name.ToLower().Contains(args.ToLower())) { goto NoPlayer; }
				if (str1.Length < maxNameLength)
				{
					int x = maxNameLength - str1.Length;
					int y = maxNameLength - pl.Name.Length;
					string str2 = pl.Name;
					for (int i = 0; i < x; i++)
					{
						str1 += "z";
					}
					for (int i = 0; i < y; i++)
					{
						str2 += "z";
					}
					int nameDifference = LevenshteinDistance(str1, str2);
					if (nameDifference < LastnameDifference)
					{
						LastnameDifference = nameDifference;
						plyer = pl;
					}
				}
				NoPlayer:;
			}
			playerOut = plyer;
			return playerOut;
		}

		public static Player FindPlayer(string steamid)
		{
			foreach (Player player in PluginManager.Manager.Server.GetPlayers())
				if (player.SteamId == steamid)
					return player;
			return null;
		}

		public static void TeleportTo106(Player Player)
		{
			foreach (Player player in PluginManager.Manager.Server.GetPlayers().Where(x => x.TeamRole.Role == Role.SCP_106))
			{
				Timing.Next(() =>
				{
					Player.Teleport(player.GetPosition());
				});
				break;
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

			foreach (Smod2.API.Item item in player.GetInventory())
				item.Remove();

			player.GiveItem(ItemType.E11_STANDARD_RIFLE);
			player.GiveItem(ItemType.CHAOS_INSURGENCY_DEVICE);
			player.GiveItem(ItemType.RADIO);
			player.GiveItem(ItemType.FLASHBANG);
			player.GiveItem(ItemType.MEDKIT);
		}

		public static void SpawnSHSquad(List<Player> Playerlist)
		{
			List<Player> SHPlayers = new List<Player>();
			List<Player> CIPlayers = Playerlist;
			for (int i = 0; i < shMaxSquad; i++)
			{
				Player player = CIPlayers[rand.Next(CIPlayers.Count)];
				SHPlayers.Add(player);
				CIPlayers.Remove(player);
			}

			foreach (Player player in SHPlayers)
				SpawnPlayer(player);
			foreach (Player player in CIPlayers)
				player.ChangeRole(Role.SPECTATOR);

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
