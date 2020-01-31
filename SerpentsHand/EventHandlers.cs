using System;
using EXILED;
using System.Collections.Generic;

namespace SerpentsHand
{
    partial class EventHandlers
    {
        private Plugin plugin;
        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        private List<int> shPlayers = new List<int>();
        private List<int> shPocketPlayers = new List<int>();

        private bool refreshPlayers;
        private bool isRoundStarted = false;

        private int respawnCount = 0;

        private Random rand = new Random();

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
            if (ev.IsChaos)
            {
                if (rand.Next(1, 101) <= Configs.spawnChance && Plugin.GetHubs().Count > 0 && respawnCount >= Configs.respawnDelay)
                {
                    List<ReferenceHub> SHPlayers = new List<ReferenceHub>();
                    List<ReferenceHub> CIPlayers = ev.ToRespawn;

                    for (int i = 0; i < Configs.maxSquad && CIPlayers.Count > 0; i++)
                    {
                        ReferenceHub player = CIPlayers[rand.Next(CIPlayers.Count)];
                        SHPlayers.Add(player);
                        CIPlayers.Remove(player);
                    }

                    ev.ToRespawn = SHPlayers;

                    Timing.InTicks(() =>
                    {
                        SHPlugin.SpawnSHSquad(ev.PlayerList);
                    }, 4);
                }
                else
                {
                    string ann = Configs.ciEntryAnnouncement;
                    if (ann != "")
                        PluginManager.Manager.Server.Map.AnnounceCustomMessage(ann);
                }
            }
            respawnCount++;
        }

        public void OnPocketDimensionEnter(PocketDimEnterEvent ev)
        {
            if (shPlayers.Contains(ev.Player.GetPlayerID()))
            {
                shPocketPlayers.Add(ev.Player.GetPlayerID());
            }
        }

        public void OnPocketDimensionDie(PocketDimDeathEvent ev)
        {
            if (shPlayers.Contains(ev.Player.GetPlayerID()))
            {
                if (!Configs.friendlyFire)
                {
                    ev.Allow = false;
                }
                if (Configs.teleportTo106)
                {
                    SHPlugin.TeleportTo106(ev.Player);
                }
                shPocketPlayers.Remove(ev.Player.GetPlayerID());
            }
        }

        public void OnPocketDimensionExit(PocketDimEscapedEvent ev)
        {
            if (shPlayers.Contains(ev.Player.GetPlayerID()))
            {
                if (Configs.teleportTo106)
                {
                    SHPlugin.TeleportTo106(ev.Player);
                }
                shPocketPlayers.Remove(ev.Player.GetPlayerID());
            }
        }

        public void OnPlayerHurt(ref PlayerHurtEvent ev)
        {
            if (ev.Attacker.GetPlayerID == "0" || !isRoundStarted) return;

            if (((shPlayers.Contains(ev.Player.GetPlayerID()) && (ev.Attacker.GetTeam() == Team.SCP || ev.DamageType == DamageType.POCKET)) ||
                (shPlayers.Contains(ev.Attacker.GetPlayerID()) && ev.Player.GetTeam() == Team.SCP) ||
                (shPlayers.Contains(ev.Player.GetPlayerID()) && shPlayers.Contains(ev.Attacker.GetPlayerID()) &&
                ev.Player.SteamId != ev.Attacker.GetPlayerID())) && !Configs.friendlyFire)
            {
                ev.Damage = 0;
            }
        }

        public void OnPlayerDie(ref PlayerDeathEvent ev)
        {
            if (shPlayers.Contains(ev.Player.GetPlayerID()))
            {
                shPlayers.Remove(ev.Player.GetPlayerID());
            }

            if (ev.Player.GetRole() == Role.SCP_106 && !Configs.friendlyFire)
            {
                foreach (ReferenceHub player in Plugin.GetHubs().Where(x => shPockerPlayers.Contains(x.GetPlayerID())))
                {
                    player.Kill();
                }
            }
        }

