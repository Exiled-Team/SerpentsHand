using System.Collections.Generic;

namespace SerpentsHand.API
{
	public static class SerpentsHand
	{
		public static void SpawnPlayer(ReferenceHub player, bool full = true)
		{
			EventHandlers.SpawnPlayer(player, full);
		}

		public static void SpawnSquad(List<ReferenceHub> playerList)
		{
			EventHandlers.SpawnSquad(playerList);
		}

		public static void SpawnSquad(int size)
		{
			EventHandlers.CreateSquad(size);
		}

		public static List<int> GetSHPlayers()
		{
			return EventHandlers.shPlayers;
		}
	}
}
