using System.Threading;
using Smod2.API;
using Smod2;
using System.Collections.Generic;
using System;

namespace SerpentsHand
{
	class SpawnDelay
	{
		Random rand = new Random();

		public SpawnDelay(Plugin plugin, List<Player> Playerlist, int delay)
		{
			Thread.Sleep(delay);
			List<Player> SHPlayers = new List<Player>();
			List<Player> CIPlayers = Playerlist;
			for (int i = 1; i <= plugin.GetConfigInt("sh_max_squad"); i++)
			{
				Player player = CIPlayers[rand.Next(CIPlayers.Count)];
				SHPlayers.Add(player);
				CIPlayers.Remove(player);
			}

			foreach (Player player in SHPlayers)
				SerpentsHand.SpawnPlayer(player);
			foreach (Player player in CIPlayers)
				player.ChangeRole(Role.SPECTATOR);
		}
	}
}
