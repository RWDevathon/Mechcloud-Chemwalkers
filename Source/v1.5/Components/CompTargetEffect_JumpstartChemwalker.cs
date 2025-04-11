using RimWorld;
using Verse.AI;
using Verse;

namespace ArtificialBeings
{
    // Target controller for jumpstarting a chemwalker.
    public class CompTargetEffect_JumpstartChemwalker : CompTargetEffect
    {
        public CompTargetEffectProperties_JumpstartChemwalker Props
        {
            get
            {
                return (CompTargetEffectProperties_JumpstartChemwalker)props;
            }
        }

        public override bool CanApplyOn(Thing target)
        {
            return target is ABF_ArmorPawnSpawner spawner && spawner.Active && spawner.workRemaining > 0;
        }

        public override void DoEffectOn(Pawn user, Thing target)
        {
            if (!user.CanReserveAndReach(target, PathEndMode.Touch, Danger.Deadly))
            {
                Messages.Message("CannotReach".Translate(), MessageTypeDefOf.RejectInput, false);
            }

            if (target is ABF_ArmorPawnSpawner spawner && spawner.workRemaining > 0)
            {
                Job job = JobMaker.MakeJob(ABF_JobDefOf.ABF_Job_Chemwalker_JumpstartChemwalker, target, parent);
                job.count = 1;
                user.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }
        }
    }
}
