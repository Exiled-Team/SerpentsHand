namespace SerpentsHand.Commands.SubCommands
{
    using System;
    using System.Collections.Generic;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// A command which shows a list of players that are currently a Serpents Hand.
    /// </summary>
    public class List : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "list";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "l" };

        /// <inheritdoc/>
        public string Description { get; } = "Shows a list with players that are Serpents Hand.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("sh.list"))
            {
                response = "You don't have permission to execute this command. Required permission: sh.list";
                return false;
            }

            string message = "\nList of players that are Serpent's Hand:\n";

            List<Player> shPlayers = API.GetSHPlayers();

            foreach (var shPly in shPlayers)
            {
                message += $"- ({shPly.Id}) {shPly.Nickname}\n";
            }

            response = message;
            return true;
        }
    }
}
