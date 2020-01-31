using System;
using EXILED;

namespace SerpentsHand
{
    public class Plugin : EXILED.Plugin
    {
        public EventHandlers EventHandlers;

        public override void OnEnable()
        {
            EventHandlers = new EventHandlers(this);

            Events.RoundStartEvent += EventHandlers.OnRoundStart;
            Events.RoundEndEvent += EventHandlers.OnRoundEnd;
            Events.TeamRespawnEvent += EventHandlers.OnTeamRespawn;
            Events.PocketDimEnterEvent += EventHandlers.OnPocketDimensionEnter;
            Events.PocketDimDeathEvent += EventHandlers.OnPocketDimensionDie;
            Events.PocketDimEscapedEvent += EventHandlers.OnPocketDimensionExit;
            Events.PlayerDeathEvent += EventHandlers.OnPlayerDie;
            Events.PlayerHurtEvent += EventHandlers.OnPlayerHurt;
            Events.CheckRoundEndEvent += EventHandlers.OnCheckRoundEnd;
            Events.WaitingForPlayersEvent += EventHandlers.OnWaitingForPlayers;
            Events.SetClassEvent += EventHandlers.OnSetRole;
            Events.PlayerLeaveEvent += EventHandlers.OnDisconnect;
            Events.Scp106ContainEvent += EventHandlers.OnContain106;
        }

        public override void OnDisable()
        {
            Events.RoundStartEvent -= EventHandlers.OnRoundStart;
            Events.RoundEndEvent -= EventHandlers.OnRoundEnd;
            Events.TeamRespawnEvent -= EventHandlers.OnTeamRespawn;
            Events.PocketDimEnterEvent -= EventHandlers.OnPocketDimensionEnter;
            Events.PocketDimDeathEvent -= EventHandlers.OnPocketDimensionDie;
            Events.PocketDimEscapedEvent -= EventHandlers.OnPocketDimensionExit;
            Events.PlayerDeathEvent -= EventHandlers.OnPlayerDie;
            Events.PlayerHurtEvent -= EventHandlers.OnPlayerHurt;
            Events.CheckRoundEndEvent -= EventHandlers.OnCheckRoundEnd;
            Events.WaitingForPlayersEvent -= EventHandlers.OnWaitingForPlayers;
            Events.SetClassEvent -= EventHandlers.OnSetRole;
            Events.PlayerLeaveEvent -= EventHandlers.OnDisconnect;
            Events.Scp106ContainEvent -= EventHandlers.OnContain106;

            EventHandlers = null;
        }

        public override void OnReload() { }

        public override string getName { get; } = "Serpents Hand";
    }
}
