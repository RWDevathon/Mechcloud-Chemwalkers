using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class JobDriver_AttuneToArmor : JobDriver
    {
        public ABF_ArmorPawnSpawner Spawner => job.GetTarget(TargetIndex.A).Thing as ABF_ArmorPawnSpawner;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            pawn.Reserve(Spawner, job);
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(600).WithProgressBarToilDelay(TargetIndex.A);
            Toil completeAttunement = ToilMaker.MakeToil("MakeNewToils");
            completeAttunement.initAction = delegate
            {
                Spawner.SpawnPawn(completeAttunement.actor.Faction);
                Messages.Message("ABF_MechcloudAttuned".Translate(completeAttunement.actor.LabelShortCap), MessageTypeDefOf.PositiveEvent);
            };
            completeAttunement.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return completeAttunement;
        }
    }
}
