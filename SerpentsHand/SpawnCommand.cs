using Smod2.Commands;
using Smod2.API;

namespace SerpentsHand
{
	class SpawnCommand : ICommandHandler
	{
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
				Player cPlayer = Plugin.GetPlayer(args[0], out cPlayer);
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

				Player tPlayer = Plugin.FindPlayer(id);
				if (tPlayer != null)
				{
					Plugin.SpawnPlayer(tPlayer);
					return new string[] { "Spawned player \"" + tPlayer.Name + "\" as Serpent's Hand." };
				}
			}
			return new string[] { GetUsage() };
		}
	}
}
