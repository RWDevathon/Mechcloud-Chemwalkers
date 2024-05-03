using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class JobDriver_PackUpArmor : JobDriver
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
            yield return Toils_General.Wait(60).WithProgressBarToilDelay(TargetIndex.A);
            Toil packUp = ToilMaker.MakeToil("MakeNewToils");
            packUp.initAction = delegate
            {
                Spawner.ResetWorkRemaining();
                Spawner.Uninstall();
                pawn.records.Increment(RecordDefOf.ThingsUninstalled);
            };
            packUp.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return packUp;
        }
    }
}
