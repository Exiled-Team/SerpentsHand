namespace SerpentsHand
{
    using System;
    using Exiled.API.Features;
    using Exiled.Loader;
    using HarmonyLib;

    using ServerEvent = Exiled.Events.Handlers.Server;
    using PlayerEvent = Exiled.Events.Handlers.Player;
    using Scp106Event = Exiled.Events.Handlers.Scp106;

    public class SerpentsHand : Plugin<Configs.Config>
    {
        public override string Name => "SerpentsHand";
        public override string Author => "Cyanox, maintained by Michal78900";
        public override Version Version => new Version(2, 0, 0);
        public override Version RequiredExiledVersion => new Version(2, 8, 0);


        public static SerpentsHand instance;

        private EventHandlers EventHandlers;

        private Harmony hInstance;

        public static bool isScp035 = false;
        public static bool isCustomItems = false;


        public override void OnEnabled()
        {
            instance = this;
            EventHandlers = new EventHandlers(this);

            hInstance = new Harmony($"cyanox.serpentshand-{DateTime.Now.Ticks}");
            hInstance.PatchAll();

            foreach (var plugin in Loader.Plugins)
            {
                if (plugin.Name.ToLower() == "scp035" && plugin.Config.IsEnabled)
                {
                    isScp035 = true;
                    Log.Debug("SCP-035 plugin detected!");
                }

                if (plugin.Name.ToLower() == "customitems" && plugin.Config.IsEnabled)
                {
                    isCustomItems = true;
                    Log.Debug("CustomItems plugin detected!");
                }
            }

            ServerEvent.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
            ServerEvent.RespawningTeam += EventHandlers.OnTeamRespawn;
            ServerEvent.EndingRound += EventHandlers.OnCheckRoundEnd;
            //ServerEvent.SendingRemoteAdminCommand += EventHandlers.OnRACommand;

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

        public override void OnDisabled()
        {
            ServerEvent.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
            ServerEvent.RespawningTeam -= EventHandlers.OnTeamRespawn;
            ServerEvent.EndingRound -= EventHandlers.OnCheckRoundEnd;
            //ServerEvent.SendingRemoteAdminCommand -= EventHandlers.OnRACommand;

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
            EventHandlers = null;
            instance = null;
            //EventHandlers.instance = null;

            base.OnDisabled();
        }
    }
}
