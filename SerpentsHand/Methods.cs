using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using scp035.API;

namespace SerpentsHand
{
    partial class EventHandlers
    {
        internal static void SpawnPlayer(ReferenceHub player)
        {
            shPlayers.Add(player.queryProcessor.PlayerId);
            player.characterClassManager.SetClassID(RoleType.Tutorial);
            player.ammoBox.Networkamount = "250:250:250";

            player.inventory.items.ToList().Clear();
            for (int i = 0; i < Configs.spawnItems.Count; i++)
            {
                player.inventory.AddNewItem((ItemType)Configs.spawnItems[i]);
            }
            player.playerStats.health = Configs.health;

            Timing.CallDelayed(0.5f, () => player.plyMovementSync.OverridePosition(shSpawnPos, 180f));
        }

        internal static void CreateSquad(int size)
        {
            List<ReferenceHub> spec = new List<ReferenceHub>();
            List<ReferenceHub> pList = Plugin.GetHubs();

            foreach (ReferenceHub player in pList)
            {
                if (Plugin.GetTeam(player.characterClassManager.CurClass) == Team.RIP)
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

            EXILED.Extensions.Cassie.CassieMessage(Configs.entryAnnouncement, true, true);
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
                Plugin.Warn("SCP-035 not installed, ignoring API call.");
            }

            int count = 0;
            foreach (ReferenceHub pl in Plugin.GetHubs())
            {
                if (Plugin.GetTeam(pl.characterClassManager.CurClass) == team)
                {
                    if (scp035 != null && pl.queryProcessor.PlayerId == scp035.queryProcessor.PlayerId) continue;
                    count++;
                }
            }
            return count;
        }

        private void TeleportTo106(ReferenceHub player)
        {
            ReferenceHub scp106 = Plugin.GetHubs().Where(x => x.characterClassManager.CurClass == RoleType.Scp106).FirstOrDefault();
            if (scp106 != null)
            {
                player.plyMovementSync.OverridePosition(scp106.transform.position, 0f);
            }
        }
    }
}
