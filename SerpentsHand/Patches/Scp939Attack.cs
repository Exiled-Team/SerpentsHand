namespace SerpentsHand.Patches
{
	using Exiled.API.Features;
	using HarmonyLib;
	using UnityEngine;

	[HarmonyPatch(typeof(Scp939PlayerScript), nameof(Scp939PlayerScript.CallCmdShoot))]
	public class Scp939Attack
	{
		public static void Postfix(Scp939PlayerScript __instance, GameObject target)
		{
			Player player = Player.Get(target);
			if (EventHandlers.shPlayers.Contains(player.Id) && !SerpentsHand.instance.Config.SerepentsHandModifiers.FriendlyFire)
			{
				player.ReferenceHub.playerEffectsController.DisableEffect<CustomPlayerEffects.Amnesia>();
			}
		}
	}
}
