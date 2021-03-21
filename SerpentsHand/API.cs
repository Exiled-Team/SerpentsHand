using Exiled.API.Features;
using System.Collections.Generic;
using System.Linq;

namespace SerpentsHand
{
	public static class API
	{
		public static bool IsSerpent(Player player)
        {
			return EventHandlers.shPlayers.Contains(player.Id);
        }

		public static void SpawnPlayer(Player player, bool full = true)
		{
			EventHandlers.instance.SpawnPlayer(player, full);
		}

		public static void SpawnSquad(List<Player> playerList)
		{
			EventHandlers.instance.SpawnSquad(playerList);
		}

		public static void SpawnSquad(int size)
		{
			EventHandlers.instance.CreateSquad(size);
		}

		public static List<Player> GetSHPlayers()
		{
			return EventHandlers.shPlayers.Select(x => Player.Get(x)).ToList();
		}
	}
}
