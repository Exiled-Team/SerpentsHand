using Smod2;
using Smod2.Attributes;
using System;
using Smod2.API;
using System.Collections.Generic;
using System.Threading;

namespace SerpentsHand
{
	[PluginDetails(
	author = "Cyanox",
	name = "Serpents Hand",
	description = "A new class for SCP:SL",
	id = "cyan.serpents.hand",
	version = "0.1",
	SmodMajor = 3,
	SmodMinor = 0,
	SmodRevision = 0
	)]
	public class SerpentsHand : Plugin
    {
		private static Vector shSpawnPos = new Vector(0, 1001, 8);
		private static Plugin plugin;
		public static List<string> shPlayers = new List<string>();
		public static List<int> shItemList = new List<int>();
		public static List<string> shPlayersInPocket = new List<string>();

		public override void OnEnable() {}

		public override void OnDisable() {}

		public override void Register()
		{
			plugin = this;

			AddEventHandlers(new EventHandler(this));

			AddConfig(new Smod2.Config.ConfigSetting("sh_spawn_chance", 50, Smod2.Config.SettingType.NUMERIC, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_entry_announcement", "serpents hand entered", Smod2.Config.SettingType.STRING, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_spawn_items", "20,26,12,14,10", Smod2.Config.SettingType.STRING, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_ci_entry_announcement", "", Smod2.Config.SettingType.STRING, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_friendly_fire", false, Smod2.Config.SettingType.BOOL, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_teleport_to_106", true, Smod2.Config.SettingType.BOOL, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_ci_win_with_scp", false, Smod2.Config.SettingType.BOOL, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_health", 120, Smod2.Config.SettingType.NUMERIC, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_max_squad", 8, Smod2.Config.SettingType.NUMERIC, true, ""));

			AddCommands(new string[] { "spawnsh" }, new SpawnCommand(this));
			AddCommands(new string[] { "spawnshsquad" }, new SpawnSquad(this));
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

		public static int GetItemCount(Player player)
		{
			int count = 0;
			foreach (Item item in player.GetInventory())
				count++;
			return count;
		}

		public static void SpawnPlayer(Player player)
		{
			player.ChangeRole(Role.TUTORIAL);
			player.SetAmmo(AmmoType.DROPPED_5, 250);
			player.SetAmmo(AmmoType.DROPPED_7, 250);
			player.SetAmmo(AmmoType.DROPPED_9, 250);
			player.SetHealth(plugin.GetConfigInt("sh_health"));
			player.Teleport(shSpawnPos);

			foreach (Item item in player.GetInventory())
				item.Remove();
			foreach (int a in shItemList)
			{
				if (GetItemCount(player) <= shItemList.Count)
					player.GiveItem((ItemType)a);
			}

			shPlayers.Add(player.SteamId);
		}

		public static int CountRoles(Role role)
		{
			int count = 0;
			foreach (Player pl in PluginManager.Manager.Server.GetPlayers())
				if (pl.TeamRole.Role == role)
					count++;
			return count;
		}

		public static int CountRoles(Team team)
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
			Random rand = new Random();

			foreach (Player player in PluginManager.Manager.Server.GetPlayers())
				if (player.TeamRole.Team == Team.SPECTATOR)
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

		public static void TeleportTo106(Player Player)
		{
			foreach (Player player in PluginManager.Manager.Server.GetPlayers())
			{
				if (player.TeamRole.Role == Role.SCP_106)
				{
					Thread TeleportDelay = new Thread(new ThreadStart(() => new TeleportDelay(Player, player, 100)));
					TeleportDelay.Start();
					break;
				}
			}
		}
	}
}
