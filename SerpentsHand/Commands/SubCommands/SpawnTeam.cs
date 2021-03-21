using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace SerpentsHand.Commands.SubCommands
{
    /// <summary>
    /// A command which spawns a Serpents Hand team.
    /// </summary>
    public class SpawnTeam : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "spawnteam";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "st" };

        /// <inheritdoc/>
        public string Description { get; } = "Spawns Serpents Hand team.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("sh.spawnteam"))
            {
                response = "You don't have permission to execute this command. Required permission: sh.spawnteam";
                return false;
            }

            if (arguments.Count == 0)
            {
                API.SpawnSquad(SerpentsHand.instance.Config.SpawnManager.MaxSquad);
            }
            else
            {
                if (int.TryParse(arguments.At(0), out int num))
                {
                    response = $"\"{arguments.At(0)}\" is not a valid number.";
                    return false;
                }

                API.SpawnSquad(num);
            }

            response = "Serpent's Hand squad has been spawned.";
            return true;
        }
    }
}
