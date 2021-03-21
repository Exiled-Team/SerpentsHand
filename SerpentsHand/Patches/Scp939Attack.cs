namespace SerpentsHand.Patches
{
    using Exiled.API.Features;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Disables Amnesia effect from Serpents Hand, when SCP-939 bites them.
    /// </summary>
    [HarmonyPatch(typeof(Scp939PlayerScript), nameof(Scp939PlayerScript.CallCmdShoot))]
    public class Scp939Attack
    {
        public static void Postfix(Scp939PlayerScript __instance, GameObject target)
        {
            Player player = Player.Get(target);
            if (EventHandlers.ShPlayers.Contains(player.Id) && !SerpentsHand.Instance.Config.SerepentsHandModifiers.FriendlyFire)
            {
                player.ReferenceHub.playerEffectsController.DisableEffect<CustomPlayerEffects.Amnesia>();
            }
        }
    }
}
