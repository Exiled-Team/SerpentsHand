namespace SerpentsHand.Configs.SubConfigs
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Enums;

    /// <summary>
    /// Configs for Serpents Hand player.
    /// </summary>
    public class SerepentsHandModifiers
    {
        /// <summary>
        /// Gets Serpents Hand role name.
        /// </summary>
        [Description("Determines role name seen in game:")]
        public string RoleName { get; private set; } = "Serpent's Hand";

        /// <summary>
        /// Gets Serpents Hand role color.
        /// </summary>
        [Description("Determines color role name seen in game: (leave empty for default Tutorial green)")]
        public string RoleColor { get; private set; } = string.Empty;

        /// <summary>
        /// Gets Serpents Hand health.
        /// </summary>
        [Description("The amount of health Serpents Hand has.")]
        public float Health { get; private set; } = 120f;

        /// <summary>
        /// Gets Serpents Hand inventory.
        /// </summary>
        [Description("The items Serpents Hand spawn with. (supports CustomItems)")]
        public List<string> SpawnItems { get; private set; } = new List<string>()
        {
            "GunCrossvec",
            "KeycardChaosInsurgency",
            "GrenadeFlash",
            "Radio",
            "Medkit",
            "ArmorCombat",
        };

        /// <summary>
        /// Gets Serpents Hand starting ammo.
        /// </summary>
        [Description("The ammo Serpents Hand spawn with.")]
        public Dictionary<AmmoType, ushort> SpawnAmmo { get; private set; } = new Dictionary<AmmoType, ushort>()
        {
            { AmmoType.Nato556, 0 },
            { AmmoType.Nato762, 0 },
            { AmmoType.Nato9, 120 },
            { AmmoType.Ammo12Gauge, 0 },
            { AmmoType.Ammo44Cal, 0 },
        };

        /// <summary>
        /// Gets a value indicating whether friendly fire between Serpents Hand and SCPs is enabled.
        /// </summary>
        [Description("Determines if friendly fire between Serpents Hand and SCPs is enabled.")]
        public bool FriendlyFire { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether Serpents Hand should teleport to SCP-106 after exiting his pocket dimension.
        /// </summary>
        [Description("Determines if Serpents Hand should teleport to SCP-106 after exiting his pocket dimension.")]
        public bool TeleportTo106 { get; private set; } = true;

        /// <summary>
        /// Gets a value indicating whether Chaos and SCPs CANNOT win together on a server.
        /// </summary>
        [Description("Set this to false if Chaos and SCPs CANNOT win together on your server.")]
        public bool ScpsWinWithChaos { get; private set; } = true;
    }
}
