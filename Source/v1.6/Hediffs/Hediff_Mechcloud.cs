using RimWorld;
using Verse;

namespace ArtificialBeings
{
    // Mechclouds have specific behaviors that need to happen only for them. Chemwalkers should ALWAYS have this hediff.
    public class Hediff_Mechcloud : Hediff
    {
        public override bool ShouldRemove => false;

        public override bool Visible => false;

        // Downed chemwalkers of non-player factions instantly die.
        public override void Notify_Downed()
        {
            base.Notify_Downed();
            if (pawn.Faction != Faction.OfPlayer)
            {
                pawn.Kill(null);
            }
        }

        // Because chemwalkers have no corpse, they can accidentally delete their equipment if they are killed before being downed.
        // This prevents that from happening, due to order of operations in the call chain for the kill method.
        public override void Notify_PawnKilled()
        {
            base.Notify_PawnKilled();
            pawn.DropAndForbidEverything();
        }

        // Chemwalkers should have all tendable injuries automatically tended upon taking damage. The tend quality should always be a flat 50%.
        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
            foreach (Hediff hediff in pawn.health.hediffSet.GetHediffsTendable())
            {
                if (hediff is Hediff_Injury hediffInjury && !hediff.IsTended())
                {
                    // Set it to 100% quality, with 50% max quality, to avoid randomness setting it lower. Set a batchPosition of 1 to make it tend silently.
                    hediffInjury.Tended(1f, 0.5f, 1);
                }
            }
        }
    }
}
