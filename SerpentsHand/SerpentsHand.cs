using Exiled.API.Features;
using Exiled.Loader;

namespace SerpentsHand
{
    public class SerpentsHand : Plugin<Config>
    {
        public EventHandlers EventHandlers;

        public static SerpentsHand instance;

        public static bool isScp035 = false;

        public override void OnEnabled()
        {
            if (!Config.IsEnabled) return;

            instance = this;
            EventHandlers = new EventHandlers();
            Check035();

            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStart;
            Exiled.Events.Handlers.Server.RespawningTeam += EventHandlers.OnTeamRespawn;
            Exiled.Events.Handlers.Player.EnteringPocketDimension += EventHandlers.OnPocketDimensionEnter;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension += EventHandlers.OnPocketDimensionDie;
            Exiled.Events.Handlers.Player.EscapingPocketDimension += EventHandlers.OnPocketDimensionExit;
            Exiled.Events.Handlers.Player.Died += EventHandlers.OnPlayerDie;
            Exiled.Events.Handlers.Player.Hurting += EventHandlers.OnPlayerHurt;
            Exiled.Events.Handlers.Server.EndingRound += EventHandlers.OnCheckRoundEnd;
            Exiled.Events.Handlers.Player.ChangingRole += EventHandlers.OnSetRole;
            Exiled.Events.Handlers.Player.Left += EventHandlers.OnDisconnect;
            Exiled.Events.Handlers.Scp106.Containing += EventHandlers.OnContain106;
            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand += EventHandlers.OnRACommand;
            Exiled.Events.Handlers.Player.InsertingGeneratorTablet += EventHandlers.OnGeneratorInsert;
            Exiled.Events.Handlers.Player.EnteringFemurBreaker += EventHandlers.OnFemurEnter;
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStart;
            Exiled.Events.Handlers.Server.RespawningTeam -= EventHandlers.OnTeamRespawn;
            Exiled.Events.Handlers.Player.EnteringPocketDimension -= EventHandlers.OnPocketDimensionEnter;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension -= EventHandlers.OnPocketDimensionDie;
            Exiled.Events.Handlers.Player.EscapingPocketDimension -= EventHandlers.OnPocketDimensionExit;
            Exiled.Events.Handlers.Player.Died -= EventHandlers.OnPlayerDie;
            Exiled.Events.Handlers.Player.Hurting -= EventHandlers.OnPlayerHurt;
            Exiled.Events.Handlers.Server.EndingRound -= EventHandlers.OnCheckRoundEnd;
            Exiled.Events.Handlers.Player.ChangingRole -= EventHandlers.OnSetRole;
            Exiled.Events.Handlers.Player.Left -= EventHandlers.OnDisconnect;
            Exiled.Events.Handlers.Scp106.Containing -= EventHandlers.OnContain106;
            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand -= EventHandlers.OnRACommand;
            Exiled.Events.Handlers.Player.InsertingGeneratorTablet -= EventHandlers.OnGeneratorInsert;
            Exiled.Events.Handlers.Player.EnteringFemurBreaker -= EventHandlers.OnFemurEnter;

            EventHandlers = null;
        }

        public override string Name => "SerpentsHand";

        internal void Check035()
        {
            foreach (var plugin in Loader.Plugins)
            {
                if (plugin.Name == "scp035")
                {
                    isScp035 = true;
                    return;
                }
            }
        }
    }
}
