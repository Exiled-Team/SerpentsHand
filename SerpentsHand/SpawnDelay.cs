using System.Threading;
using Smod2.API;

namespace SerpentsHand
{
	class SpawnDelay
	{
		public SpawnDelay(Player player, int delay)
		{
			Thread.Sleep(delay);
			SerpentsHand.SpawnPlayer(player);
		}
	}
}
