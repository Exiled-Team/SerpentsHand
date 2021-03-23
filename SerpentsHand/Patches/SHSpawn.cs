namespace SerpentsHand.Patches
{
    using System;
    using Exiled.API.Features;
    using HarmonyLib;
    using Respawning;

    /// <summary>
    /// Handles calling <see cref="EventHandlers.CalculateChance"/> when <see cref="SpawnableTeamType"/> is choosed.
    /// </summary>
    [HarmonyPatch(typeof(RespawnTickets), nameof(RespawnTickets.DrawRandomTeam))]
    internal class SHSpawn
    {
        /// <inheritdoc/>
        public static void Postfix(ref SpawnableTeamType __result)
        {
            try
            {
                if (__result == SpawnableTeamType.ChaosInsurgency)
                    EventHandlers.Instance.CalculateChance();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}