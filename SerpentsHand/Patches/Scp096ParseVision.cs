using Exiled.API.Features;
using HarmonyLib;
using PlayableScps;
using RemoteAdmin;
using UnityEngine;

namespace SerpentsHand.Patches
{
	[HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.ParseVisionInformation))]
	class Scp096ParseVision
	{
		public static bool Prefix(PlayableScps.Scp096 __instance, VisionInformation info)
		{
			PlayableScpsController playableScpsController = info.RaycastResult.transform.gameObject.GetComponent<PlayableScpsController>();
			if (__instance == null || !info.Looking || !info.RaycastHit || playableScpsController == null || playableScpsController.CurrentScp == null || playableScpsController.CurrentScp != __instance)
			{
				return false;
			}
			if (!SerpentsHand.instance.Config.CanTrigger096)
			{
				Player player = Player.Get(info.Source);
				if (player != null && EventHandlers.shPlayers.Contains(player.Id))
				{
					return false;
				}
			}
			float delay = (1f - info.DotProduct) / 0.25f * (Vector3.Distance(info.Source.transform.position, info.Target.transform.position) * 0.1f);
			if (!__instance.Calming)
			{
				__instance.AddTarget(info.Source);
			}
			if (__instance.CanEnrage && info.Source != null)
			{
				__instance.PreWindup(delay);
			}

			return false;
		}
	}
}
