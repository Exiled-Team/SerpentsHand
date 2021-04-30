namespace SerpentsHand.Commands.SubCommands
{
    using System;
    using System.Linq;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

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

            uint validPlayers = 0;
            foreach (Player player in Player.List.Where(x => x.Team == Team.RIP && !x.IsOverwatchEnabled))
            {
                validPlayers++;
            }

            if (arguments.Count == 0)
            {
                uint maxSquad = SerpentsHand.Instance.Config.SpawnManager.MaxSquad;

                if (validPlayers >= maxSquad)
                {
                    API.SpawnSquad(SerpentsHand.Instance.Config.SpawnManager.MaxSquad);

                    response = $"Serpent's Hand squad with {maxSquad} players has been spawned.";
                    return true;
                }
                else
                {
                    response = $"There is not enough Spectators to spawn Serpent's Hand squad with {maxSquad} players. Required {maxSquad - validPlayers} more.";
                    return false;
                }
            }
            else
            {
                if (!uint.TryParse(arguments.At(0), out uint num) || num == 0)
                {
                    response = $"\"{arguments.At(0)}\" is not a valid number.";
                    return false;
                }

                if (validPlayers >= num)
                {
                    API.SpawnSquad(num);

                    response = $"Serpent's Hand squad with {num} players has been spawned.";
                    return true;
                }
                else
                {
                    response = $"There is not enough Spectators to spawn Serpent's Hand squad with {num} players. Required {num - validPlayers} more.";
                    return false;
                }
            }
        }
    }
}
