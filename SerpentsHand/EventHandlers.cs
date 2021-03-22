namespace SerpentsHand
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using MEC;
    using UnityEngine;

    /// <summary>
    /// EventHandlers and Logic which Serpents Hand use.
    /// </summary>
    public partial class EventHandlers
    {
        private readonly SerpentsHand plugin;

        /// <inheritdoc/>
        public EventHandlers(SerpentsHand plugin) => this.plugin = plugin;

        /// <summary>
        /// The <see cref="EventHandlers"></see> <see langword="static"/> instance.
        /// </summary>
        internal static EventHandlers Instance;

        /// <summary>
        /// Players IDs that are currently Serpents Hand.
        /// </summary>
        public static List<int> ShPlayers = new List<int>();

        /// <summary>
        /// How many respaws have occured.
        /// </summary>
        public static int TeamRespawnCount = 0;

        /// <summary>
        /// How many times Serpents Hand team have been spawned by <see cref="Exiled.Events.Handlers.Server.OnRespawningTeam(RespawningTeamEventArgs)"/>.
        /// </summary>
        public static int SerpentsRespawnCount = 0;

        /// <summary>
        /// Is Serpents Hand spawnable in <see cref="Exiled.Events.Handlers.Server.OnRespawningTeam(RespawningTeamEventArgs)"/>.
        /// </summary>
        public static bool IsSpawnable;

        private static System.Random rand = new System.Random();

        private static Vector3 shSpawnPos;

        private List<int> shPocketPlayers = new List<int>();

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers()"/>
        internal void OnWaitingForPlayers()
        {
            Instance = this;

            shSpawnPos = new Vector3(plugin.Config.SpawnManager.SpawnPos["X"], plugin.Config.SpawnManager.SpawnPos["Y"], plugin.Config.SpawnManager.SpawnPos["Z"]);

            ShPlayers.Clear();
            shPocketPlayers.Clear();
            TeamRespawnCount = 0;
            SerpentsRespawnCount = 0;
        }

        /// <summary>
        /// Handles Serpents Hand spawn chance with all other conditions.
        /// </summary>
        internal void CalculateChance()
        {
            Log.Info(Convert.ToInt32(TryGet035() != null));

            if (rand.Next(1, 101) <= plugin.Config.SpawnManager.SpawnChance &&
                TeamRespawnCount >= plugin.Config.SpawnManager.RespawnDelay &&
                SerpentsRespawnCount < plugin.Config.SpawnManager.MaxSpawns &&
                !(!plugin.Config.SpawnManager.CanSpawnWithoutSCPs && Player.Get(Team.SCP).Count() + Convert.ToInt32(TryGet035() != null) == 0))
            {
                IsSpawnable = true;
            }
            else
            {
                IsSpawnable = false;
            }

            Log.Debug($"Is Serpents Hand team now spawnable?: {IsSpawnable}", plugin.Config.Debug);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRespawningTeam(RespawningTeamEventArgs)"/>
        internal void OnTeamRespawn(RespawningTeamEventArgs ev)
        {
            TeamRespawnCount++;

            if (ev.NextKnownTeam == Respawning.SpawnableTeamType.ChaosInsurgency)
            {
                if (IsSpawnable)
                {
                    ev.IsAllowed = false;

                    List<Player> sHPlayers = new List<Player>();

                    for (int i = 0; i < plugin.Config.SpawnManager.MaxSquad && ev.Players.Count > 0; i++)
                    {
                        Player player = ev.Players[rand.Next(ev.Players.Count)];
                        sHPlayers.Add(player);
                        ev.Players.Remove(player);
                    }

                    Timing.CallDelayed(0.1f, () =>
                    {
                        if (!IsSpawnable)
                            sHPlayers.Clear();

                        if (IsSpawnable)
                        {
                            SpawnSquad(sHPlayers);

                            if (plugin.Config.SpawnManager.MaxSpawns > 0)
                                SerpentsRespawnCount++;
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

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnEnteringPocketDimension(EnteringPocketDimensionEventArgs)"/>
        internal void OnPocketDimensionEnter(EnteringPocketDimensionEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Player.Id))
            {
                shPocketPlayers.Add(ev.Player.Id);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs)"/>
        internal void OnPocketDimensionFail(FailingEscapePocketDimensionEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Player.Id))
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

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnEscapingPocketDimension(EscapingPocketDimensionEventArgs)"/>
        internal void OnPocketDimensionEscape(EscapingPocketDimensionEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Player.Id))
            {
                ev.IsAllowed = false;
                if (plugin.Config.SerepentsHandModifiers.TeleportTo106)
                {
                    TeleportTo106(ev.Player);
                }

                shPocketPlayers.Remove(ev.Player.Id);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnHurting(HurtingEventArgs)"/>
        internal void OnPlayerHurt(HurtingEventArgs ev)
        {
            Player scp035 = null;

            if (SerpentsHand.IsScp035)
            {
                scp035 = TryGet035();
            }

            if (((ShPlayers.Contains(ev.Target.Id) && (ev.Attacker.Team == Team.SCP || ev.HitInformations.GetDamageType() == DamageTypes.Pocket)) ||
                (ShPlayers.Contains(ev.Attacker.Id) && (ev.Target.Team == Team.SCP || (scp035 != null && ev.Target == scp035))) ||
                (ShPlayers.Contains(ev.Target.Id) && ShPlayers.Contains(ev.Attacker.Id) && ev.Target != ev.Attacker)) && !plugin.Config.SerepentsHandModifiers.FriendlyFire)
            {
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnEndingRound(EndingRoundEventArgs)"/>
        internal void OnCheckRoundEnd(EndingRoundEventArgs ev)
        {
            Player scp035 = null;

            if (SerpentsHand.IsScp035)
            {
                scp035 = TryGet035();
            }

            bool mtfAlive = CountRoles(Team.MTF) > 0;
            bool ciAlive = CountRoles(Team.CHI) > 0;
            bool scpAlive = CountRoles(Team.SCP) + (scp035 != null && scp035.Role != RoleType.Spectator ? 1 : 0) > 0;
            bool dclassAlive = CountRoles(Team.CDP) > 0;
            bool scientistsAlive = CountRoles(Team.RSC) > 0;
            bool shAlive = ShPlayers.Count > 0;

            if (shAlive && ((ciAlive && !plugin.Config.SerepentsHandModifiers.ScpsWinWithChaos) || dclassAlive || mtfAlive || scientistsAlive))
            {
                ev.IsAllowed = false;
            }
            else if (shAlive && scpAlive && !mtfAlive && !dclassAlive && !scientistsAlive)
            {
                if (!plugin.Config.SerepentsHandModifiers.ScpsWinWithChaos)
                {
                    if (!ciAlive)
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
            else if (shAlive && !scpAlive && !mtfAlive && !dclassAlive && !scientistsAlive)
            {
                if (plugin.Config.SerepentsHandModifiers.EndRoundFriendlyFire)
                    GrantFF();
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnShooting(ShootingEventArgs)"/>
        internal void OnShooting(ShootingEventArgs ev)
        {
            Player target = Player.Get(ev.Target);
            if (target != null && target.Role == RoleType.Scp096 && ShPlayers.Contains(ev.Shooter.Id))
            {
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Scp106.OnContaining(ContainingEventArgs)"/>
        internal void OnContaining106(ContainingEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Player.Id) && !plugin.Config.SerepentsHandModifiers.FriendlyFire)
            {
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnInsertingGeneratorTablet(InsertingGeneratorTabletEventArgs)"/>
        internal void OnGeneratorInsert(InsertingGeneratorTabletEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Player.Id) && !plugin.Config.SerepentsHandModifiers.FriendlyFire)
            {
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnEnteringFemurBreaker(EnteringFemurBreakerEventArgs)"/>
        internal void OnFemurEnter(EnteringFemurBreakerEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Player.Id) && !plugin.Config.SerepentsHandModifiers.FriendlyFire)
            {
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDestroying(DestroyingEventArgs)"/>
        internal void OnDestroying(DestroyingEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Player.Id))
            {
                DestroySH(ev.Player);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDied(DiedEventArgs)"/>
        internal void OnPlayerDeath(DiedEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Target.Id))
            {
                DestroySH(ev.Target);
                return;
            }

            if (ev.Target.Role == RoleType.Scp106 && !plugin.Config.SerepentsHandModifiers.FriendlyFire)
            {
                foreach (Player player in Player.List.Where(x => shPocketPlayers.Contains(x.Id)))
                {
                    player.Hurt(50000f, DamageTypes.Contain, "WORLD", player.Id);
                }
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnChangingRole(ChangingRoleEventArgs)"/>
        internal void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Player.Id) && ev.NewRole != RoleType.Tutorial)
            {
                DestroySH(ev.Player);
            }
        }
    }
}