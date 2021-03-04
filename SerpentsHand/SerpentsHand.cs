using System;
using Exiled.API.Features;
using Exiled.Loader;
using HarmonyLib;

using PlayerEvent = Exiled.Events.Handlers.Player;
using ServerEvent = Exiled.Events.Handlers.Server;
using Scp106Event = Exiled.Events.Handlers.Scp106;

namespace SerpentsHand
{
    public class SerpentsHand : Plugin<Config>
    {
        public EventHandlers EventHandlers;

        public static SerpentsHand instance;
        private Harmony hInstance;

        public static bool isScp035 = false;

        public override void OnEnabled()
        {
            hInstance = new Harmony($"cyanox.serpentshand-{DateTime.Now.Ticks}");
            hInstance.PatchAll();

            instance = this;
            EventHandlers = new EventHandlers();
            Check035();

            ServerEvent.RoundStarted += EventHandlers.OnRoundStart;
            ServerEvent.RespawningTeam += EventHandlers.OnTeamRespawn;
            PlayerEvent.EnteringPocketDimension += EventHandlers.OnPocketDimensionEnter;
            PlayerEvent.FailingEscapePocketDimension += EventHandlers.OnPocketDimensionDie;
            PlayerEvent.EscapingPocketDimension += EventHandlers.OnPocketDimensionExit;
            PlayerEvent.Dying += EventHandlers.OnPlayerDying;
            PlayerEvent.Hurting += EventHandlers.OnPlayerHurt;
            ServerEvent.EndingRound += EventHandlers.OnCheckRoundEnd;
            PlayerEvent.ChangingRole += EventHandlers.OnSetRole;
            PlayerEvent.Left += EventHandlers.OnDisconnect;
            Scp106Event.Containing += EventHandlers.OnContain106;
            ServerEvent.SendingRemoteAdminCommand += EventHandlers.OnRACommand;
            PlayerEvent.InsertingGeneratorTablet += EventHandlers.OnGeneratorInsert;
            PlayerEvent.EnteringFemurBreaker += EventHandlers.OnFemurEnter;
            PlayerEvent.Died += EventHandlers.OnPlayerDeath;
            PlayerEvent.Shooting += EventHandlers.OnShoot;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            ServerEvent.RoundStarted -= EventHandlers.OnRoundStart;
            ServerEvent.RespawningTeam -= EventHandlers.OnTeamRespawn;
            PlayerEvent.EnteringPocketDimension -= EventHandlers.OnPocketDimensionEnter;
            PlayerEvent.FailingEscapePocketDimension -= EventHandlers.OnPocketDimensionDie;
            PlayerEvent.EscapingPocketDimension -= EventHandlers.OnPocketDimensionExit;
            PlayerEvent.Dying -= EventHandlers.OnPlayerDying;
            PlayerEvent.Hurting -= EventHandlers.OnPlayerHurt;
            ServerEvent.EndingRound -= EventHandlers.OnCheckRoundEnd;
            PlayerEvent.ChangingRole -= EventHandlers.OnSetRole;
            PlayerEvent.Left -= EventHandlers.OnDisconnect;
            Scp106Event.Containing -= EventHandlers.OnContain106;
            ServerEvent.SendingRemoteAdminCommand -= EventHandlers.OnRACommand;
            PlayerEvent.InsertingGeneratorTablet -= EventHandlers.OnGeneratorInsert;
            PlayerEvent.EnteringFemurBreaker -= EventHandlers.OnFemurEnter;
            PlayerEvent.Died -= EventHandlers.OnPlayerDeath;
            PlayerEvent.Shooting -= EventHandlers.OnShoot;

            hInstance.UnpatchAll();
            EventHandlers = null;
            instance = null;

            base.OnDisabled();
        }

        public override string Name => "SerpentsHand";
        public override string Author => "Cyanox";

        internal void Check035()
        {
            foreach (var plugin in Loader.Plugins)
            {
                if (plugin.Name.ToLower() == "scp035" && plugin.Config.IsEnabled)
                {
                    isScp035 = true;
                    Log.Debug("SCP-035 plugin detected!");
                    return;
                }
            }
        }
    }
}
