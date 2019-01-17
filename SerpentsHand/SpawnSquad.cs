using Smod2;
using Smod2.Commands;
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
				if (int.TryParse(args[0], out int a))
				{
					SHPlugin.SpawnSquad(a);
				}
				else
				{
					return new string[] { "Error: invalid size." };
				}
			}
			else
			{
				SHPlugin.SpawnSquad(5);
			}
			PluginManager.Manager.Server.Map.AnnounceCustomMessage(SHPlugin.shAnnouncement);
			return new string[] { "Spawned squad." };
		}
	}
}
