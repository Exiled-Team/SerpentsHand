using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Server;
using Exiled.Loader;
using Respawning;
using System;
using Server = Exiled.Events.Handlers.Server;

namespace SerpentsHand
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "Serpent's Hand";
        public override string Author => "Marco15453";
        public override Version RequiredExiledVersion => new Version(7, 0, 0);
        public override Version Version => new Version(6, 0, 0);

        private EventHandlers eventHandlers;

        public override void OnEnabled()
        {
            Config.SerpentsHand.Register();
            eventHandlers = new EventHandlers(this);

            Server.RoundStarted += eventHandlers.OnRoundStarted;
            Server.RespawningTeam += eventHandlers.OnRespawningTeam;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            CustomRole.UnregisterRoles();
            Server.RoundStarted -= eventHandlers.OnRoundStarted;
            Server.RespawningTeam -= eventHandlers.OnRespawningTeam;

            eventHandlers = null;
            base.OnDisabled();
        }
    }
}
