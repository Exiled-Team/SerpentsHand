namespace SerpentsHand
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using MEC;
    using UnityEngine;

    public partial class EventHandlers
    {
        private readonly SerpentsHand plugin;
        public EventHandlers(SerpentsHand plugin) => this.plugin = plugin;

        public static EventHandlers instance;


        public static List<int> shPlayers = new List<int>();
        private List<int> shPocketPlayers = new List<int>();

        public static int teamRespawnCount = 0;
        public static int serpentsRespawnCount = 0;

        public static bool isSpawnable;

        private static System.Random rand = new System.Random();

        private static Vector3 shSpawnPos = new Vector3(0, 1002, 8);

        public void OnWaitingForPlayers()
        {
            instance = this;

            shPlayers.Clear();
            shPocketPlayers.Clear();
            teamRespawnCount = 0;
            serpentsRespawnCount = 0;
        }

        public void CalculateChance()
        {
            if (rand.Next(1, 101) <= plugin.Config.SpawnManager.SpawnChance &&
                Player.List.Count() > 0 &&
                teamRespawnCount >= plugin.Config.SpawnManager.RespawnDelay &&
                serpentsRespawnCount < plugin.Config.SpawnManager.MaxSpawns)
            {
                isSpawnable = true;
            }

            else
                isSpawnable = false;

            Log.Debug($"Is Serpents Hand team now spawnable?: {isSpawnable}");
        }

        public void OnTeamRespawn(RespawningTeamEventArgs ev)
        {
            teamRespawnCount++;

            if (ev.NextKnownTeam == Respawning.SpawnableTeamType.ChaosInsurgency)
            {
                if (isSpawnable)
                {
                    ev.IsAllowed = false;

                    List<Player> SHPlayers = new List<Player>();

                    for (int i = 0; i < plugin.Config.SpawnManager.MaxSquad && ev.Players.Count > 0; i++)
                    {
                        Player player = ev.Players[rand.Next(ev.Players.Count)];
                        SHPlayers.Add(player);
                        ev.Players.Remove(player);
                    }
                    Timing.CallDelayed(0.1f, () =>
                    {
                        if (!isSpawnable)
                            SHPlayers.Clear();

                        if (isSpawnable)
                        {
                            SpawnSquad(SHPlayers);
                            serpentsRespawnCount++;
                        }
                    });
                }
                else
                {
                    string ann = plugin.Config.SpawnManager.CiEntryAnnouncement;
                    if (ann != string.Empty)
                    {
                        Cassie.GlitchyMessage(ann, 0.05f, 0.05f);
                    }
                }
            }
        }

        public void OnPocketDimensionEnter(EnteringPocketDimensionEventArgs ev)
        {
            if (shPlayers.Contains(ev.Player.Id))
            {
                shPocketPlayers.Add(ev.Player.Id);
            }
        }

        public void OnPocketDimensionFail(FailingEscapePocketDimensionEventArgs ev)
        {
            if (shPlayers.Contains(ev.Player.Id))
            {
                if (!plugin.Config.SerepentsHandModifiers.FriendlyFire)
                {
                    ev.IsAllowed = false;
                }
                if (plugin.Config.SerepentsHandModifiers.TeleportTo106)
                {
                    TeleportTo106(ev.Player);
                }
                shPocketPlayers.Remove(ev.Player.Id);
            }
        }

        public void OnPocketDimensionEscape(EscapingPocketDimensionEventArgs ev)
        {
            if (shPlayers.Contains(ev.Player.Id))
            {
                ev.IsAllowed = false;
                if (plugin.Config.SerepentsHandModifiers.TeleportTo106)
                {
                    TeleportTo106(ev.Player);
                }
                shPocketPlayers.Remove(ev.Player.Id);
            }
        }

        public void OnPlayerHurt(HurtingEventArgs ev)
        {
            Player scp035 = null;

            if (SerpentsHand.isScp035)
            {
                scp035 = TryGet035();
            }

            if (((shPlayers.Contains(ev.Target.Id) && (ev.Attacker.Team == Team.SCP || ev.HitInformations.GetDamageType() == DamageTypes.Pocket)) ||
                (shPlayers.Contains(ev.Attacker.Id) && (ev.Target.Team == Team.SCP || (scp035 != null && ev.Target == scp035))) ||
                (shPlayers.Contains(ev.Target.Id) && shPlayers.Contains(ev.Attacker.Id) && ev.Target != ev.Attacker)) && !plugin.Config.SerepentsHandModifiers.FriendlyFire)
            {
                ev.IsAllowed = false;
            }
        }

        public void OnPlayerDeath(DiedEventArgs ev)
        {
            if (shPlayers.Contains(ev.Target.Id))
            {
                ev.Target.CustomInfo = string.Empty;
                ev.Target.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
                shPlayers.Remove(ev.Target.Id);
            }

            if (ev.Target.Role == RoleType.Scp106 && !plugin.Config.SerepentsHandModifiers.FriendlyFire)
            {
                foreach (Player player in Player.List.Where(x => shPocketPlayers.Contains(x.Id)))
                {
                    player.ReferenceHub.playerStats.HurtPlayer(new PlayerStats.HitInfo(50000, "WORLD", ev.HitInformations.GetDamageType(), player.Id), player.GameObject);
                }
            }
        }

        public void OnCheckRoundEnd(EndingRoundEventArgs ev)
        {
            Player scp035 = null;

            if (SerpentsHand.isScp035)
            {
                scp035 = TryGet035();
            }

            bool MTFAlive = CountRoles(Team.MTF) > 0;
            bool CiAlive = CountRoles(Team.CHI) > 0;
            bool ScpAlive = CountRoles(Team.SCP) + (scp035 != null && scp035.Role != RoleType.Spectator ? 1 : 0) > 0;
            bool DClassAlive = CountRoles(Team.CDP) > 0;
            bool ScientistsAlive = CountRoles(Team.RSC) > 0;
            bool SHAlive = shPlayers.Count > 0;

            if (SHAlive && ((CiAlive && !plugin.Config.SerepentsHandModifiers.ScpsWinWithChaos) || DClassAlive || MTFAlive || ScientistsAlive))
            {
                ev.IsAllowed = false;
            }
            else if (SHAlive && ScpAlive && !MTFAlive && !DClassAlive && !ScientistsAlive)
            {
                if (!plugin.Config.SerepentsHandModifiers.ScpsWinWithChaos)
                {
                    if (!CiAlive)
                    {
                        ev.LeadingTeam = Exiled.API.Enums.LeadingTeam.Anomalies;
                        ev.IsAllowed = true;
                        ev.IsRoundEnded = true;

                        if (plugin.Config.SerepentsHandModifiers.ScpsWinWithChaos)
                            GrantFF();
                    }
                }
                else
                {
                    ev.LeadingTeam = Exiled.API.Enums.LeadingTeam.Anomalies;
                    ev.IsAllowed = true;
                    ev.IsRoundEnded = true;

                    if (plugin.Config.SerepentsHandModifiers.EndRoundFriendlyFire)
                        GrantFF();
                }
            }
            else if (SHAlive && !ScpAlive && !MTFAlive && !DClassAlive && !ScientistsAlive)
            {
                if (plugin.Config.SerepentsHandModifiers.EndRoundFriendlyFire)
                    GrantFF();
            }
        }

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (shPlayers.Contains(ev.Player.Id))
            {
                if (ev.NewRole != RoleType.Tutorial)
                {
                    shPlayers.Remove(ev.Player.Id);
                    ev.Player.CustomInfo = string.Empty;
                    ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Nickname;
                    ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
                }
                else
                {
                    string RoleName = string.Empty;

                    if (!string.IsNullOrEmpty(plugin.Config.SerepentsHandModifiers.RoleColor))
                        RoleName += ($"<color={plugin.Config.SerepentsHandModifiers.RoleColor}>");

                    RoleName += $"{ev.Player.Nickname}\n{plugin.Config.SerepentsHandModifiers.RoleName}";

                    if (!string.IsNullOrEmpty(plugin.Config.SerepentsHandModifiers.RoleColor))
                        RoleName += "</color>";

                    ev.Player.CustomInfo = RoleName;
                    ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
                    ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;
                }
            }
        }

        public void OnShooting(ShootingEventArgs ev)
        {
            Player target = Player.Get(ev.Target);
            if (target != null && target.Role == RoleType.Scp096 && shPlayers.Contains(ev.Shooter.Id))
            {
                ev.IsAllowed = false;
            }
        }

        public void OnDestroying(DestroyingEventArgs ev)
        {
            if (shPlayers.Contains(ev.Player.Id))
            {
                shPlayers.Remove(ev.Player.Id);
                ev.Player.CustomInfo = string.Empty;
                ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Nickname;
                ev.Player.ReferenceHub.nicknameSync.ShownPlayerInfo |= PlayerInfoArea.Role;
            }
        }

        public void OnContaining106(ContainingEventArgs ev)
        {
            if (shPlayers.Contains(ev.Player.Id) && !plugin.Config.SerepentsHandModifiers.FriendlyFire)
            {
                ev.IsAllowed = false;
            }
        }

        // Needs to be removed and redone with new command system.
        /*
        public void OnRACommand(SendingRemoteAdminCommandEventArgs ev)
        {
            string cmd = ev.Name.ToLower();
            if (cmd == "spawnsh")
            {
                ev.IsAllowed = false;

                if (ev.Arguments.Count > 0 && ev.Arguments[0].Length > 0)
                {
                    Player cPlayer = Player.Get(ev.Arguments[0]);
                    if (cPlayer != null)
                    {
                        SpawnPlayer(cPlayer);
                        ev.Sender.RemoteAdminMessage($"Spawned {cPlayer.Nickname} as Serpents Hand.", true);
                        return;
                    }
                    else
                    {
                        ev.Sender.RemoteAdminMessage("Invalid player.", false);
                        return;
                    }
                }
                else
                {
                    ev.Sender.RemoteAdminMessage("SPAWNSH [Player Name / Player ID]", false);
                }
            }
            else if (cmd == "spawnshsquad")
            {
                ev.IsAllowed = false;

                if (ev.Arguments.Count > 0)
                {
                    if (int.TryParse(ev.Arguments[0], out int a))
                    {
                        CreateSquad(a);
                    }
                    else
                    {
                        ev.Sender.RemoteAdminMessage("Error: invalid size.", false);
                        return;
                    }
                }
                else
                {
                    CreateSquad(5);
                }
                Cassie.Message(plugin.Config.SpawnManager.EntryAnnouncement, true, true);
                ev.Sender.RemoteAdminMessage("Spawned squad.", true);
            }
        }
        */

        public void OnGeneratorInsert(InsertingGeneratorTabletEventArgs ev)
        {
            if (shPlayers.Contains(ev.Player.Id) && !plugin.Config.SerepentsHandModifiers.FriendlyFire)
            {
                ev.IsAllowed = false;
            }
        }

        public void OnFemurEnter(EnteringFemurBreakerEventArgs ev)
        {
            if (shPlayers.Contains(ev.Player.Id) && !plugin.Config.SerepentsHandModifiers.FriendlyFire)
            {
                ev.IsAllowed = false;
            }
        }

        public static int MaxSpawns = SerpentsHand.instance.Config.SpawnManager.MaxSpawns;
    }
}