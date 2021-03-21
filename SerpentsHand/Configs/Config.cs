namespace SerpentsHand.Configs
{
    using System.ComponentModel;
    using Exiled.API.Interfaces;
    using SubConfigs;

    /// <inheritdoc cref="IConfig"/>
    public class Config : IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether the plugin is enabled.
        /// </summary>
        [Description("If Serpents Hand is enabled.")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the debug mode is enabled.
        /// </summary>
        [Description("Should debug messages be printed in a console:")]
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets a <see cref="SerepentsHandModifiers"/> configs.
        /// </summary>
        [Description("Options for SerpentsHand players:")]
        public SerepentsHandModifiers SerepentsHandModifiers { get; private set; } = new SerepentsHandModifiers();

        /// <summary>
        /// Gets a <see cref="SpawnManager"/> configs.
        /// </summary>
        [Description("Options for SerpentsHand spawn:")]
        public SpawnManager SpawnManager { get; private set; } = new SpawnManager();
    }
}
