using Exiled.API.Interfaces;
using SerpentsHand.Configs;
using System.ComponentModel;

namespace SerpentsHand
{
    public sealed class Config : IConfig
    {
        [Description("Whether or not the plugin is enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not debug messages will be shown")]
        public bool Debug { get; set; } = false;

        [Description("Options for Serpents Hand players")]
        public SerpentsHandModifiers SerpentsHandModifiers { get; set; } = new SerpentsHandModifiers();

        [Description("Options for Serpents Hand spawn")]
        public SpawnManager SpawnManager { get; set; } = new SpawnManager();
    }
}
