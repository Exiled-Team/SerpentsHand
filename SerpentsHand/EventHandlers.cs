using System;
using EXILED;
using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;
using EXILED.Extensions;
using System.IO;

namespace SerpentsHand
{
    public partial class EventHandlers
    {
        public static List<ReferenceHub> shPlayers = new List<ReferenceHub>();
        private List<ReferenceHub> shPocketPlayers = new List<ReferenceHub>();

        private int respawnCount = 0;

        private static System.Random rand = new System.Random();

        private static Vector3 shSpawnPos = new Vector3(0, 1001, 8);

        public void OnWaitingForPlayers()
        {
            Configs.ReloadConfigs();
        }

        public void OnRoundStart()
        {
            shPlayers.Clear();
            shPocketPlayers.Clear();
            respawnCount = 0;
        }

        public void OnTeamRespawn(ref TeamRespawnEvent ev)
        {
            if (ev.IsChaos)
            {
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
            if (shPlayers.Contains(ev.Player))
            {
                shPocketPlayers.Add(ev.Player);
            }
        }

        public void OnPocketDimensionDie(PocketDimDeathEvent ev)
        {
            if (shPlayers.Contains(ev.Player))
            {
                if (!Configs.friendlyFire)
                {
                    ev.Allow = false;
                }
                if (Configs.teleportTo106)
                {
                    TeleportTo106(ev.Player);
                }
                shPocketPlayers.Remove(ev.Player);
            }
        }

        public void OnPocketDimensionExit(PocketDimEscapedEvent ev)
        {
            if (shPlayers.Contains(ev.Player))
            {
                ev.Allow = false;
                if (Configs.teleportTo106)
                {
                    TeleportTo106(ev.Player);
                }
                shPocketPlayers.Remove(ev.Player);
            }
        }

        public void OnPlayerHurt(ref PlayerHurtEvent ev)
        {
            ReferenceHub scp035 = null;

            if (Plugin.isScp035)
            {
                scp035 = TryGet035();
            }

            if (((shPlayers.Contains(ev.Player) && (ev.Attacker.GetTeam() == Team.SCP || ev.Info.GetDamageType() == DamageTypes.Pocket)) ||
                (shPlayers.Contains(ev.Attacker) && (ev.Player.GetTeam() == Team.SCP || (scp035 != null && ev.Player == scp035))) ||
                (shPlayers.Contains(ev.Player) && shPlayers.Contains(ev.Attacker) && ev.Player != ev.Attacker)) && !Configs.friendlyFire)
            {
                ev.Amount = 0f;
            }
        }

        public void OnPlayerDie(ref PlayerDeathEvent ev)
        {
            if (shPlayers.Contains(ev.Player))
            {
                shPlayers.Remove(ev.Player);
            }

            if (ev.Player.characterClassManager.CurClass == RoleType.Scp106 && !Configs.friendlyFire)
            {
                foreach (ReferenceHub player in Player.GetHubs().Where(x => shPocketPlayers.Contains(x)))
                {
                    player.playerStats.HurtPlayer(new PlayerStats.HitInfo(50000, "WORLD", ev.Info.GetDamageType(), player.queryProcessor.PlayerId), player.gameObject);
                }
            }
        }

        public void OnCheckRoundEnd(ref CheckRoundEndEvent ev)
        {
            ReferenceHub scp035 = null;

            if (Plugin.isScp035)
            {
                scp035 = TryGet035();
            }

            bool MTFAlive = CountRoles(Team.MTF) > 0;
            bool CiAlive = CountRoles(Team.CHI) > 0;
            bool ScpAlive = CountRoles(Team.SCP) + (scp035 != null && scp035.characterClassManager.CurClass != RoleType.Spectator ? 1 : 0) > 0;
            bool DClassAlive = CountRoles(Team.CDP) > 0;
            bool ScientistsAlive = CountRoles(Team.RSC) > 0;
            bool SHAlive = shPlayers.Count > 0;

            if (SHAlive && ((CiAlive && !Configs.scpsWinWithChaos) || DClassAlive || MTFAlive || ScientistsAlive))
            {
                ev.Allow = false;
            }
            else if (SHAlive && ScpAlive && !MTFAlive && !DClassAlive && !ScientistsAlive)
            {
                if (!Configs.scpsWinWithChaos)
                {
                    if (!CiAlive)
                    {
                        ev.LeadingTeam = RoundSummary.LeadingTeam.Anomalies;
                        ev.Allow = true;
                        ev.ForceEnd = true;
                    }
                }
                else
                {
                    ev.LeadingTeam = RoundSummary.LeadingTeam.Anomalies;
                    ev.Allow = true;
                    ev.ForceEnd = true;
                }
            }
        }

        public void OnSetRole(SetClassEvent ev)
        {
            if (shPlayers.Contains(ev.Player))
            {
                if (ev.Role.GetTeam() != Team.TUT)
                { 
                    shPlayers.Remove(ev.Player);
                }
            }
        }

        public void OnDisconnect(PlayerLeaveEvent ev)
        {
            if (shPlayers.Contains(ev.Player))
            {
                shPlayers.Remove(ev.Player);
            }
        }

        public void OnContain106(Scp106ContainEvent ev)
        {
            if (shPlayers.Contains(ev.Player) && !Configs.friendlyFire)
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
                        ev.Sender.RAMessage($"Spawned {cPlayer.nicknameSync.Network_myNickSync} as Serpents Hand.", true);
                        return;
                    }
                    else
                    {
                        ev.Sender.RAMessage("Invalid player.", false);
                        return;
                    }
                }
                else
                {
                    ev.Sender.RAMessage("SPAWNSH [Player Name / Player ID]", false);
                }
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
                        ev.Sender.RAMessage("Error: invalid size.", false);
                        return;
                    }
                }
                else
                {
                    CreateSquad(5);
                }
                Cassie.CassieMessage(Configs.entryAnnouncement, true, true);
                ev.Sender.RAMessage("Spawned squad.", true);
            }
        }

        public void OnGeneratorInsert(ref GeneratorInsertTabletEvent ev)
        {
            if (shPlayers.Contains(ev.Player) && !Configs.friendlyFire)
            {
                ev.Allow = false;
            }
        }

        public void OnFemurEnter(FemurEnterEvent ev)
        {
            if (shPlayers.Contains(ev.Player) && !Configs.friendlyFire)
            {
                ev.Allow = false;
            }
        }
    }
}
