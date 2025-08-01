using Verse;
using HarmonyLib;
using VFEPirates;

namespace ArtificialBeings
{
    public class Building_WarcasketFoundry_Patch
    {
        // Mechclouds cannot be entombed into warcaskets.
        [HarmonyPatch(typeof(Building_WarcasketFoundry), nameof(Building_WarcasketFoundry.CannotUseNowReason))]
        public class Building_WarcasketFoundry_CannotUseNowReason_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ref string __result, Pawn selPawn)
            {
                if (__result == null && selPawn.def.race.FleshType == ABF_FleshTypeDefOf.ABF_FleshType_Chemwalker_Base)
                {
                    __result = "ABF_MechcloudsCannotUseThis".Translate();
                }
            }
        }
    }
}