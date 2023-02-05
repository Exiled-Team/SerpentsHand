namespace SerpentsHand.Configs
{
    using Exiled.API.Features;
    using System.Collections.Generic;
    using System.ComponentModel;
    using UnityEngine;

    public class SpawnManager
    {
        [Description("The chance for Serpents Hand to spawn instead of Chaos.")]
        public int SpawnChance { get; set; } = 50;

        [Description("The maximum size of a Serpents Hand squad.")]
        public uint MaxSquad { get; set; } = 8;

        [Description("How many respawn waves must occur before considering Serpents Hand to spawn.")]
        public int RespawnDelay { get; set; } = 1;

        [Description("The maximum number of times Serpents can spawn per game.")]
        public int MaxSpawns { get; set; } = 1;

        [Description("Determines if Serpents Hand should be able to spawn when there is no SCPs.")]
        public bool CanSpawnWithoutScps { get; set; } = false;

        [Description("The message annouced by CASSIE when Serpents hand spawn. (Empty = Disabled)")]
        public string EntryAnnoucement { get; set; } = "SERPENTS HAND HASENTERED";

        [Description("The message annouced by CASSIE when Chaos spawn. (Empty = Disabled)")]
        public string ChaosEntryAnnoucement { get; set; } = "CHAOSINSURGENCY HASENTERED";

        [Description("The broadcast sent to Serpents Hand when they spawn.")]
        public Broadcast SpawnBroadcast { get; set; } = new Broadcast("<size=60>You are <color=#03F555><b>Serpents Hand</b></color></size>\n<i>Help the <color=\"red\">SCPs</color> by killing all other classes!</i>");

        [Description("The broadcast shown to SCPs when the Serpents Hand respawns.")]
        public Broadcast EntryBroadcast { get; set; } = new Broadcast("<color=orange>Serpent's Hand has entered the facility!</color>");

        [Description("The Serpents Hand spawn position.")]
        public Vector3 SpawnPos { get; set; } = new Vector3(0f, 1002f, 8f);

        /*[Description("The Serpents Hand Unit Names")]
        public List<string> UnitNames { get; set; } = new List<string>
        {
            "SH-Vector 13",
            "SH-Alpha 09",
            "SH-Delta 15"
        };*/
    }
}
