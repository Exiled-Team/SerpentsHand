using Exiled.API.Interfaces;
using System.ComponentModel;

namespace SerpentsHand
{
    public sealed class Config : IConfig
    {
        [Description("Whether or not the plugin is enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not debug messages will be shown")]
        public bool Debug { get; set; } = false;

        public SerpentsHand SerpentsHand { get; set; } = new();
    }
}
