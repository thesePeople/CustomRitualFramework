using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    [StaticConstructorOnStartup]
    class RitualOutcomeEffectWorker_FromQuality_TP_Custom : RitualOutcomeEffectWorker_FromQuality
    {
        public RitualOutcomeEffectWorker_FromQuality_TP_Custom()
        {
        }

        public RitualOutcomeEffectWorker_FromQuality_TP_Custom(RitualOutcomeEffectDef def)
            : base(def)
        {
        }   
        protected override void ApplyExtraOutcome(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
        {
           // Log.Message("checking extra outcome for your speech thingy I guess");
            
            extraOutcomeDesc = null;
            RitualAttachableOutcomeEffectDef_TP_Custom outcomeEffect = this.def.GetModExtension<RitualAttachableOutcomeEffectDef_TP_Custom>() ?? null;
            
            if (outcomeEffect != null)
            {
               // Log.Message("outcome positivityIndex: " + outcome.positivityIndex);
                ApplyUtility.ApplyNode(outcomeEffect, totalPresence, jobRitual, outcome, ref letterLookTargets);
            }
        }   
    }
}
