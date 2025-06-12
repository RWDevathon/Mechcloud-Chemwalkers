using Verse;
using HarmonyLib;
using VFEPirates;

namespace ArtificialBeings
{
    public class Building_WarcasketFoundry_Patch
    {
        // Mechclouds cannot be entombed into warcaskets. Use the DeathActionWorker as there exists no other identification method at the moment.
        [HarmonyPatch(typeof(Building_WarcasketFoundry), nameof(Building_WarcasketFoundry.CannotUseNowReason))]
        public class Building_WarcasketFoundry_CannotUseNowReason_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ref string __result, Pawn selPawn)
            {
                if (__result == null && selPawn.RaceProps.DeathActionWorker is DeathActionWorker_Mechcloud)
                {
                    __result = "ABF_MechcloudsCannotUseThis".Translate();
                }
            }
        }
    }
}