using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using SerpentsHand.Configs.SubConfigs;

namespace SerpentsHand.Configs
{
    public class Config : IConfig
    {
        [Description("If Serpents Hand is enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should debug messages be printed in a console:")]
        public bool Debug { get; set; } = false;

        [Description("Options for SerpentsHand players:")]
        public SerepentsHandModifiers SerepentsHandModifiers { get; set; } = new SerepentsHandModifiers();

        [Description("Options for SerpentsHand spawn:")]
        public SpawnManager SpawnManager { get; set; } = new SpawnManager();
    }
}
