using System.Collections.Generic;
using Smod2.API;

namespace SerpentsHand.API
{
	public static class SerpentsHand
	{
		public static void SpawnPlayer(Player player)
		{
			SHPlugin.SpawnPlayer(player);
		}

		public static void SpawnSquad(List<Player> PlayerList)
		{
			SHPlugin.SpawnSHSquad(PlayerList);
		}

		public static void SpawnSquad(int size)
		{
			SHPlugin.SpawnSquad(size);
		}
	}
}
