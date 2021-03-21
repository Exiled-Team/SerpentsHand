namespace SerpentsHand
{
    using System;
    using Exiled.API.Features;
    using Exiled.Loader;
    using HarmonyLib;

    using PlayerEvent = Exiled.Events.Handlers.Player;
    using Scp106Event = Exiled.Events.Handlers.Scp106;
    using ServerEvent = Exiled.Events.Handlers.Server;

    /// <summary>
    /// The main class which inherits <see cref="Plugin{TConfig}"/>.
    /// </summary>
    public class SerpentsHand : Plugin<Configs.Config>
    {
        /// <inheritdoc/>
        public override string Name => "SerpentsHand";

        /// <inheritdoc/>
        public override string Author => "Cyanox, maintained by Michal78900";

        /// <inheritdoc/>
        public override Version Version => new Version(2, 0, 0);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(2, 8, 0);


        public static SerpentsHand Instance;

        private EventHandlers eventHandlers;

        private Harmony hInstance;

        /// <summary>
        /// Gets or sets a value indicating whether SCP-035 plugin is installed.
        /// </summary>
        public static bool IsScp035 { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether ustomItems plugin is installed.
        /// </summary>
        public static bool IsCustomItems { get; set; } = false;


        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;
            eventHandlers = new EventHandlers(this);

            hInstance = new Harmony($"cyanox.serpentshand-{DateTime.Now.Ticks}");
            hInstance.PatchAll();

            foreach (var plugin in Loader.Plugins)
            {
                if (plugin.Name.ToLower() == "scp035" && plugin.Config.IsEnabled)
                {
                    IsScp035 = true;
                    Log.Debug("SCP-035 plugin detected!");
                }

                if (plugin.Name.ToLower() == "customitems" && plugin.Config.IsEnabled)
                {
                    IsCustomItems = true;
                    Log.Debug("CustomItems plugin detected!");
                }
            }

            ServerEvent.WaitingForPlayers += eventHandlers.OnWaitingForPlayers;
            ServerEvent.RespawningTeam += eventHandlers.OnTeamRespawn;
            ServerEvent.EndingRound += eventHandlers.OnCheckRoundEnd;

            PlayerEvent.EnteringPocketDimension += eventHandlers.OnPocketDimensionEnter;
            PlayerEvent.FailingEscapePocketDimension += eventHandlers.OnPocketDimensionFail;
            PlayerEvent.EscapingPocketDimension += eventHandlers.OnPocketDimensionEscape;
            PlayerEvent.Hurting += eventHandlers.OnPlayerHurt;
            PlayerEvent.ChangingRole += eventHandlers.OnChangingRole;
            PlayerEvent.Destroying += eventHandlers.OnDestroying;
            PlayerEvent.InsertingGeneratorTablet += eventHandlers.OnGeneratorInsert;
            PlayerEvent.EnteringFemurBreaker += eventHandlers.OnFemurEnter;
            PlayerEvent.Died += eventHandlers.OnPlayerDeath;
            PlayerEvent.Shooting += eventHandlers.OnShooting;

            Scp106Event.Containing += eventHandlers.OnContaining106;

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            ServerEvent.WaitingForPlayers -= eventHandlers.OnWaitingForPlayers;
            ServerEvent.RespawningTeam -= eventHandlers.OnTeamRespawn;
            ServerEvent.EndingRound -= eventHandlers.OnCheckRoundEnd;

            PlayerEvent.EnteringPocketDimension -= eventHandlers.OnPocketDimensionEnter;
            PlayerEvent.FailingEscapePocketDimension -= eventHandlers.OnPocketDimensionFail;
            PlayerEvent.EscapingPocketDimension -= eventHandlers.OnPocketDimensionEscape;
            PlayerEvent.Hurting -= eventHandlers.OnPlayerHurt;
            PlayerEvent.ChangingRole -= eventHandlers.OnChangingRole;
            PlayerEvent.Destroying -= eventHandlers.OnDestroying;
            PlayerEvent.InsertingGeneratorTablet -= eventHandlers.OnGeneratorInsert;
            PlayerEvent.EnteringFemurBreaker -= eventHandlers.OnFemurEnter;
            PlayerEvent.Died -= eventHandlers.OnPlayerDeath;
            PlayerEvent.Shooting -= eventHandlers.OnShooting;

            Scp106Event.Containing -= eventHandlers.OnContaining106;

            hInstance.UnpatchAll();
            eventHandlers = null;
            Instance = null;

            base.OnDisabled();
        }
    }
}
