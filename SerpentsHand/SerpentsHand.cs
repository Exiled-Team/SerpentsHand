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
        public override Version Version => new Version(2, 1, 1);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(2, 10, 0);

        /// <inheritdoc/>
        public static SerpentsHand Instance;

        private Harmony hInstance;

        /// <summary>
        /// Gets a value indicating whether SCP-035 plugin is installed and enabled.
        /// </summary>
        public static bool IsScp035 { get; private set; } = false;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            hInstance = new Harmony($"cyanox.serpentshand-{DateTime.Now.Ticks}");
            hInstance.PatchAll();

            foreach (var plugin in Loader.Plugins)
            {
                if (Name.ToLower() == "scp035" && Config.IsEnabled)
                {
                    IsScp035 = true;
                    Log.Debug("SCP-035 plugin detected!", Config.Debug);
                    break;
                }
            }

            ServerEvent.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
            ServerEvent.RespawningTeam += EventHandlers.OnTeamRespawn;
            ServerEvent.EndingRound += EventHandlers.OnCheckRoundEnd;

            PlayerEvent.EnteringPocketDimension += EventHandlers.OnPocketDimensionEnter;
            PlayerEvent.FailingEscapePocketDimension += EventHandlers.OnPocketDimensionFail;
            PlayerEvent.EscapingPocketDimension += EventHandlers.OnPocketDimensionEscape;
            PlayerEvent.Hurting += EventHandlers.OnPlayerHurt;
            PlayerEvent.ChangingRole += EventHandlers.OnChangingRole;
            PlayerEvent.Destroying += EventHandlers.OnDestroying;
            PlayerEvent.InsertingGeneratorTablet += EventHandlers.OnGeneratorInsert;
            PlayerEvent.EnteringFemurBreaker += EventHandlers.OnFemurEnter;
            PlayerEvent.Died += EventHandlers.OnPlayerDeath;
            PlayerEvent.Shooting += EventHandlers.OnShooting;

            Scp106Event.Containing += EventHandlers.OnContaining106;

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            ServerEvent.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
            ServerEvent.RespawningTeam -= EventHandlers.OnTeamRespawn;
            ServerEvent.EndingRound -= EventHandlers.OnCheckRoundEnd;

            PlayerEvent.EnteringPocketDimension -= EventHandlers.OnPocketDimensionEnter;
            PlayerEvent.FailingEscapePocketDimension -= EventHandlers.OnPocketDimensionFail;
            PlayerEvent.EscapingPocketDimension -= EventHandlers.OnPocketDimensionEscape;
            PlayerEvent.Hurting -= EventHandlers.OnPlayerHurt;
            PlayerEvent.ChangingRole -= EventHandlers.OnChangingRole;
            PlayerEvent.Destroying -= EventHandlers.OnDestroying;
            PlayerEvent.InsertingGeneratorTablet -= EventHandlers.OnGeneratorInsert;
            PlayerEvent.EnteringFemurBreaker -= EventHandlers.OnFemurEnter;
            PlayerEvent.Died -= EventHandlers.OnPlayerDeath;
            PlayerEvent.Shooting -= EventHandlers.OnShooting;

            Scp106Event.Containing -= EventHandlers.OnContaining106;

            hInstance.UnpatchAll();
            Instance = null;

            base.OnDisabled();
        }
    }
}
