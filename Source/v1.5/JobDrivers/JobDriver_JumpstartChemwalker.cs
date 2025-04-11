using System;
using System.Collections.Generic;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class JobDriver_JumpstartChemwalker : JobDriver
    {
        private ABF_ArmorPawnSpawner Armor => (ABF_ArmorPawnSpawner)job.GetTarget(TargetIndex.A).Thing;

        private Thing Item => job.GetTarget(TargetIndex.B).Thing;

        private Mote warmupMote;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (pawn.Reserve(Armor, job, 1, -1, null, errorOnFailed))
            {
                return pawn.Reserve(Item, job, 1, -1, null, errorOnFailed);
            }
            return false;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.B).FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
            Toil toil = Toils_General.Wait(600);
            toil.WithProgressBarToilDelay(TargetIndex.A);
            toil.FailOnDespawnedOrNull(TargetIndex.A);
            toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            toil.tickAction = delegate
            {
                CompUsable compUsable = Item.TryGetComp<CompUsable>();
                if (compUsable != null && warmupMote == null && compUsable.Props.warmupMote != null)
                {
                    warmupMote = MoteMaker.MakeAttachedOverlay(Armor, compUsable.Props.warmupMote, Vector3.zero);
                }

                warmupMote?.Maintain();
            };
            yield return toil;
            yield return Toils_General.Do(new Action(Jumpstart));
        }

        // Reduce the target shell's work remaining by the designated amount of ticks from the target effect properties.
        private void Jumpstart()
        {
            int jumpstartTicks = Item.def.GetCompProperties<CompTargetEffectProperties_JumpstartChemwalker>().jumpstartTicks;
            Armor.workRemaining = Math.Max(0, Armor.workRemaining - jumpstartTicks);
            Item.SplitOff(1).Destroy(DestroyMode.Vanish);
        }
    }
}
