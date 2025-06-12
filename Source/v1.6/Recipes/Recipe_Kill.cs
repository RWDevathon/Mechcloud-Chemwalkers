using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class Recipe_Kill : RecipeWorker
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (IsViolationOnPawn(pawn, part, Faction.OfPlayer))
            {
                ReportViolation(pawn, billDoer, pawn.HomeFaction, -100);
            }
            pawn.Kill(null);
            ThoughtUtility.GiveThoughtsForPawnExecuted(pawn, billDoer, PawnExecutionKind.GenericHumane);
        }
    }
}
