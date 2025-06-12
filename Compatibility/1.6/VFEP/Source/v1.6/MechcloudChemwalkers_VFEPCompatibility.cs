using HarmonyLib;
using System.Reflection;
using Verse;

namespace ArtificialBeings
{
    [StaticConstructorOnStartup]
    public static class MechcloudChemwalkers_VFEPCompatibility_PostInit
    {
        static MechcloudChemwalkers_VFEPCompatibility_PostInit()
        {
            new Harmony("MechcloudChemwalkers_VFEPCompatibility").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}