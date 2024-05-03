using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class Recipe_AttuneWildArmor : RecipeWorker
    {
        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            if (thing is Pawn pawn && pawn.Faction == null)
            {
                return true;
            }
            return false;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (!billDoer.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
            {
                Messages.Message("ABF_RequiresPsychicPowers".Translate(billDoer), MessageTypeDefOf.RejectInput, false);
            }
            else
            {
                pawn.SetFaction(Faction.OfPlayer, billDoer);
                Messages.Message("ABF_MechcloudAttuned".Translate(billDoer.LabelShortCap), MessageTypeDefOf.PositiveEvent);
            }
        }
    }
}
