using RimWorld;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    public class Verb_CastPsyop : Verb_CastAbility
    {
        public Psyop Psyop => ability as Psyop;

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (!base.ValidateTarget(target, showMessages))
            {
                return false;
            }
            if (!(target.Pawn is Pawn pawn && pawn.def.race.DeathActionWorker is DeathActionWorker_Mechcloud))
            {
                return false;
            }
            if (Psyop.def.EntropyGain > float.Epsilon && CasterPawn.psychicEntropy.WouldOverflowEntropy(Psyop.def.EntropyGain))
            {
                if (showMessages)
                {
                    Messages.Message("CommandPsycastWouldExceedEntropy".Translate(), caster, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            float psyfocusCost = Psyop.FinalPsyfocusCost(target);
            if (psyfocusCost > float.Epsilon && psyfocusCost > CasterPawn.psychicEntropy.CurrentPsyfocus + 0.0005f)
            {
                if (showMessages)
                {
                    Messages.Message("CommandPsycastNotEnoughPsyfocus".Translate(psyfocusCost.ToStringPercent("0.#"), CasterPawn.psychicEntropy.CurrentPsyfocus.ToStringPercent("0.#"), Psyop.def.label.Named("PSYCASTNAME"), caster.Named("CASTERNAME")), caster, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            return true;
        }

        public override void OnGUI(LocalTargetInfo target)
        {
            Texture2D texture2D = UIIcon;
            if (!Psyop.CanApplyOn(target))
            {
                texture2D = TexCommand.CannotShoot;
            }
            GenUI.DrawMouseAttachment(texture2D);
            DrawAttachmentExtraLabel(target);
        }
    }
}
