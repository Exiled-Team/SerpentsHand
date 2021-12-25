using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace SerpentsHand.Commands.Subcmds
{
    public class Spawn : ICommand
    {
        public string Command { get; } = "spawn";
        public string[] Aliases { get; } = { "s" };
        public string Description { get; } = "Makes player a Serpents Hand";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("sh.spawn"))
            {
                response = "You don't have permission to execute this command. Required permission: sh.spawn";
                return false;
            }

            if (arguments.Count == 0)
            {
                Player ply = Player.Get(sender);

                if (API.IsSerpent(ply))
                {
                    response = "You are already a Serpent's Hand!";
                    return false;
                }

                API.SpawnPlayer(ply);
                response = "You are now a Serpent's Hand.";
                return true;
            }

            Player player = Player.Get(arguments.At(0));
            if (player == null)
            {
                response = "Provided player is invalid. Please give player's id or nickname!";
                return false;
            }

            if (API.IsSerpent(player))
            {
                response = $"({player.Id}) {player.Nickname} is already a Serpent's Hand!";
                return false;
            }

            API.SpawnPlayer(player);
            response = $"({player.Id}) {player.Nickname} is now a Serpent's Hand.";
            return true;
        }
    }
}
