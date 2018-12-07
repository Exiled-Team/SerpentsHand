using System.Threading;
using Smod2.API;
using System.Collections.Generic;

namespace SerpentsHand
{
	class SpawnDelay
	{
		public SpawnDelay(List<Player> Playerlist, int delay)
		{
			Thread.Sleep(delay);
			foreach (Player player in Playerlist)
			{
				SerpentsHand.SpawnPlayer(player);
			}
		}
	}
}
