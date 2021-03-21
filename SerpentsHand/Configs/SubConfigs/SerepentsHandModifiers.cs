namespace SerpentsHand.Configs.SubConfigs
{
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Configs for Serpents Hand player.
    /// </summary>
    public class SerepentsHandModifiers
    {
        /// <summary>
        /// Gets
        /// </summary>
        [Description("Determines role name seen in game:")]
        public string RoleName { get; private set; } = "Serpent's Hand";

        /// <summary>
        /// Gets
        /// </summary>
        [Description("Determines color role name seen in game: (leave empty for default Tutorial green)")]
        public string RoleColor { get; private set; } = "";

        /// <summary>
        /// Gets
        /// </summary>
        [Description("The amount of health Serpents Hand has.")]
        public float Health { get; private set; } = 120f;

        /// <summary>
        /// Gets
        /// </summary>
        [Description("The items Serpents Hand spawn with. (supports CustomItems)")]
        public List<string> SpawnItems { get; private set; } = new List<string>()
        {
            "GunProject90",
            "KeycardChaosInsurgency",
            "GrenadeFlash",
            "Radio",
            "Medkit",
        };

        /// <summary>
        /// Gets
        /// </summary>
        [Description("Determines if friendly fire between Serpents Hand and SCPs is enabled.")]
        public bool FriendlyFire { get; private set; } = false;

        /// <summary>
        /// Gets
        /// </summary>
        [Description("Determines if Serpents Hand should teleport to SCP-106 after exiting his pocket dimension.")]
        public bool TeleportTo106 { get; private set; } = true;

        /// <summary>
        /// Gets
        /// </summary>
        [Description("Determines if Serpents Hand should be able to hurt SCPs after the round ends.")]
        public bool EndRoundFriendlyFire { get; private set; } = false;

        /// <summary>
        /// Gets
        /// </summary>
        [Description("Set this to false if Chaos and SCPs CANNOT win together on your server.")]
        public bool ScpsWinWithChaos { get; private set; } = true;
    }
}
