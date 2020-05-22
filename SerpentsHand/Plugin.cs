using EXILED;

namespace SerpentsHand
{
    public class Plugin : EXILED.Plugin
    {
        public EventHandlers EventHandlers;

        public static bool isScp035 = false;

        public override void OnEnable()
        {
            EventHandlers = new EventHandlers();

            Check035();

            Events.RoundStartEvent += EventHandlers.OnRoundStart;
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
            Events.RemoteAdminCommandEvent += EventHandlers.OnRACommand;
            Events.GeneratorInsertedEvent += EventHandlers.OnGeneratorInsert;
            Events.FemurEnterEvent += EventHandlers.OnFemurEnter;
        }

        public override void OnDisable()
        {
            Events.RoundStartEvent -= EventHandlers.OnRoundStart;
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
            Events.RemoteAdminCommandEvent -= EventHandlers.OnRACommand;
            Events.GeneratorInsertedEvent -= EventHandlers.OnGeneratorInsert;

            EventHandlers = null;
        }

        public override void OnReload() { }

        public override string getName { get; } = "SerpentsHand";

        internal void Check035()
        {
            foreach (EXILED.Plugin plugin in PluginManager._plugins)
            {
                if (plugin.getName == "SCP-035")
                {
                    isScp035 = true;
                    return;
                }
            }
        }
    }
}
