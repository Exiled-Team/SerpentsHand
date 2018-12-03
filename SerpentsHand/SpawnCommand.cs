using Smod2;
using Smod2.Commands;
using Smod2.API;

namespace SerpentsHand
{
	class SpawnCommand : ICommandHandler
	{
		private Plugin plugin;

		public SpawnCommand(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Spawns a player as Serpents Hand.";
		}

		public string GetUsage()
		{
			return "(SPAWNSH) (PLAYER NAME / STEAMID64)";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (args.Length > 0)
			{
				string id = "";
				Player cPlayer = SerpentsHand.GetPlayer(args[0], out cPlayer);
				if (cPlayer != null)
				{
					id = cPlayer.SteamId;
				}
				else if (ulong.TryParse(args[0], out ulong a))
				{
					id = a.ToString();
				}
				else
				{
					return new string[] { "Error: invalid player id." };
				}

				Player tPlayer = SerpentsHand.FindPlayer(id);
				if (tPlayer != null)
				{
					SerpentsHand.SpawnPlayer(tPlayer);
					return new string[] { "Spawned player \"" + tPlayer.Name + "\" as Serpent's Hand." };
				}
			}
			return new string[] { GetUsage() };
		}
	}
}
