using System.Collections.Generic;
using MEC;

namespace SerpentsHand
{
    partial class EventHandlers
    {
        private void SpawnPlayer(ReferenceHub player)
        {
            shPlayers.Add(player.GetPlayerID());
            player.ChangeRole(Role.TUTORIAL);
            player.SetAmmo(AmmoType.DROPPED_5, 250);
            player.SetAmmo(AmmoType.DROPPED_7, 250);
            player.SetAmmo(AmmoType.DROPPED_9, 250);
            player.SetHealth(Configs.health);
            player.SetPosition(shSpawnPos);
        }

        private void CreateSquad(int size)
        {
            List<ReferenceHub> spec = new List<ReferenceHub>();
            List<ReferenceHub> pList = Plugin.GetHubs();

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

        private void SpawnSquad(List<ReferenceHub> players)
        {
            foreach (ReferenceHub player in players)
            {
                SpawnPlayer(player);
            }

            PluginManager.Manager.Server.Map.AnnounceCustomMessage(Configs.entryAnnouncement);
        }

        private int CountRoles(Role role)
        {
            int count = 0;
            foreach (ReferenceHub pl in Plugin.GetHubs())
                if (pl.characterClassManager.curClass == role)
                    count++;
            return count;
        }

        private int CountRoles(Team team)
        {
            int count = 0;
            foreach (ReferenceHub pl in Plugin.GetHubs())
                if (pl.GetTeam() == team)
                    count++;
            return count;
        }

        private void TeleportTo106(ReferenceHub Player)
        {
            ReferenceHub player = Plugin.GetHubs().Where(x => x.GetRole() == Role.SCP_106).FirstOrDefault();
            if (player != null)
            {
                //Timing.Next(() =>
                //{
                    player.plySyncManager.OverridePosition(player.transform.position);
                //});
            }
        }

        private IEnumerator<float> DelayAction(float delay, Action x)
        {
            Timing.WaitForSeconds(delay);
            x();
        }
    }
}
