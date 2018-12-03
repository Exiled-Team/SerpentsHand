using Smod2;
using Smod2.Commands;
using Smod2.API;
using System;

namespace SerpentsHand
{
	class SpawnSquad : ICommandHandler
	{
		private Plugin plugin;

		public SpawnSquad(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Spawns a Serpent's Hand squad.";
		}

		public string GetUsage()
		{
			return "(SPAWNSHSQUAD) (SIZE)";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (args.Length > 0)
			{
				if (Int32.TryParse(args[0], out int a))
				{
					SerpentsHand.SpawnSquad(a);
				}
				else
				{
					return new string[] { "Error: invalid size." };
				}
			}
			else
			{
				SerpentsHand.SpawnSquad(5);
			}
			plugin.pluginManager.Server.Map.AnnounceCustomMessage(plugin.GetConfigString("sh_entry_announcement"));
			return new string[] { "Spawned squad." };
		}
	}
}
