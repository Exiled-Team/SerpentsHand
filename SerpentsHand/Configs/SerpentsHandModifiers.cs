using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;

namespace SerpentsHand.Configs
{
	public class SerpentsHandModifiers
	{
		[Description("Determines role name seen ingame (Supports Colors)")]
		public string RoleName { get; set; } = "Serpent's Hand";

		[Description("The amount of health Serpents Hand has.")]
		public float Health { get; set; } = 120f;

		[Description("The items Serpents Hand spawn with. (supports CustomItems)")]
		public List<string> SpawnItems { get; set; } = new List<string>
		{
			"GunCrossvec",
			"KeycardChaosInsurgency",
			"GrenadeFlash",
			"Radio",
			"Medkit",
			"ArmorCombat"
		};

		[Description("The ammo Serpents Hand spawn with.")]
		public Dictionary<AmmoType, ushort> SpawnAmmo { get; set; } = new Dictionary<AmmoType, ushort>()
        {
			{ AmmoType.Nato556, 0 },
			{ AmmoType.Nato762, 0 },
			{ AmmoType.Nato9, 120 },
			{ AmmoType.Ammo12Gauge, 0 },
			{ AmmoType.Ammo44Cal, 0 },
		};

		[Description("Determines if friendly fire between Serpents Hand and SCPs is enabled")]
		public bool FriendlyFire { get; set; } = false;

		[Description("Determines if Serpents Hand should teleport to SCP-106 after exiting his pocket dimension")]
		public bool TeleportTo106 { get; set; } = true;

		[Description("Determines if Serpents Hand should be killed within the pocket dimension when the warhead detonates.")]
		public bool WarheadKillsInPocket { get; set; } = true;

		[Description("Set this to false if Chaos and SCPs CANNOT win together on your server")]
		public bool ScpsWinWithChaos { get; set; } = true;
	}
}
