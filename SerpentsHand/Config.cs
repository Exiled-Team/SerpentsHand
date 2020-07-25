using Exiled.API.Interfaces;
using System.Collections.Generic;

namespace SerpentsHand
{
    public class Config : IConfig
    {
		public bool IsEnabled { get; set; } = true;

		public List<int> SpawnItems { get; set; } = new List<int>() { 21, 26, 12, 14, 10 };

		public int SpawnChance { get; set; } = 50;
		public int Health { get; set; } = 120;
		public int MaxSquad { get; set; } = 8;
		public int RespawnDelay { get; set; } = 1;

		public string EntryAnnouncement { get; set; } = "SERPENTS HAND HASENTERED";
		public string CiEntryAnnouncement { get; set; } = "";

		public bool FriendlyFire { get; set; } = false;
		public bool TeleportTo106 { get; set; } = true;
		public bool ScpsWinWithChaos { get; set; } = true;
    }
}
