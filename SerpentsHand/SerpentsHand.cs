using Exiled.API.Features;
using SerpentsHand.Events;
using System;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;
using Warhead = Exiled.Events.Handlers.Warhead;

namespace SerpentsHand
{
    public class SerpentsHand : Plugin<Config>
    {
        public static SerpentsHand Singleton;

        public override string Name => "Serpents Hand";
        public override string Author => "yanox, Michal78900 and Marco15453";
        public override Version RequiredExiledVersion => new Version(6, 1, 0);
        public override Version Version => new Version(5, 0, 1);

        public int TeamRespawnCount;
        public int SerpentsRespawnCount;
        public bool IsSpawnable;

        private PlayerHandler playerHandler;
        private ServerHandler serverHandler;
        private WarheadHandler warheadHandler;

        public override void OnEnabled()
        {
            Singleton = this;

            RegisterEvents();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            playerHandler = new PlayerHandler();
            serverHandler = new ServerHandler();
            warheadHandler = new WarheadHandler();

            // Player
            Player.FailingEscapePocketDimension += playerHandler.OnFailingEscapePocketDimension;
            Player.EscapingPocketDimension += playerHandler.OnEscapingPocketDimension;
            Player.Hurting += playerHandler.OnHurting;
            Player.Shooting += playerHandler.OnShooting;
            Player.ActivatingGenerator += playerHandler.OnActivatingGenerator;
            Player.Destroying += playerHandler.OnDestroying;
            Player.Died += playerHandler.OnDied;
            Player.ChangingRole += playerHandler.OnChangingRole;

            // Server
            Server.WaitingForPlayers += serverHandler.OnWaitingForPlayers;
            Server.RespawningTeam += serverHandler.OnRespawningTeam;
            Server.EndingRound += serverHandler.OnEndingRound;

            // Warhead
            Warhead.Detonated += warheadHandler.OnDetonated;
        }

        private void UnregisterEvents()
        {
            // Player
            Player.FailingEscapePocketDimension -= playerHandler.OnFailingEscapePocketDimension;
            Player.EscapingPocketDimension -= playerHandler.OnEscapingPocketDimension;
            Player.Hurting -= playerHandler.OnHurting;
            Player.Shooting -= playerHandler.OnShooting;
            Player.ActivatingGenerator -= playerHandler.OnActivatingGenerator;
            Player.Destroying -= playerHandler.OnDestroying;
            Player.Died -= playerHandler.OnDied;
            Player.ChangingRole -= playerHandler.OnChangingRole;

            // Server
            Server.WaitingForPlayers -= serverHandler.OnWaitingForPlayers;
            Server.RespawningTeam -= serverHandler.OnRespawningTeam;
            Server.EndingRound -= serverHandler.OnEndingRound;

            // Warhead
            Warhead.Detonated -= warheadHandler.OnDetonated;

            playerHandler = null;
            serverHandler = null;
            warheadHandler = null;
        }
    }
}
