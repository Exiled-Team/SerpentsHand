using System;
using System.Collections.Generic;
using System.Linq;
using EXILED;
using EXILED.Extensions;
using MEC;
using scp035.API;

namespace SerpentsHand
{
    partial class EventHandlers
    {
        internal static void SpawnPlayer(ReferenceHub player, bool full = true)
        {
            shPlayers.Add(player.queryProcessor.PlayerId);
            player.characterClassManager.SetClassID(RoleType.Tutorial);
            if (full)
            {
                player.ammoBox.Networkamount = "250:250:250";

                player.inventory.items.ToList().Clear();
                for (int i = 0; i < Configs.spawnItems.Count; i++)
                {
                    player.inventory.AddNewItem((ItemType)Configs.spawnItems[i]);
                }
                player.playerStats.health = Configs.health;
            }

            Timing.CallDelayed(0.3f, () => player.plyMovementSync.OverridePosition(shSpawnPos, 0f));
        }

        internal static void CreateSquad(int size)
        {
            List<ReferenceHub> spec = new List<ReferenceHub>();
            List<ReferenceHub> pList = Player.GetHubs().ToList();

            foreach (ReferenceHub player in pList)
            {
                if (player.GetTeam() == Team.RIP)
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

        internal static void SpawnSquad(List<ReferenceHub> players)
        {
            foreach (ReferenceHub player in players)
            {
                SpawnPlayer(player);
            }

            Cassie.CassieMessage(Configs.entryAnnouncement, true, true);
        }

        private int CountRoles(Team team)
        {
            ReferenceHub scp035 = null;

            try
            {
                scp035 = Scp035Data.GetScp035();
            }
            catch (Exception x)
            {
                Log.Warn("SCP-035 not installed, ignoring API call.");
            }

            int count = 0;
            foreach (ReferenceHub pl in Player.GetHubs())
            {
                if (pl.GetTeam() == team)
                {
                    if (scp035 != null && pl.queryProcessor.PlayerId == scp035.queryProcessor.PlayerId) continue;
                    count++;
                }
            }
            return count;
        }

        private void TeleportTo106(ReferenceHub player)
        {
            ReferenceHub scp106 = Player.GetHubs().Where(x => x.characterClassManager.CurClass == RoleType.Scp106).FirstOrDefault();
            if (scp106 != null)
            {
                player.plyMovementSync.OverridePosition(scp106.transform.position, 0f);
            }
        }
    }
}
