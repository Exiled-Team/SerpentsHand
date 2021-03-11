using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using Scp035.API;

namespace SerpentsHand
{
    partial class EventHandlers
    {
        internal static void SpawnPlayer(Player player, bool full = true)
        {
            shPlayers.Add(player.Id);
            player.SetRole(RoleType.Tutorial);
            player.Broadcast(10, SerpentsHand.instance.Config.SpawnBroadcast);
            if (full)
            {
                player.ResetInventory(SerpentsHand.instance.Config.SpawnItems);

                player.Ammo[(int)AmmoType.Nato556] = 250;
                player.Ammo[(int)AmmoType.Nato762] = 250;
                player.Ammo[(int)AmmoType.Nato9] = 250;

                player.Health = SerpentsHand.instance.Config.Health;
            }

            Timing.CallDelayed(0.5f, () => player.Position = shSpawnPos);
        }

        internal static void CreateSquad(int size)
        {
            List<Player> spec = new List<Player>();
            List<Player> pList = Player.List.ToList();

            foreach (Player player in pList)
            {
                if (player.Team == Team.RIP)
                {
                    spec.Add(player);
                }
            }

            int spawnCount = 1;
            while (spec.Count > 0 && spawnCount <= size)
            {
                int index = rand.Next(0, spec.Count);
                if (spec[index] != null)
                {
                    SpawnPlayer(spec[index]);
                    spec.RemoveAt(index);
                    spawnCount++;
                }
            }
        }

        internal static void SpawnSquad(List<Player> players)
        {
            foreach (Player player in players)
            {
                SpawnPlayer(player);
            }

            if (players.Count > 0) 
                Cassie.GlitchyMessage(SerpentsHand.instance.Config.EntryAnnouncement, 0.05f, 0.05f);
        }

        internal static void GrantFF()
        {
            foreach (int id in shPlayers)
            {
                Player p = Player.Get(id);
                if (p != null) p.IsFriendlyFireEnabled = true;
            }

            foreach (int id in SerpentsHand.instance.EventHandlers.shPocketPlayers)
            {
                Player p = Player.Get(id);
                if (p != null) p.IsFriendlyFireEnabled = true;
            }

            shPlayers.Clear();
            SerpentsHand.instance.EventHandlers.shPocketPlayers.Clear();
        }

        private Player TryGet035()
        {
            return Scp035Data.AllScp035.FirstOrDefault();
        }

        private int CountRoles(Team team)
        {
            Player scp035 = null;

            if (SerpentsHand.isScp035)
            {
                scp035 = TryGet035();
            }

            int count = 0;
            foreach (Player pl in Player.List)
            {
                if (pl.Team == team)
                {
                    if (scp035 != null && pl.Id == scp035.Id) continue;
                    count++;
                }
            }
            return count;
        }

        private void TeleportTo106(Player player)
        {
            Player scp106 = Player.List.Where(x => x.Role == RoleType.Scp106).FirstOrDefault();
            if (scp106 != null)
            {
                player.Position = scp106.Position;
            }
            else
            {
                player.Position = Map.GetRandomSpawnPoint(RoleType.Scp096);
            }
        }
    }
}