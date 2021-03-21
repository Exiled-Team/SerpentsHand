namespace SerpentsHand.Configs.SubConfigs
{
    using System.ComponentModel;

    /// <summary>
    /// Configs for Serpents Hand spawning options.
    /// </summary>
    public class SpawnManager
    {
        /// <summary>
        /// Gets
        /// </summary>
        [Description("The change for Serpents Hand to spawn instead of Chaos.")]
        public int SpawnChance { get; private set; } = 50;

        /// <summary>
        /// Gets
        /// </summary>
        [Description("The maximum size of a Serpents Hand squad.")]
        public int MaxSquad { get; private set; } = 8;

        /// <summary>
        /// Gets
        /// </summary>
        [Description("How many respawn waves must occur before considering Serpents Hand to spawn.")]
        public int RespawnDelay { get; private set; } = 1;

        /// <summary>
        /// Gets
        /// </summary>
        [Description("The maxium number of times Serpents can spawn per game.")]
        public int MaxSpawns { get; private set; } = 1;

        /// <summary>
        /// Gets
        /// </summary>
        [Description("The message announced by CASSIE when Serpents hand spawn.")]
        public string EntryAnnouncement { get; private set; } = "SERPENTS HAND HASENTERED";

        /// <summary>
        /// Gets
        /// </summary>
        [Description("The message announced by CASSIE when Chaos spawn.")]
        public string CiEntryAnnouncement { get; private set; } = "";

        /// <summary>
        /// Gets
        /// </summary>
        [Description("The broadcast sent to Serpents Hand when they spawn.")]
        public string SpawnBroadcast { get; private set; } = "<size=60>You are <color=#03F555><b>Serpents Hand</b></color></size>\n<i>Help the <color=\"red\">SCPs</color> by killing all other classes!</i>";
    }
}
