namespace SerpentsHand
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;

    /// <summary>
    /// The Serpents Hand API with <see langword="static"/> methods.
    /// </summary>
    public static class API
    {
        /// <summary>
        /// Checks if <see cref="Player"/> is Serpents Hand.
        /// </summary>
        /// <param name="player"> The player to check.</param>
        /// <returns><see langword="true"/> if player is Serpents Hand, <see langword="false"/> if not.</returns>
        public static bool IsSerpent(Player player) => player.SessionVariables.ContainsKey("IsSH");

        /// <summary>
        /// Spawns <see cref="Player"/> as Serpents Hand.
        /// </summary>
        /// <param name="player"> The player to spawn.</param>
        /// <param name="full"> Should items and ammo be given to spawned <see cref="Player"/>.</param>
        public static void SpawnPlayer(Player player, bool full = true)
        {
            EventHandlers.SpawnPlayer(player, full);
        }

        /// <summary>
        /// Spawns Serpents Hand squad.
        /// </summary>
        /// <param name="playerList"> List of players to spawn.</param>
        public static void SpawnSquad(List<Player> playerList)
        {
            EventHandlers.SpawnSquad(playerList);
        }

        /// <summary>
        /// Spawns Serpents Hand squad.
        /// </summary>
        /// <param name="size"> The number of players in squad (this can be lower due to not enough number of Spectators).</param>
        public static void SpawnSquad(uint size)
        {
            EventHandlers.SpawnSquad(size);
        }

        /// <summary>
        /// Gets all alive Serpents Hand players.
        /// </summary>
        /// <returns><see cref="List{Player}"/> of all alive Serpents Hand players.</returns>
        public static List<Player> GetSHPlayers()
        {
            return Player.List.Where(x => x.SessionVariables.ContainsKey("IsSH")).ToList();
        }
    }
}
