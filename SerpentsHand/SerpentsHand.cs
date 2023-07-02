using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;

using PlayerEvent = Exiled.Events.Handlers.Player;

namespace SerpentsHand
{
    [CustomRole(RoleTypeId.Tutorial)]
    public class SerpentsHand : CustomRole
    {
        public override uint Id { get; set; } = 1;
        public override RoleTypeId Role { get; set; } = RoleTypeId.Tutorial;
        public override int MaxHealth { get; set; } = 120;
        public override string Name { get; set; } = "Serpents Hand";
        public override string Description { get; set; } = "Help the SCPs by killing all other classes";
        public override string CustomInfo { get; set; } = "Serpents Hand";
        public override float SpawnChance { get; set; } = 75f;

        [Description("The maximum size of a Serpents Hand squad.")]
        public int MaxSquad { get; set; } = 8;

        [Description("How many respawn waves must occur before considering Serpents Hand to spawn.")]
        public int RespawnDelay { get; set; } = 1;

        [Description("The maximum number of times Serpents can spawn per game.")]
        public int MaxSpawns { get; set; } = 1;

        [Description("Should Tutorial automaticly be converted to Serpends Hand?")]
        public bool AutoConvertTutorial { get; set; } = false;

        [Description("Determines if Serpents Hand should be able to spawn when there is no SCPs.")]
        public bool CanSpawnWithoutScps { get; set; } = false;

        [Description("Set this to false if Chaos and SCPs CANNOT win together on your server")]
        public bool ScpsWinWithChaos { get; set; } = true;

        [Description("The message annouced by CASSIE when Serpents hand spawn. (Empty = Disabled)")]
        public string EntryAnnoucement { get; set; } = "SERPENTS HAND HASENTERED";

        [Description("Should the Cassie Message use subtitles")]
        public bool Subtitles { get; set;} = false;

        [Description("The broadcast shown to SCPs when the Serpents Hand respawns.")]
        public Exiled.API.Features.Broadcast EntryBroadcast { get; set; } = new Exiled.API.Features.Broadcast("<color=orange>Serpents Hand has entered the facility!</color>");


        public override List<string> Inventory { get; set; } = new()
        {
            $"{ItemType.GunCrossvec}",
            $"{ItemType.KeycardChaosInsurgency}",
            $"{ItemType.GrenadeFlash}",
            $"{ItemType.Radio}",
            $"{ItemType.Medkit}",
            $"{ItemType.ArmorCombat}"
        };

        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            { AmmoType.Nato9, 120 }
        };

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            StaticSpawnPoints = new List<StaticSpawnPoint>
            {
                new()
                {
                    Name = "Spawn Point",
                    Position = new UnityEngine.Vector3(63f, 992f, -50f),
                    Chance = 100
                }
            }
        };

        protected override void SubscribeEvents()
        {
            PlayerEvent.EnteringPocketDimension += OnEnteringPocketDimension;
            PlayerEvent.Hurting += OnHurting;
            PlayerEvent.Shooting += OnShooting;
            PlayerEvent.ActivatingGenerator += OnActivatingGenerator;
            PlayerEvent.ChangingRole += OnChangingRole;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            PlayerEvent.EnteringPocketDimension -= OnEnteringPocketDimension;
            PlayerEvent.Hurting -= OnHurting;
            PlayerEvent.Shooting -= OnShooting;
            PlayerEvent.ActivatingGenerator -= OnActivatingGenerator;
            PlayerEvent.ChangingRole -= OnChangingRole;

            base.UnsubscribeEvents();
        }

        private void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev)
        {
            if (Check(ev.Player))
                ev.IsAllowed = false;
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if ((Check(ev.Player) && ev.Attacker.Role.Team == Team.SCPs) ||
                (ev.Attacker != null && Check(ev.Attacker) && ev.Player.Role.Team == Team.SCPs) ||
                (ev.Attacker != null && Check(ev.Attacker) && Check(ev.Player) && ev.Player != ev.Attacker))
                ev.IsAllowed = false;
        }

        private void OnShooting(ShootingEventArgs ev)
        {
            Player? target = Player.Get(ev.TargetNetId);
            if (target != null && target.Role == RoleTypeId.Scp096 && Check(ev.Player))
                ev.IsAllowed = false;
        }

        private void OnActivatingGenerator(ActivatingGeneratorEventArgs ev)
        {
            if (Check(ev.Player))
                ev.IsAllowed = false;
        }

        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (AutoConvertTutorial && ev.NewRole == Role && !ev.Player.IsOverwatchEnabled && !Check(ev.Player))
                AddRole(ev.Player);
        }
    }
}
