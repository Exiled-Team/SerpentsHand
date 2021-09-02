namespace SerpentsHand
{
    using System;
    using Configs;
    using Exiled.API.Features;
    using HarmonyLib;

    using PlayerEvent = Exiled.Events.Handlers.Player;
    using Scp106Event = Exiled.Events.Handlers.Scp106;
    using ServerEvent = Exiled.Events.Handlers.Server;

    /// <summary>
    /// The main class which inherits <see cref="Plugin{TConfig}"/>.
    /// </summary>
    public class SerpentsHand : Plugin<Config>
    {
        /// <inheritdoc/>
        public static SerpentsHand Instance;

        private Harmony harmony;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            harmony = new Harmony($"cyanox.serpentshand-{DateTime.Now.Ticks}");
            harmony.PatchAll();

            ServerEvent.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
            ServerEvent.RespawningTeam += EventHandlers.OnTeamRespawn;
            ServerEvent.EndingRound += EventHandlers.OnCheckRoundEnd;

            PlayerEvent.EnteringPocketDimension += EventHandlers.OnPocketDimensionEnter;
            PlayerEvent.FailingEscapePocketDimension += EventHandlers.OnPocketDimensionFail;
            PlayerEvent.EscapingPocketDimension += EventHandlers.OnPocketDimensionEscape;
            PlayerEvent.Hurting += EventHandlers.OnPlayerHurt;
            PlayerEvent.ChangingRole += EventHandlers.OnChangingRole;
            PlayerEvent.Destroying += EventHandlers.OnDestroying;
            PlayerEvent.ActivatingGenerator += EventHandlers.OnActivatingGenerator;
            PlayerEvent.EnteringFemurBreaker += EventHandlers.OnFemurEnter;
            PlayerEvent.Died += EventHandlers.OnPlayerDeath;
            PlayerEvent.SpawningRagdoll += EventHandlers.OnSpawningRagdoll;
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
            PlayerEvent.ActivatingGenerator -= EventHandlers.OnActivatingGenerator;
            PlayerEvent.EnteringFemurBreaker -= EventHandlers.OnFemurEnter;
            PlayerEvent.Died -= EventHandlers.OnPlayerDeath;
            PlayerEvent.SpawningRagdoll -= EventHandlers.OnSpawningRagdoll;
            PlayerEvent.Shooting -= EventHandlers.OnShooting;

            Scp106Event.Containing -= EventHandlers.OnContaining106;

            harmony.UnpatchAll();
            Instance = null;

            base.OnDisabled();
        }

        /// <inheritdoc/>
        public override string Name => "SerpentsHand";

        /// <inheritdoc/>
        public override string Author => "Cyanox, maintained by Michal78900";

        /// <inheritdoc/>
        public override Version Version => new Version(3, 0, 2);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(3, 0, 0);
    }
}
