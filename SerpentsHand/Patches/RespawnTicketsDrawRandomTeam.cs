using HarmonyLib;
using Respawning;

namespace SerpentsHand.Patches
{
    [HarmonyPatch(typeof(RespawnTickets), nameof(RespawnTickets.DrawRandomTeam))]
    internal class RespawnTicketsDrawRandomTeam
    {
        public static void Postfix(ref SpawnableTeamType __result)
        {
            if (__result == SpawnableTeamType.ChaosInsurgency)
                Extensions.CalculateChance();
        }
    }
}
