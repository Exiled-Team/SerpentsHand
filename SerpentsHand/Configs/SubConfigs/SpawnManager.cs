using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerpentsHand.Configs.SubConfigs
{
    public class SpawnManager
    {
        [Description("The change for Serpents Hand to spawn instead of Chaos.")]
        public int SpawnChance { get; set; } = 50;

        [Description("The maximum size of a Serpents Hand squad.")]
        public int MaxSquad { get; set; } = 8;

        [Description("How many respawn waves must occur before considering Serpents Hand to spawn.")]
        public int RespawnDelay { get; set; } = 1;

        [Description("The maxium number of times Serpents can spawn per game.")]
        public int MaxSpawns { get; set; } = 1;

        [Description("The message announced by CASSIE when Serpents hand spawn.")]
        public string EntryAnnouncement { get; set; } = "SERPENTS HAND HASENTERED";

        [Description("The message announced by CASSIE when Chaos spawn.")]
        public string CiEntryAnnouncement { get; set; } = "";

        [Description("The broadcast sent to Serpents Hand when they spawn.")]
        public string SpawnBroadcast { get; set; } = "<size=60>You are <color=#03F555><b>Serpents Hand</b></color></size>\n<i>Help the <color=\"red\">SCPs</color> by killing all other classes!</i>";
    }
}
