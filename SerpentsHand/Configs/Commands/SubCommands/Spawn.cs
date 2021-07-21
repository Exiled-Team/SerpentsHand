namespace SerpentsHand.Commands.SubCommands
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;

    /// <summary>
    /// A command which spawns a single Serpents Hand.
    /// </summary>
    public class Spawn : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "spawn";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "s" };

        /// <inheritdoc/>
        public string Description { get; } = "Makes player a Serpents Hand";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("sh.spawn"))
            {
                response = "You don't have permission to execute this command. Required permission: sh.spawn";
                return false;
            }

            if (arguments.Count == 0)
            {
                Player player = Player.Get((sender as PlayerCommandSender).ReferenceHub);
                if (API.IsSerpent(player))
                {
                    response = "You are already a Serpent's Hand.";
                    return false;
                }

                API.SpawnPlayer(player);
                response = "You are now a Serpent's Hand.";
                return true;
            }
            else
            {
                Player player = Player.Get(arguments.At(0));
                if (player == null)
                {
                    response = "Provided player is invalid. Please give player's id or nickname.";
                    return false;
                }

                if (API.IsSerpent(player))
                {
                    response = $"{player.Nickname} is already a Serpent's Hand.";
                    return false;
                }

                API.SpawnPlayer(player);

                response = $"{player.Nickname} is now a Serpent's Hand.";
                return true;
            }
        }
    }
}
