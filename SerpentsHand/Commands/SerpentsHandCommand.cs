using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using SerpentsHand.Commands.Subcmds;

namespace SerpentsHand.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class SerpentsHandCommand : ParentCommand
	{
		public SerpentsHandCommand() => LoadGeneratedCommands();

		public override string Command => "sh";
		public override string[] Aliases => Array.Empty<string>();
		public override string Description => "Parent command for Serpents Hand";

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new List());
			RegisterCommand(new Spawn());
			RegisterCommand(new SpawnTeam());
		}

		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			Player player = Player.Get(sender);
			response = "\nPlease enter a valid subcommand:\n";
			foreach (var command in AllCommands)
				if (player.CheckPermission($"sh.{command.Command}"))
					response += $"- {command.Command} ({string.Join(", ", Aliases)})";
			return false;
		}
	}
}
