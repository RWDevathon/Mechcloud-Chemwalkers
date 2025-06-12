using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class CompTargetable_SingleChemwalkerShell: CompTargetable
    {
        protected override bool PlayerChoosesTarget => true;

        protected override TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetPawns = false,
                canTargetBuildings = true,
            };
        }

        public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
        {
            yield return targetChosenByPlayer;
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (target.Thing is ABF_ArmorPawnSpawner)
            {
                return base.ValidateTarget(target.Thing, showMessages);
            }
            return false;
        }
    }
}
