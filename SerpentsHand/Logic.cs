namespace SerpentsHand
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using MEC;

    public partial class EventHandlers
    {
        internal void SpawnPlayer(Player player, bool full = true)
        {
            shPlayers.Add(player.Id);
            player.Role = RoleType.Tutorial;
            player.Broadcast(10, plugin.Config.SpawnManager.SpawnBroadcast);

            if (full)
            {
                player.ClearInventory();

                foreach (string item in plugin.Config.SerepentsHandModifiers.SpawnItems)
                {
                    try
                    {
                        player.AddItem((ItemType)Enum.Parse(typeof(ItemType), item, true));
                    }
                    catch(Exception)
                    {
                        if (!SerpentsHand.isCustomItems)
                        {
                            Log.Error($"\"{item}\" is not a valid item name.");
                            continue;
                        }
                        else
                            CustomItemHandler(player, item);
                    }
                }

                player.Ammo[(int)AmmoType.Nato556] = 250;
                player.Ammo[(int)AmmoType.Nato762] = 250;
                player.Ammo[(int)AmmoType.Nato9] = 250;

                player.Health = plugin.Config.SerepentsHandModifiers.Health;
            }

            Timing.CallDelayed(0.5f, () => player.Position = shSpawnPos);
        }

        internal void CustomItemHandler(Player player, string item)
        {
            if(!Exiled.CustomItems.API.Features.CustomItem.TryGive(player, item, false))
            {
                Log.Error($"\"{item}\" is not a valid item / customitem name.");
            }
        }

        internal void CreateSquad(int size)
        {
            List<Player> spec = new List<Player>();

            foreach (Player player in Player.List.Where(x => x.Team == Team.RIP && !x.IsOverwatchEnabled))
            {
                spec.Add(player);
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

        internal void SpawnSquad(List<Player> players)
        {
            foreach (Player player in players)
            {
                SpawnPlayer(player);
            }

            if (players.Count > 0)
                Cassie.GlitchyMessage(plugin.Config.SpawnManager.EntryAnnouncement, 0.05f, 0.05f);
        }

        internal void GrantFF()
        {
            foreach (int id in shPlayers)
            {
                Player p = Player.Get(id);
                if (p != null) p.IsFriendlyFireEnabled = true;
            }

            foreach (int id in shPocketPlayers)
            {
                Player p = Player.Get(id);
                if (p != null) p.IsFriendlyFireEnabled = true;
            }

            shPlayers.Clear();
            shPocketPlayers.Clear();
        }

        private Player TryGet035()
        {
            return Scp035.API.AllScp035.FirstOrDefault();
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
            Player scp106 = Player.List.FirstOrDefault(x => x.Role == RoleType.Scp106);
            if (scp106 != null)
            {
                player.Position = scp106.Position;
            }
            else
            {
                player.Position = Map.GetRandomSpawnPoint(RoleType.Scp096);
            }
        }

        private string FakeMtfUnit()
        {
            const string alphabet = "abcdefghijklmnopqrstuvwxyz";
            int unitNumber = rand.Next(0, 16);
            string unitName = $"nato_{alphabet[rand.Next(alphabet.Count())]} {unitNumber}";

            return unitName;
        }
    }
}