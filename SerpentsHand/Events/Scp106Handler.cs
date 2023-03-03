using Exiled.Events.EventArgs;

namespace SerpentsHand.Events
{
    internal sealed class Scp106Handler
    {
        private Config config = SerpentsHand.Singleton.Config;

        /*public void OnContaining(ContainingEventArgs ev)
        {
            if (API.IsSerpent(ev.Player) && !config.SerpentsHandModifiers.FriendlyFire)
                ev.IsAllowed = false;
        }*/
    }
}
