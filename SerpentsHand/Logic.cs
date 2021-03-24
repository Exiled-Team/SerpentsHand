namespace SerpentsHand
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using MEC;

    /// <summary>
    /// EventHandlers and Logic which Serpents Hand use.
    /// </summary>
    public partial class EventHandlers
    {
        /// <summary>
        /// Spawns <see cref="Player"/> as Serpents Hand.
        /// </summary>
        /// <param name="player"> The player to spawn.</param>
        /// <param name="full"> Should items and ammo be given to spawned <see cref="Player"/>.</param>
        internal void SpawnPlayer(Player player, bool full = true)
        {
            ShPlayers.Add(player.Id);
            player.Role = RoleType.Tutorial;
            player.Broadcast(10, plugin.Config.SpawnManager.SpawnBroadcast);

            if (full)
            {
                if (plugin.Config.SerepentsHandModifiers.SpawnItems.Count > 0)
                    player.ClearInventory();

                foreach (string item in plugin.Config.SerepentsHandModifiers.SpawnItems)
                {
                    try
                    {
                        player.AddItem((ItemType)Enum.Parse(typeof(ItemType), item, true));
                    }
                    catch (Exception)
                    {
                        if (!SerpentsHand.IsCustomItems)
                        {
                            Log.Error($"\"{item}\" is not a valid item name.");
                        }
                        else
                        {
                            CustomItemHandler(player, item);
                        }
                    }
                }

                foreach (var ammo in plugin.Config.SerepentsHandModifiers.SpawnAmmo)
                {
                    player.Ammo[(int)ammo.Key] = ammo.Value;
                }
            }

            player.MaxHealth = (int)plugin.Config.SerepentsHandModifiers.Health;
            player.Health = plugin.Config.SerepentsHandModifiers.Health;

            string roleName = string.Empty;

            if (!string.IsNullOrEmpty(plugin.Config.SerepentsHandModifiers.RoleColor))
                roleName += $"<color={plugin.Config.SerepentsHandModifiers.RoleColor}>";

            roleName += $"{player.Nickname}\n{plugin.Config.SerepentsHandModifiers.RoleName}";

            if (!string.IsNullOrEmpty(plugin.Config.SerepentsHandModifiers.RoleColor))
                roleName += "</color>";

            player.CustomInfo = roleName;
            player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
            player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;

            Timing.CallDelayed(0.5f, () => player.Position = shSpawnPos);
        }

        /// <summary>
        /// Handles giving customitems to the player.
        /// </summary>
        /// <param name="player">The player to which custom item should be given.</param>
        /// <param name="item">The name of custom item.</param>
        internal void CustomItemHandler(Player player, string item)
        {
            if (!Exiled.CustomItems.API.Features.CustomItem.TryGive(player, item, false))
            {
                Log.Error($"\"{item}\" is not a valid item / customitem name.");
            }
        }

        /// <summary>
        /// Removes Serpents Hand role name and id from a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The player to remove.</param>
        internal void DestroySH(Player player)
        {
            ShPlayers.Remove(player.Id);

            player.MaxHealth = default;

            player.CustomInfo = string.Empty;
            player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Nickname;
            player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
        }

        /// <summary>
        /// Spawns Serpents Hand squad.
        /// </summary>
        /// <param name="size"> The number of players in squad (this can be lower due to not enough Spectators).</param>
        internal void SpawnSquad(uint size)
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

            if (spawnCount > 0)
                Cassie.GlitchyMessage(plugin.Config.SpawnManager.EntryAnnouncement, 0.05f, 0.05f);
        }

        /// <summary>
        /// Spawns Serpents Hand squad.
        /// </summary>
        /// <param name="players"> List of players to spawn.</param>
        internal void SpawnSquad(List<Player> players)
        {
            foreach (Player player in players)
            {
                SpawnPlayer(player);
            }

            if (players.Count > 0)
                Cassie.GlitchyMessage(plugin.Config.SpawnManager.EntryAnnouncement, 0.05f, 0.05f);
        }

        /// <summary>
        /// Gives Serpents Hand players friendly fire.
        /// </summary>
        internal void GrantFF()
        {
            foreach (int id in ShPlayers)
            {
                Player p = Player.Get(id);
                if (p != null)
                    p.IsFriendlyFireEnabled = true;
            }

            foreach (int id in shPocketPlayers)
            {
                Player p = Player.Get(id);
                if (p != null)
                    p.IsFriendlyFireEnabled = true;
            }

            ShPlayers.Clear();
            shPocketPlayers.Clear();
        }

        private Player TryGet035()
        {
            return Scp035.API.AllScp035.FirstOrDefault();
        }

        private int CountRoles(Team team)
        {
            Player scp035 = null;

            if (SerpentsHand.IsScp035)
            {
                scp035 = TryGet035();
            }

            int count = 0;
            foreach (Player pl in Player.List)
            {
                if (pl.Team == team)
                {
                    if (scp035 != null && pl.Id == scp035.Id)
                        continue;
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
    }
}