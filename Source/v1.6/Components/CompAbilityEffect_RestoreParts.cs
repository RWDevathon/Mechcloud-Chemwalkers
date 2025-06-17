using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    public class CompAbilityEffect_RestoreParts : CompAbilityEffect
    {
        public new CompAbilityEffectProperties_RestoreParts Props => (CompAbilityEffectProperties_RestoreParts)props;

        public Pawn Caster => parent.pawn;

        private float casterPsyfocusLimit;

        private float casterEntropyLimit;

        // Make absolutely sure this is only being used on chemwalkers. Heal as many hediffs as psyfocus/entropy allows, starting from the core part.
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (target.Pawn.def.race.FleshType == ABF_FleshTypeDefOf.ABF_FleshType_Chemwalker_Base)
            {
                casterPsyfocusLimit = Caster.psychicEntropy.CurrentPsyfocus;
                casterEntropyLimit = Caster.psychicEntropy.MaxEntropy - Caster.psychicEntropy.EntropyValue;
                float severityToHeal = Mathf.Min(casterPsyfocusLimit / Props.psyfocusCostPerSeverity, casterEntropyLimit / Props.entropyCostPerSeverity);
                float severityUsed = 0f;
                RestoreParts(target.Pawn, target.Pawn.def.race.body.corePart, ref severityToHeal, ref severityUsed);
                Caster.psychicEntropy.OffsetPsyfocusDirectly(-severityUsed * Props.psyfocusCostPerSeverity);
                Caster.psychicEntropy.TryAddEntropy(severityUsed * Props.entropyCostPerSeverity);
            }
        }

        // Recursively restore children parts of the originally restored part. IE. hands and fingers when an arm was restored.
        private void RestoreParts(Pawn pawn, BodyPartRecord part, ref float severityLeftToRestoreChildren, ref float severityUsed)
        {
            if (part == null || severityLeftToRestoreChildren <= 0)
                return;

            // Acquire a list of all hediffs on this specific part, and prepare a bool to check if this part has hediffs that can't be handled with the available points.
            List<Hediff> targetHediffs = pawn.health.hediffSet.hediffs.Where(hediff => hediff.Part == part && !hediff.def.keepOnBodyPartRestoration && hediff.def.isBad).ToList();

            // Restore severity up to the limit. If there is any hediff with a severity too high, then recursion stops at this node.
            foreach (Hediff hediff in targetHediffs)
            {
                float severity;
                // Missing parts use the max health of the part for severity, as the hediff severity does not match the "lost" health.
                if (hediff.def == HediffDefOf.MissingBodyPart)
                {
                    severity = hediff.Part.def.GetMaxHealth(pawn);
                }
                // Diseases and various other effects usually just set severity to 0.5. Increase it by tenfold to make it a bit more reasonable.
                else if (hediff.def.injuryProps == null)
                {
                    severity = hediff.Severity * 10f;
                }
                // Injuries have severity equal to hit points lost.
                else
                {
                    severity = hediff.Severity;
                }

                // If there is insufficient healing to remove the hediff, we are going to terminate here one way or another.
                if (severityLeftToRestoreChildren < severity)
                {
                    // If the targeted part is Missing, we have used no severity yet, and we can restore at least half of it, treat it as using all severity possible.
                    // This should reduce the possibility of being unable to restore the part ever.
                    if (hediff.def == HediffDefOf.MissingBodyPart && severityUsed == 0f && severityLeftToRestoreChildren >= severity / 2)
                    {
                        severityUsed = severityLeftToRestoreChildren;
                        severityLeftToRestoreChildren = 0f;
                        pawn.health.RemoveHediff(hediff);
                        return;
                    }
                    else if (hediff.def.injuryProps != null)
                    {
                        hediff.Severity -= severityLeftToRestoreChildren;
                        severityUsed += severityLeftToRestoreChildren;
                        severityLeftToRestoreChildren = 0;
                        return;
                    }
                }
                else
                {
                    severityUsed += severity;
                    severityLeftToRestoreChildren -= severity;
                    pawn.health.RemoveHediff(hediff);
                }
            }

            foreach (BodyPartRecord childPart in part.GetDirectChildParts())
            {
                RestoreParts(pawn, childPart, ref severityLeftToRestoreChildren, ref severityUsed);
            }
        }
    }
}
