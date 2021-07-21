namespace SerpentsHand.Configs.SubConfigs
{
    using System.ComponentModel;
    using UnityEngine;

    /// <summary>
    /// Configs for Serpents Hand spawning options.
    /// </summary>
    public class SpawnManager
    {
        /// <summary>
        /// Gets the chance for Serpents Hand to spawn instead of Chaos.
        /// </summary>
        [Description("The chance for Serpents Hand to spawn instead of Chaos.")]
        public int SpawnChance { get; private set; } = 50;

        /// <summary>
        /// Gets the maximum size of a Serpents Hand squad.
        /// </summary>
        [Description("The maximum size of a Serpents Hand squad.")]
        public uint MaxSquad { get; private set; } = 8;

        /// <summary>
        /// Gets the number of respawn waves which must occur before considering Serpents Hand to spawn.
        /// </summary>
        [Description("How many respawn waves must occur before considering Serpents Hand to spawn.")]
        public int RespawnDelay { get; private set; } = 1;

        /// <summary>
        /// Gets the maximum number of times Serpents can spawn per game.
        /// </summary>
        [Description("The maximum number of times Serpents can spawn per game.")]
        public int MaxSpawns { get; private set; } = 1;

        /// <summary>
        /// Gets a value indicating whether Serpents Hand should be able to spawn when there is no SCPs.
        /// </summary>
        [Description("Determines if Serpents Hand should be able to spawn when there is no SCPs.")]
        public bool CanSpawnWithoutScps { get; private set; } = false;

        /// <summary>
        /// Gets the message announced by CASSIE when Serpents hand spawn.
        /// </summary>
        [Description("The message announced by CASSIE when Serpents hand spawn.")]
        public string EntryAnnouncement { get; private set; } = "SERPENTS HAND HASENTERED";

        /// <summary>
        /// Gets the message announced by CASSIE when Chaos spawn.
        /// </summary>
        [Description("The message announced by CASSIE when Chaos spawn.")]
        public string CiEntryAnnouncement { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the broadcast sent to Serpents Hand when they spawn.
        /// </summary>
        [Description("The broadcast sent to Serpents Hand when they spawn.")]
        public string SpawnBroadcast { get; private set; } = "<size=60>You are <color=#03F555><b>Serpents Hand</b></color></size>\n<i>Help the <color=\"red\">SCPs</color> by killing all other classes!</i>";

        /// <summary>
        /// Gets the Serpents Hand spawn position.
        /// </summary>
        [Description("The Serpents Hand spawn position.")]
        public Vector3 SpawnPos { get; private set; } = new Vector3(0f, 1002f, 8f);
    }
}
