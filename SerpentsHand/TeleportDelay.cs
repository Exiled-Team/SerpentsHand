using System.Threading;
using Smod2.API;

namespace SerpentsHand
{
	class TeleportDelay
	{
		public TeleportDelay(Player player, Player target, int delay)
		{
			Thread.Sleep(delay);
			player.Teleport(target.GetPosition());
		}
	}
}
