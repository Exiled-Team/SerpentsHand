using System;
using EXILED;
using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;
using scp035.API;
using EXILED.Extensions;

namespace SerpentsHand
{
    public partial class EventHandlers
    {
        private static List<int> shPlayers = new List<int>();
        private List<int> shPocketPlayers = new List<int>();

        private bool isRoundStarted = false;

        private int respawnCount = 0;

        private static System.Random rand = new System.Random();

        private PlayerStats.HitInfo noDamage = new PlayerStats.HitInfo(0, "WORLD", DamageTypes.Nuke, 0);

        private static Vector3 shSpawnPos = new Vector3(0, 1001, 8);

        public void OnWaitingForPlayers()
        {
            Configs.ReloadConfigs();
        }

        public void OnRoundStart()
        {
            shPlayers.Clear();
            shPocketPlayers.Clear();
            isRoundStarted = true;
            respawnCount = 0;
        }

        public void OnRoundEnd()
        {
            isRoundStarted = false;
        }

        public void OnTeamRespawn(ref TeamRespawnEvent ev)
        {
            Log.Info("team respawn");
            if (ev.IsChaos)
            {
                Log.Info((rand.Next(1, 101) <= Configs.spawnChance).ToString());
                Log.Info((Player.GetHubs().Count() > 0).ToString());
                Log.Info((respawnCount >= Configs.respawnDelay).ToString());
                if (rand.Next(1, 101) <= Configs.spawnChance && Player.GetHubs().Count() > 0 && respawnCount >= Configs.respawnDelay)
                {
                    List<ReferenceHub> SHPlayers = new List<ReferenceHub>();
                    List<ReferenceHub> CIPlayers = new List<ReferenceHub>(ev.ToRespawn);
                    ev.ToRespawn.Clear();

                    for (int i = 0; i < Configs.maxSquad && CIPlayers.Count > 0; i++)
                    {
                        ReferenceHub player = CIPlayers[rand.Next(CIPlayers.Count)];
                        SHPlayers.Add(player);
                        CIPlayers.Remove(player);
                    }
                    Timing.CallDelayed(0.1f, () => SpawnSquad(SHPlayers));
                }
                else
                {
                    string ann = Configs.ciEntryAnnouncement;
                    if (ann != string.Empty)
                    {
                        Cassie.CassieMessage(ann, true, true);
                    }
                }
            }
            respawnCount++;
        }

        public void OnPocketDimensionEnter(PocketDimEnterEvent ev)
        {
            if (shPlayers.Contains(ev.Player.queryProcessor.PlayerId))
            {
                shPocketPlayers.Add(ev.Player.queryProcessor.PlayerId);
            }
        }

        public void OnPocketDimensionDie(PocketDimDeathEvent ev)
        {
            if (shPlayers.Contains(ev.Player.queryProcessor.PlayerId))
            {
                if (!Configs.friendlyFire)
                {
                    ev.Allow = false;
                }
                if (Configs.teleportTo106)
                {
                    TeleportTo106(ev.Player);
                }
                shPocketPlayers.Remove(ev.Player.queryProcessor.PlayerId);
            }
        }

        public void OnPocketDimensionExit(PocketDimEscapedEvent ev)
        {
            if (shPlayers.Contains(ev.Player.queryProcessor.PlayerId))
            {
                ev.Allow = false;
                if (Configs.teleportTo106)
                {
                    TeleportTo106(ev.Player);
                }
                shPocketPlayers.Remove(ev.Player.queryProcessor.PlayerId);
            }
        }

        public void OnPlayerHurt(ref PlayerHurtEvent ev)
        {
            if (ev.Attacker.queryProcessor.PlayerId == 0 || !isRoundStarted) return;

            ReferenceHub scp035 = null;

            try
            {
                scp035 = Scp035Data.GetScp035();
            } catch (Exception x)
            {
                Log.Warn("SCP-035 not installed, ignoring API call.");
            }

            if (((shPlayers.Contains(ev.Player.queryProcessor.PlayerId) && (ev.Attacker.GetTeam() == Team.SCP || ev.Info.GetDamageType() == DamageTypes.Pocket)) ||
                (shPlayers.Contains(ev.Attacker.queryProcessor.PlayerId) && (ev.Player.GetTeam() == Team.SCP || (scp035 != null && ev.Attacker.queryProcessor.PlayerId == scp035.queryProcessor.PlayerId))) ||
                (shPlayers.Contains(ev.Player.queryProcessor.PlayerId) && shPlayers.Contains(ev.Attacker.queryProcessor.PlayerId) &&
                ev.Player.queryProcessor.PlayerId != ev.Attacker.queryProcessor.PlayerId)) && !Configs.friendlyFire)
            {
                ev.Info = noDamage;
            }
        }

