using HarmonyLib;
using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class ABF_Pawn_HealthTracker_Patch
    {
        // Chemwalkers not part of your faction are destroyed outright when they are downed.
        [HarmonyPatch(typeof(Pawn_HealthTracker), "MakeDowned")]
        public static class MakeDowned_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn_HealthTracker __instance, DamageInfo? dinfo, Hediff hediff, ref Pawn ___pawn)
            {
                if (___pawn.def.race.DeathActionWorker is DeathActionWorker_Mechcloud && ___pawn.Faction != Faction.OfPlayer)
                {
                    ___pawn.Kill(dinfo, hediff);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
