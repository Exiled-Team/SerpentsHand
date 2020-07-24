using Exiled.API.Interfaces;
using System.Collections.Generic;

namespace SerpentsHand
{
    public class Config : IConfig
    {
		public bool IsEnabled { get; set; } = true;

		public List<int> SpawnItems = new List<int>() { 21, 26, 12, 14, 10 };

		public int SpawnChance = 50;
		public int Health = 120;
		public int MaxSquad = 8;
		public int RespawnDelay = 1;

		public string EntryAnnouncement = "SERPENTS HAND HASENTERED";
		public string CiEntryAnnouncement = "";

		public bool FriendlyFire = false;
		public bool TeleportTo106 = true;
		public bool ScpsWinWithChaos = true;
    }
}