        public void OnPlayerDie(ref PlayerDeathEvent ev)
        {
            if (shPlayers.Contains(ev.Player.queryProcessor.PlayerId))
            {
                shPlayers.Remove(ev.Player.queryProcessor.PlayerId);
            }

            if (ev.Player.characterClassManager.CurClass == RoleType.Scp106 && !Configs.friendlyFire)
            {
                foreach (ReferenceHub player in Player.GetHubs().Where(x => shPocketPlayers.Contains(x.queryProcessor.PlayerId)))
                {
                    player.playerStats.HurtPlayer(new PlayerStats.HitInfo(50000, "WORLD", ev.Info.GetDamageType(), player.queryProcessor.PlayerId), player.gameObject);
                }
            }
        }

        public void OnCheckRoundEnd(ref CheckRoundEndEvent ev)
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

            bool MTFAlive = CountRoles(Team.MTF) > 0;
            bool CiAlive = CountRoles(Team.CHI) > 0;
            bool ScpAlive = CountRoles(Team.SCP) + (scp035 != null && scp035.characterClassManager.CurClass != RoleType.Spectator ? 1 : 0) > 0;
            bool DClassAlive = CountRoles(Team.CDP) > 0;
            bool ScientistsAlive = CountRoles(Team.RSC) > 0;
            bool SHAlive = shPlayers.Count > 0;

            if (SHAlive && (CiAlive || DClassAlive || MTFAlive || ScientistsAlive))
            {
                ev.Allow = false;
            }
            else if (SHAlive && ScpAlive && !MTFAlive && !CiAlive && !DClassAlive && !ScientistsAlive)
            {
                ev.LeadingTeam = RoundSummary.LeadingTeam.Anomalies;
                ev.ForceEnd = true;
            }
        }

        public void OnSetRole(SetClassEvent ev)
        {
            if (shPlayers.Contains(ev.Player.queryProcessor.PlayerId))
            {
                if (ev.Player.GetTeam() != Team.TUT)
                { 
                    shPlayers.Remove(ev.Player.queryProcessor.PlayerId);
                }
            }
        }

        public void OnDisconnect(PlayerLeaveEvent ev)
        {
            Timing.CallDelayed(1f, () =>
            {
                int[] curPlayers = Player.GetHubs().Select(x => x.queryProcessor.PlayerId).ToArray();
                shPlayers.RemoveAll(x => !curPlayers.Contains(x));
            });
        }

        public void OnContain106(Scp106ContainEvent ev)
        {
            if (shPlayers.Contains(ev.Player.queryProcessor.PlayerId) && !Configs.friendlyFire)
            {
                ev.Allow = false;
            }
        }

        public void OnRACommand(ref RACommandEvent ev)
        {
            string cmd = ev.Command.ToLower();
            if (cmd.StartsWith("spawnsh") && !cmd.StartsWith("spawnshsquad"))
            {
                ev.Allow = false;

                string[] args = cmd.Replace("spawnsh", "").Trim().Split(' ');

                if (args.Length > 0)
                {
                    ReferenceHub cPlayer = Player.GetPlayer(args[0]);
                    if (cPlayer != null)
                    {
                        SpawnPlayer(cPlayer);
                        ev.Sender.RAMessage($"Spawned {cPlayer.nicknameSync.Network_myNickSync} as SerpentsHand", true, "SerpentsHand");
                        return;
                    }
                    else
                    {
                        ev.Sender.RAMessage("Invalid player.", false, "SerpentsHand");
                        return;
                    }
                }
                ev.Sender.RaReply("SPAWNSH [Player Name / Player ID]", true, true, string.Empty);
            }
            else if (cmd.StartsWith("spawnshsquad"))
            {
                ev.Allow = false;

                string[] args = cmd.Replace("spawnshsquad", "").Trim().Split(' ');

                if (args.Length > 0)
                {
                    if (int.TryParse(args[0], out int a))
                    {
                        CreateSquad(a);
                    }
                    else
                    {
                        ev.Sender.RAMessage("Error: invalid size.", false, "SerpentsHand");
                        return;
                    }
                }
                else
                {
                    CreateSquad(5);
                }
                Cassie.CassieMessage(Configs.entryAnnouncement, true, true);
                ev.Sender.RAMessage("Spawned squad.", true, "SerpentsHand");
            }
        }

        public void OnGeneratorInsert(ref GeneratorInsertTabletEvent ev)
        {
            if (shPlayers.Contains(ev.Player.queryProcessor.PlayerId) && !Configs.friendlyFire)
            {
                ev.Allow = false;
            }
        }

        public void OnFemurEnter(FemurEnterEvent ev)
        {
            if (shPlayers.Contains(ev.Player.queryProcessor.PlayerId) && !Configs.friendlyFire)
            {
                ev.Allow = false;
            }
        }
    }
}
