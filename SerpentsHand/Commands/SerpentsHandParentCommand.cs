namespace SerpentsHand.Commands
{
    using System;
    using System.Linq;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;
    using SubCommands;


    /// <summary>
    /// The base parent command.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SerpentsHandParentCommand : ParentCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerpentsHandParentCommand"/> class.
        /// </summary>
        public SerpentsHandParentCommand() => LoadGeneratedCommands();

        /// <inheritdoc/>
        public override string Command => "sh";

        /// <inheritdoc/>
        public override string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public override string Description => "Parent command for Serpents Hand";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new List());
            RegisterCommand(new Spawn());
            RegisterCommand(new SpawnTeam());
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get((sender as PlayerCommandSender).ReferenceHub);

            string message = "\nPlease enter a valid subcommand:\n";

            foreach (var command in AllCommands.ToList())
            {
                if (player.CheckPermission($"sh.{command.Command}"))
                {
                    message += $"- {command.Command} ({command.Aliases[0]})\n";
                }
            }

            response = message;
            return false;
        }
    }
}
