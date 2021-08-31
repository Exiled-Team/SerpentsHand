namespace SerpentsHand
{
    using System.Collections.Generic;
    using System.Linq;
    using Configs;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using MEC;
    using Respawning;
    using UnityEngine;
    using static API;

    /// <summary>
    /// EventHandlers and Logic which Serpents Hand uses.
    /// </summary>
    public partial class EventHandlers
    {
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

        private static List<Player> shPocketPlayers = new List<Player>();

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers()"/>
        internal static void OnWaitingForPlayers()
        {
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

            if (GetScp035s() != null)
                scp035num = 1;

            IsSpawnable = Random.Range(0, 101) <= Config.SpawnManager.SpawnChance &&
                TeamRespawnCount >= Config.SpawnManager.RespawnDelay &&
                SerpentsRespawnCount < Config.SpawnManager.MaxSpawns &&
                !(!Config.SpawnManager.CanSpawnWithoutScps && Player.Get(Team.SCP).Count() + scp035num == 0);

            Log.Debug($"Is Serpents Hand team now spawnable?: {IsSpawnable}", Config.Debug);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRespawningTeam(RespawningTeamEventArgs)"/>
        internal static void OnTeamRespawn(RespawningTeamEventArgs ev)
        {
            TeamRespawnCount++;

            if (ev.NextKnownTeam != SpawnableTeamType.ChaosInsurgency)
                return;

            if (IsSpawnable)
            {
                ev.IsAllowed = false;

                bool prioritySpawn = RespawnManager.Singleton._prioritySpawn;

                if (prioritySpawn)
                    ev.Players.OrderBy((x) => x.ReferenceHub.characterClassManager.DeathTime);

                List<Player> sHPlayers = new List<Player>();

                for (int i = 0; i < Config.SpawnManager.MaxSquad && ev.Players.Count > 0; i++)
                {
                    Player player = prioritySpawn ? ev.Players.First() : ev.Players[Random.Range(0, ev.Players.Count)];
                    sHPlayers.Add(player);
                    ev.Players.Remove(player);
                }

                Timing.CallDelayed(0.1f, () =>
                {
                    SpawnSquad(sHPlayers);

                    if (Config.SpawnManager.MaxSpawns > 0)
                        SerpentsRespawnCount++;

                    IsSpawnable = false;
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

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnEnteringPocketDimension(EnteringPocketDimensionEventArgs)"/>
        internal static void OnPocketDimensionEnter(EnteringPocketDimensionEventArgs ev)
        {
            if (IsSerpent(ev.Player))
            {
                shPocketPlayers.Add(ev.Player);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs)"/>
        internal static void OnPocketDimensionFail(FailingEscapePocketDimensionEventArgs ev)
        {
            if (IsSerpent(ev.Player))
            {
                if (!Config.SerepentsHandModifiers.FriendlyFire)
                {
                    ev.IsAllowed = false;
                }

                if (Config.SerepentsHandModifiers.TeleportTo106)
                {
                    TeleportTo106(ev.Player);
                }

                shPocketPlayers.Remove(ev.Player);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnEscapingPocketDimension(EscapingPocketDimensionEventArgs)"/>
        internal static void OnPocketDimensionEscape(EscapingPocketDimensionEventArgs ev)
        {
            if (IsSerpent(ev.Player))
            {
                ev.IsAllowed = false;
                if (Config.SerepentsHandModifiers.TeleportTo106)
                {
                    TeleportTo106(ev.Player);
                }

                shPocketPlayers.Remove(ev.Player);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnHurting(HurtingEventArgs)"/>
        internal static void OnPlayerHurt(HurtingEventArgs ev)
        {
            List<Player> scp035s = GetScp035s();

            if (((IsSerpent(ev.Target) && (ev.Attacker.Team == Team.SCP || ev.DamageType == DamageTypes.Pocket)) ||
                (IsSerpent(ev.Attacker) && (ev.Target.Team == Team.SCP || (scp035s != null && scp035s.Contains(ev.Target)))) ||
                (IsSerpent(ev.Target) && IsSerpent(ev.Attacker) && ev.Target != ev.Attacker)) && !Config.SerepentsHandModifiers.FriendlyFire)
            {
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnEndingRound(EndingRoundEventArgs)"/>
        internal static void OnCheckRoundEnd(EndingRoundEventArgs ev)
        {
            bool mtfAlive = CountRoles(Team.MTF) > 0;
            bool ciAlive = CountRoles(Team.CHI) > 0;
            bool scpAlive = CountRoles(Team.SCP) + GetScp035s().Count > 0;
            bool dclassAlive = CountRoles(Team.CDP) > 0;
            bool scientistsAlive = CountRoles(Team.RSC) > 0;
            bool shAlive = GetSHPlayers().Count > 0;

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
                    }
                }
                else
                {
                    ev.LeadingTeam = LeadingTeam.Anomalies;
                    ev.IsAllowed = true;
                    ev.IsRoundEnded = true;
                }
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnShooting(ShootingEventArgs)"/>
        internal static void OnShooting(ShootingEventArgs ev)
        {
            Player target = Player.Get(ev.TargetNetId);

            if (target != null && target.Role == RoleType.Scp096 && IsSerpent(ev.Shooter))
            {
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Scp106.OnContaining(ContainingEventArgs)"/>
        internal static void OnContaining106(ContainingEventArgs ev)
        {
            if (IsSerpent(ev.Player) && !Config.SerepentsHandModifiers.FriendlyFire)
            {
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnActivatingGenerator(ActivatingGeneratorEventArgs)"/>
        internal static void OnActivatingGenerator(ActivatingGeneratorEventArgs ev)
        {
            if (IsSerpent(ev.Player) && !Config.SerepentsHandModifiers.FriendlyFire)
            {
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnEnteringFemurBreaker(EnteringFemurBreakerEventArgs)"/>
        internal static void OnFemurEnter(EnteringFemurBreakerEventArgs ev)
        {
            if (IsSerpent(ev.Player) && !Config.SerepentsHandModifiers.FriendlyFire)
            {
                ev.IsAllowed = false;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDestroying(DestroyingEventArgs)"/>
        internal static void OnDestroying(DestroyingEventArgs ev)
        {
            if (IsSerpent(ev.Player))
            {
                DestroySH(ev.Player);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDied(DiedEventArgs)"/>
        internal static void OnPlayerDeath(DiedEventArgs ev)
        {
            if (IsSerpent(ev.Target))
            {
                DestroySH(ev.Target);
                return;
            }

            if (ev.Target.Role == RoleType.Scp106 && !Config.SerepentsHandModifiers.FriendlyFire)
            {
                foreach (Player player in Player.List.Where(x => shPocketPlayers.Contains(x)))
                {
                    player.Hurt(50000f, DamageTypes.Contain, "WORLD", player.Id);
                }
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnChangingRole(ChangingRoleEventArgs)"/>
        internal static void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (IsSerpent(ev.Player) && ev.NewRole != RoleType.Tutorial)
            {
                DestroySH(ev.Player);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnSpawningRagdoll(SpawningRagdollEventArgs)"/>
        internal static void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            if (IsSerpent(ev.Owner))
            {
                ev.IsAllowed = false;

                Role role = CharacterClassManager._staticClasses.SafeGet(ev.RoleType);
                global::Ragdoll.Info info = new global::Ragdoll.Info
                {
                    ClassColor = role.classColor,
                    DeathCause = ev.HitInformations,
                    FullName = Config.SerepentsHandModifiers.RoleName,
                    Nick = ev.Owner.Nickname,
                    ownerHLAPI_id = ev.PlayerId.ToString(),
                    PlayerId = ev.PlayerId,
                };

                Ragdoll.Spawn(role, info, ev.Position, ev.Rotation, ev.Velocity, true);
            }
        }

        private static readonly Config Config = SerpentsHand.Instance.Config;
    }
}