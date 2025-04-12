using RimWorld;
using Verse;

namespace ArtificialBeings.Abilities
{
    public class Command_Psyop : Command_Ability
    {
        public override string TopRightLabel
        {
            get
            {
                AbilityDef def = ability.def;
                string label = "";
                if (def.EntropyGain > 0f)
                {
                    label += "NeuralHeatLetter".Translate() + ": " + def.EntropyGain.ToString() + "\n";
                }
                if (def.PsyfocusCost > 0f)
                {
                    label += "PsyfocusLetter".Translate() + ": " + def.PsyfocusCost.ToStringPercent();
                }
                return label.TrimEndNewlines();
            }
        }

        public Command_Psyop(Psyop ability, Pawn pawn)
            : base(ability, pawn)
        {
        }

        protected override void DisabledCheck()
        {
            AbilityDef def = ability.def;
            disabled = false;
            if (def.EntropyGain > 0f && ability.pawn.psychicEntropy.WouldOverflowEntropy(def.EntropyGain))
            {
                DisableWithReason("CommandPsycastWouldExceedEntropy".Translate(def.label));
            }
            base.DisabledCheck();
        }
    }
}
