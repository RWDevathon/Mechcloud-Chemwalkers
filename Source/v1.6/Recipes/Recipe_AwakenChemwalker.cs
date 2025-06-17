using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    // Gives a player-controlled Chemwalker a psylink, if they do not already have one.
    public class Recipe_AwakenChemwalker : RecipeWorker
    {
        public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
        {
            if (thing is Pawn pawn && pawn.Faction == Faction.OfPlayer && pawn.def.race.FleshType == ABF_FleshTypeDefOf.ABF_FleshType_Chemwalker_Base && !pawn.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
            {
                return true;
            }
            return false;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (pawn.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
            {
                Messages.Message("ABF_AlreadyAwakened".Translate(pawn), MessageTypeDefOf.RejectInput, false);
            }
            else
            {
                // Give them the Stun ability prior to the Psylink or it will select a random psycast instead.
                pawn.abilities.GainAbility(ABF_AbilityDefOf.Stun);

                // No need to send a message, adding the Psylink will create a letter for us.
                Hediff_Level hediff_Level = HediffMaker.MakeHediff(HediffDefOf.PsychicAmplifier, pawn, pawn.health.hediffSet.GetBrain()) as Hediff_Level;
                pawn.health.AddHediff(hediff_Level);
                hediff_Level.SetLevelTo(1);
            }
        }
    }
}
