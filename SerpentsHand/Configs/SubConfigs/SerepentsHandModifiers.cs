using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerpentsHand.Configs.SubConfigs
{
    public class SerepentsHandModifiers
    {
        [Description("Determines role name seen in game:")]
        public string RoleName { get; set; } = "Serpent's Hand";

        [Description("Determines color role name seen in game: (leave empty for default Tutorial green)")]
        public string RoleColor { get; set; } = "";

        [Description("The amount of health Serpents Hand has.")]
        public float Health { get; set; } = 120f;

        [Description("The items Serpents Hand spawn with. (supports CustomItems)")]
        public List<string> SpawnItems { get; set; } = new List<string>()
        {
            "GunProject90",
            "KeycardChaosInsurgency",
            "GrenadeFlash",
            "Radio",
            "Medkit",
        };

        [Description("Determines if friendly fire between Serpents Hand and SCPs is enabled.")]
        public bool FriendlyFire { get; set; } = false;

        [Description("Determines if Serpents Hand should teleport to SCP-106 after exiting his pocket dimension.")]
        public bool TeleportTo106 { get; set; } = true;

        [Description("Determines if Serpents Hand should be able to hurt SCPs after the round ends.")]
        public bool EndRoundFriendlyFire { get; set; } = false;

        [Description("Set this to false if Chaos and SCPs CANNOT win together on your server.")]
        public bool ScpsWinWithChaos { get; set; } = true;
    }
}
