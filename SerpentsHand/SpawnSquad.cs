using Smod2;
using Smod2.Commands;
using Smod2.API;
using System;

namespace SerpentsHand
{
	class SpawnSquad : ICommandHandler
	{
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
					Plugin.SpawnSquad(a);
				}
				else
				{
					return new string[] { "Error: invalid size." };
				}
			}
			else
			{
				Plugin.SpawnSquad(5);
			}
			PluginManager.Manager.Server.Map.AnnounceCustomMessage(Plugin.shAnnouncement);
			return new string[] { "Spawned squad." };
		}
	}
}
