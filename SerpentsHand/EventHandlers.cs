namespace SerpentsHand
{
    #pragma warning disable SA1202
    #pragma warning disable SA1311

    using System.Collections.Generic;
    using System.Linq;
    using Configs;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using MEC;
    using Respawning;

    /// <summary>
    /// EventHandlers and Logic which Serpents Hand uses.
    /// </summary>
    public partial class EventHandlers
    {
        private static readonly Config Config = SerpentsHand.Instance.Config;

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

        private static readonly System.Random rng = new System.Random();

        private static List<int> shPocketPlayers = new List<int>();

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers()"/>
        internal static void OnWaitingForPlayers()
        {
            ShPlayers.Clear();
            shPocketPlayers.Clear();
            TeamRespawnCount = 0;
            SerpentsRespawnCount = 0;
        }

        /// <summary>
        /// Handles Serpents Hand spawn chance with all other conditions.
        /// </summary>
        internal static void CalculateChance()
        {
            int scp035num = 0;

            if (SerpentsHand.IsScp035)
            {
                if (TryGet035() != null)
                    scp035num = 1;
            }

            IsSpawnable = rng.Next(1, 101) <= Config.SpawnManager.SpawnChance &&
                TeamRespawnCount >= Config.SpawnManager.RespawnDelay &&
                SerpentsRespawnCount < Config.SpawnManager.MaxSpawns &&
                !(!Config.SpawnManager.CanSpawnWithoutScps && Player.Get(Team.SCP).Count() + scp035num == 0);

            Log.Debug($"Is Serpents Hand team now spawnable?: {IsSpawnable}", Config.Debug);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRespawningTeam(RespawningTeamEventArgs)"/>
        internal static void OnTeamRespawn(RespawningTeamEventArgs ev)
        {
            TeamRespawnCount++;

            if (ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency)
            {
                if (IsSpawnable)
                {
                    ev.IsAllowed = false;

                    bool prioritySpawn = RespawnManager.Singleton._prioritySpawn;

                    if (prioritySpawn)
                        ev.Players.OrderBy((x) => x.ReferenceHub.characterClassManager.DeathTime);

                    List<Player> sHPlayers = new List<Player>();

                    for (int i = 0; i < Config.SpawnManager.MaxSquad && ev.Players.Count > 0; i++)
                    {
                        Player player = prioritySpawn ? ev.Players.First() : ev.Players[rng.Next(ev.Players.Count)];
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

                            if (Config.SpawnManager.MaxSpawns > 0)
                                SerpentsRespawnCount++;
                        }
                    });
                }
                else
                {
                    string ann = Config.SpawnManager.CiEntryAnnouncement;
                    if (ann != string.Empty)
                    {
                        Cassie.GlitchyMessage(ann, 0.05f, 0.05f);
                    }
                }
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnEnteringPocketDimension(EnteringPocketDimensionEventArgs)"/>
        internal static void OnPocketDimensionEnter(EnteringPocketDimensionEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Player.Id))
            {
                shPocketPlayers.Add(ev.Player.Id);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs)"/>
        internal static void OnPocketDimensionFail(FailingEscapePocketDimensionEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Player.Id))
            {
                if (!Config.SerepentsHandModifiers.FriendlyFire)
                {
                    ev.IsAllowed = false;
                }

                if (Config.SerepentsHandModifiers.TeleportTo106)
                {
                    TeleportTo106(ev.Player);
                }

                shPocketPlayers.Remove(ev.Player.Id);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnEscapingPocketDimension(EscapingPocketDimensionEventArgs)"/>
        internal static void OnPocketDimensionEscape(EscapingPocketDimensionEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Player.Id))
            {
                ev.IsAllowed = false;
                if (Config.SerepentsHandModifiers.TeleportTo106)
                {
                    TeleportTo106(ev.Player);
                }

                shPocketPlayers.Remove(ev.Player.Id);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnHurting(HurtingEventArgs)"/>
        internal static void OnPlayerHurt(HurtingEventArgs ev)
        {
            Player scp035 = null;

            if (SerpentsHand.IsScp035)
            {
                scp035 = TryGet035();
            }

            if (((ShPlayers.Contains(ev.Target.Id) && (ev.Attacker.Team == Team.SCP || ev.HitInformations.GetDamageType() == DamageTypes.Pocket)) ||
                (ShPlayers.Contains(ev.Attacker.Id) && (ev.Target.Team == Team.SCP || (scp035 != null && ev.Target == scp035))) ||
                (ShPlayers.Contains(ev.Target.Id) && ShPlayers.Contains(ev.Attacker.Id) && ev.Target != ev.Attacker)) && !Config.SerepentsHandModifiers.FriendlyFire)
            {
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnEndingRound(EndingRoundEventArgs)"/>
        internal static void OnCheckRoundEnd(EndingRoundEventArgs ev)
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

            if (shAlive && ((ciAlive && !Config.SerepentsHandModifiers.ScpsWinWithChaos) || dclassAlive || mtfAlive || scientistsAlive))
            {
                ev.IsAllowed = false;
            }
            else if (shAlive && scpAlive && !mtfAlive && !dclassAlive && !scientistsAlive)
            {
                if (!Config.SerepentsHandModifiers.ScpsWinWithChaos)
                {
                    if (!ciAlive)
                    {
                        ev.LeadingTeam = LeadingTeam.Anomalies;
                        ev.IsAllowed = true;
                        ev.IsRoundEnded = true;

                        if (Config.SerepentsHandModifiers.ScpsWinWithChaos)
                            GrantFF();
                    }
                }
                else
                {
                    ev.LeadingTeam = LeadingTeam.Anomalies;
                    ev.IsAllowed = true;
                    ev.IsRoundEnded = true;

                    if (Config.SerepentsHandModifiers.EndRoundFriendlyFire)
                        GrantFF();
                }
            }
            else if (shAlive && !scpAlive && !mtfAlive && !dclassAlive && !scientistsAlive)
            {
                if (Config.SerepentsHandModifiers.EndRoundFriendlyFire)
                    GrantFF();
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnShooting(ShootingEventArgs)"/>
        internal static void OnShooting(ShootingEventArgs ev)
        {
            Player target = Player.Get(ev.Target);
            if (target != null && target.Role == RoleType.Scp096 && ShPlayers.Contains(ev.Shooter.Id))
            {
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Scp106.OnContaining(ContainingEventArgs)"/>
        internal static void OnContaining106(ContainingEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Player.Id) && !Config.SerepentsHandModifiers.FriendlyFire)
            {
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnInsertingGeneratorTablet(InsertingGeneratorTabletEventArgs)"/>
        internal static void OnGeneratorInsert(InsertingGeneratorTabletEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Player.Id) && !Config.SerepentsHandModifiers.FriendlyFire)
            {
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnEnteringFemurBreaker(EnteringFemurBreakerEventArgs)"/>
        internal static void OnFemurEnter(EnteringFemurBreakerEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Player.Id) && !Config.SerepentsHandModifiers.FriendlyFire)
            {
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDestroying(DestroyingEventArgs)"/>
        internal static void OnDestroying(DestroyingEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Player.Id))
            {
                DestroySH(ev.Player);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDied(DiedEventArgs)"/>
        internal static void OnPlayerDeath(DiedEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Target.Id))
            {
                DestroySH(ev.Target);
                return;
            }

            if (ev.Target.Role == RoleType.Scp106 && !Config.SerepentsHandModifiers.FriendlyFire)
            {
                foreach (Player player in Player.List.Where(x => shPocketPlayers.Contains(x.Id)))
                {
                    player.Hurt(50000f, DamageTypes.Contain, "WORLD", player.Id);
                }
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnChangingRole(ChangingRoleEventArgs)"/>
        internal static void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ShPlayers.Contains(ev.Player.Id) && ev.NewRole != RoleType.Tutorial)
            {
                DestroySH(ev.Player);
            }
        }
    }
}