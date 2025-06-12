using HarmonyLib;
using System.Reflection;
using Verse;

namespace ArtificialBeings
{
    public class MechcloudChemwalkers : Mod
    {
        public MechcloudChemwalkers(ModContentPack content) : base(content)
        {
            new Harmony("MechcloudChemwalkers").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}