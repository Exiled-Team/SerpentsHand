namespace SerpentsHand
{
    using System;
    using System.Linq;
    using System.Reflection;
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
        public override Version Version => new Version(2, 2, 0);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(2, 11, 1);

        /// <inheritdoc/>
        public static SerpentsHand Instance;

        private Harmony hInstance;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            hInstance = new Harmony($"cyanox.serpentshand-{DateTime.Now.Ticks}");
            hInstance.PatchAll();

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
