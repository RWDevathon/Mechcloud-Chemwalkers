using Verse;

namespace ArtificialBeings
{
    public class CompTargetEffectProperties_JumpstartChemwalker : CompProperties
    {
        public CompTargetEffectProperties_JumpstartChemwalker()
        {
            compClass = typeof(CompTargetEffect_JumpstartChemwalker);
        }

        public int jumpstartTicks = 0;
    }
}