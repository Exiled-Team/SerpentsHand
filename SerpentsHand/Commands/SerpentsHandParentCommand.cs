using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using NorthwoodLib.Pools;
using RemoteAdmin;
using SerpentsHand.Commands.SubCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerpentsHand.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SerpentsHandParentCommand : ParentCommand
    {
        public SerpentsHandParentCommand() => LoadGeneratedCommands();

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
            Player player = Player.Get((sender as PlayerCommandSender).ReferenceHub);

            string message = "\nPlease enter a valid subcommand:\n";

            foreach (var command in AllCommands.ToList())
            {
                if (player.CheckPermission($"sh.{command.Command}"))
                    message += $"- {command.Command} ({command.Aliases[0]})\n";
            }

            response = message;
            return false;
        }
    }
}
