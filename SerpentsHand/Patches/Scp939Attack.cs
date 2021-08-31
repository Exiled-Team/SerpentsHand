namespace SerpentsHand.Patches
{
    using Exiled.API.Features;
    using HarmonyLib;
    using Mirror;
    using PlayableScps;
    using UnityEngine;
    using static API;

    /// <summary>
    /// Disables Amnesia effect from Serpents Hand, when SCP-939 bites them.
    /// </summary>
    [HarmonyPatch(typeof(Scp939), nameof(Scp939.ServerAttack))]
    public class Scp939Attack
    {
        public static void Postfix(Scp939 __instance, NetworkConnection conn, PlayableScps.Messages.Scp939AttackMessage msg)
        {
            Player player = Player.Get(msg.Victim);

            if (IsSerpent(player) && !SerpentsHand.Instance.Config.SerepentsHandModifiers.FriendlyFire)
            {
                player.ReferenceHub.playerEffectsController.DisableEffect<CustomPlayerEffects.Amnesia>();
            }
        }
    }
}