        public void OnCheckRoundEnd(ref CheckRoundEndEvent ev)
        {
            if (refreshPlayers)
            {
                refreshPlayers = false;

                string[] curPlayers = Plugin.GetHubs().GetPlayerID()).ToArray();
                shPlayers.RemoveAll(x => !curPlayers.Contains(x));
            }

            bool MTFAlive = CountRoles(Smod2.API.Team.NINETAILFOX) > 0;
            bool CiAlive = CountRoles(Smod2.API.Team.CHAOS_INSURGENCY) > 0;
            bool ScpAlive = CountRoles(Smod2.API.Team.SCP) > 0;
            bool DClassAlive = CountRoles(Smod2.API.Team.CLASSD) > 0;
            bool ScientistsAlive = CountRoles(Smod2.API.Team.SCIENTIST) > 0;
            bool SHAlive = shPlayers.Count > 0;

            if (MTFAlive && (CiAlive || ScpAlive || DClassAlive || SHAlive))
            {
                ev.ForceEnd = false;
            }
            else if (CiAlive && (MTFAlive || (DClassAlive && ScpAlive) || ScientistsAlive || SHAlive))
            {
                ev.ForceEnd = false;
            }
            else if (ScpAlive && (MTFAlive || DClassAlive || ScientistsAlive))
            {
                ev.ForceEnd = false;
            }
            else if (SHAlive && ScpAlive && !MTFAlive && !CiAlive && !DClassAlive && !ScientistsAlive)
            {
                ev.LeadingTeam = RoundSummary.LeadingTeam.Anomalies;
                ev.ForceEnd = true;
            }
            else if (CiAlive && ScpAlive && !Configs.ciWinWithScp)
            {
                ev.ForceEnd = false;
            }
        }

        public void OnSetRole(SetClassEvent ev)
        {
            // Figure out how to set spawn position

            if (shPlayers.Contains(ev.Player.GetPlayerID()))
            {
                if (ev.Player.GetTeam() == Team.TUT)
                {
                    ev.Items.Clear();
                    foreach (int a in Configs.spawnItems) ev.Items.Add((ItemType)a);
                }
                else
                {
                    shPlayers.Remove(ev.Player.GetPlayerID());
                }
            }
        }

        public void OnDisconnect(PlayerLeaveEvent ev)
        {
            refreshPlayers = true;
        }

        public void OnContain106(Scp106ContainEvent ev)
        {
            if (shPlayers.Contains(ev.Player.GetPlayerID()))
            {
                ev.Allow = false;
            }
        }

        public void OnRACommand(RACommandEvent ev)
        {
            string cmd = ev.Command.ToLower();
            if (cmd.StartsWith("spawnsh"))
            {
                string[] args = cmd.Replace("spawnsh", "").Trim().Split(' ');

                if (args.Length > 0)
                {
                    ReferenceHub cPlayer = Plugin.GetPlayer(args[0]);
                    if (cPlayer != null)
                    {
                        SpawnPlayer(cPlayer);
                        ev.Sender($"Spawned {cPlayer.GetName()}");
                    }
                    else
                    {
                        ev.Sender.RAReply("Invalid player.");
                    }
                }
                ev.Sender("SPAWNSH [Player Name / Player ID]");
            }
            else if (cmd.StartsWith("spawnshsquad"))
            {
                string[] args = cmd.Replace("spawnshsquad", "").Trim().Split(' ');

                if (args.Length > 0)
                {
                    if (int.TryParse(args[0], out int a))
                    {
                        SpawnSquad(a);
                    }
                    else
                    {
                        return new string[] { "Error: invalid size." };
                    }
                }
                else
                {
                    SHPlugin.SpawnSquad(5);
                }
                PluginManager.Manager.Server.Map.AnnounceCustomMessage(SHPlugin.shAnnouncement);
                return new string[] { "Spawned squad." };
            }
        }
    }
}
