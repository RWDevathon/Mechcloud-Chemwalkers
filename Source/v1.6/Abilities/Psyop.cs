using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse.Sound;
using Verse;
using Verse.AI;
using ArtificialBeings.Abilities;

namespace ArtificialBeings
{
    public class Psyop : Ability
    {
        private Mote moteCast;

        private static float MoteCastFadeTime = 0.4f;

        private static float MoteCastScale = 1f;

        private static Vector3 MoteCastOffset = new Vector3(0f, 0f, 0.48f);

        public override AcceptanceReport CanCast
        {
            get
            {
                if (!base.CanCast)
                {
                    return false;
                }
                if (pawn.psychicEntropy.EntropyValue < 0f)
                {
                    return false;
                }
                if (def.EntropyGain > 0f)
                {
                    return !pawn.psychicEntropy.WouldOverflowEntropy(def.EntropyGain);
                }
                if (def.PsyfocusCost > pawn.psychicEntropy.CurrentPsyfocus + 0.0005f)
                {
                    return false;
                }
                return true;
            }
        }

        // Override to prevent queuing after any other ability, psycast or otherwise.
        public override bool CanQueueCast
        {
            get
            {
                if (!CanCast)
                {
                    return false;
                }
                if (pawn.jobs == null)
                {
                    return false;
                }
                foreach (Job item in pawn.jobs.AllJobs())
                {
                    if (item.ability != null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public override bool CanApplyOn(GlobalTargetInfo target)
        {
            return base.CanApplyOn(target) && target.Pawn is Pawn pawn && pawn.def.race.FleshType == ABF_FleshTypeDefOf.ABF_FleshType_Chemwalker_Base;
        }

        public override bool CanApplyOn(LocalTargetInfo target)
        {
            return base.CanApplyOn(target) && target.Pawn is Pawn pawn && pawn.def.race.FleshType == ABF_FleshTypeDefOf.ABF_FleshType_Chemwalker_Base;
        }

        public Psyop(Pawn pawn)
            : base(pawn)
        {
        }

        public Psyop(Pawn pawn, AbilityDef def)
            : base(pawn, def)
        {
        }

        public override IEnumerable<Command> GetGizmos()
        {
            if (ModLister.RoyaltyInstalled)
            {
                if (gizmo == null)
                {
                    gizmo = new Command_Psyop(this, pawn);
                }
                yield return gizmo;
            }
        }

        public override bool Activate(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (!ModLister.CheckRoyalty("Psycast"))
            {
                return false;
            }
            if (def.EntropyGain > 0f && !pawn.psychicEntropy.TryAddEntropy(def.EntropyGain))
            {
                return false;
            }
            float num = FinalPsyfocusCost(target);
            if (num > float.Epsilon)
            {
                pawn.psychicEntropy.OffsetPsyfocusDirectly(0f - num);
            }
            if (def.showPsycastEffects)
            {
                if (EffectComps.Any((CompAbilityEffect c) => c.Props.psychic))
                {
                    if (def.HasAreaOfEffect)
                    {
                        FleckMaker.Static(target.Cell, pawn.Map, FleckDefOf.PsycastAreaEffect, def.EffectRadius);
                        SoundDefOf.PsycastPsychicPulse.PlayOneShot(new TargetInfo(target.Cell, pawn.Map));
                    }
                    else
                    {
                        SoundDefOf.PsycastPsychicEffect.PlayOneShot(new TargetInfo(target.Cell, pawn.Map));
                    }
                }
                else if (def.HasAreaOfEffect && def.canUseAoeToGetTargets)
                {
                    SoundDefOf.Psycast_Skip_Pulse.PlayOneShot(new TargetInfo(target.Cell, pawn.Map));
                }
            }
            return base.Activate(target, dest);
        }

        public override bool Activate(GlobalTargetInfo target)
        {
            if (def.EntropyGain > 0f && !pawn.psychicEntropy.TryAddEntropy(def.EntropyGain))
            {
                return false;
            }
            float psyfocusCost = def.PsyfocusCost;
            if (psyfocusCost > 0f)
            {
                pawn.psychicEntropy.OffsetPsyfocusDirectly(0f - psyfocusCost);
            }
            return base.Activate(target);
        }

        // Psyops only apply to chemwalkers
        protected override void ApplyEffects(IEnumerable<CompAbilityEffect> effects, LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.Pawn.def.race.FleshType == ABF_FleshTypeDefOf.ABF_FleshType_Chemwalker_Base)
            {
                foreach (CompAbilityEffect effect in effects)
                {
                    effect.Apply(target, dest);
                }
                return;
            }
            MoteMaker.ThrowText(target.CenterVector3, target.Pawn.Map, "TextMote_Immune".Translate());
        }

        public override bool GizmoDisabled(out string reason)
        {
            if (pawn.psychicEntropy.EntropyValue < 0 || pawn.psychicEntropy.CurrentPsyfocus < 0)
            {
                reason = "ABF_RequiresAwakening".Translate(def.label, pawn);
                return true;
            }
            if (def.PsyfocusCost > pawn.psychicEntropy.CurrentPsyfocus + 0.0005f)
            {
                reason = "CommandPsycastNotEnoughPsyfocus".Translate(def.PsyfocusCostPercent, pawn.psychicEntropy.CurrentPsyfocus.ToStringPercent("0.#"), def.label.Named("PSYCASTNAME"), pawn.Named("CASTERNAME"));
                return true;
            }
            if (def.EntropyGain > 0f && pawn.psychicEntropy.WouldOverflowEntropy(def.EntropyGain))
            {
                reason = "CommandPsycastWouldExceedEntropy".Translate(def.label);
                return true;
            }
            return base.GizmoDisabled(out reason);
        }

        public override void AbilityTick()
        {
            base.AbilityTick();
            if (pawn.Spawned && Casting)
            {
                if (moteCast == null || moteCast.Destroyed)
                {
                    moteCast = MoteMaker.MakeAttachedOverlay(pawn, ThingDefOf.Mote_CastPsycast, MoteCastOffset, MoteCastScale, verb.verbProps.warmupTime - MoteCastFadeTime);
                }
                else
                {
                    moteCast.Maintain();
                }
            }
        }
    }
}
