namespace SerpentsHand.Patches
{
    using Exiled.API.Features;
    using HarmonyLib;
    using Respawning;
    using System;

    [HarmonyPatch(typeof(RespawnTickets), nameof(RespawnTickets.DrawRandomTeam))]
    class SHSpawn
    {
        public static void Postfix(ref SpawnableTeamType __result)
        {
            try
            {
                if (__result == SpawnableTeamType.ChaosInsurgency)
                    EventHandlers.instance.CalculateChance();
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }
    }
}

