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
    public class TPRitualAttachableOutcomeEffectWorker_Custom : RitualAttachableOutcomeEffectWorker
    {   
        public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, RitualOutcomePossibility outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
        {
            extraOutcomeDesc = this.def.letterInfoText;

            RitualAttachableOutcomeEffectDef_TP_Custom outcomeEffect = this.def.GetModExtension<RitualAttachableOutcomeEffectDef_TP_Custom>() ?? null;
           
            ApplyUtility.ApplyNode(outcomeEffect, totalPresence, jobRitual, outcome, ref letterLookTargets);
        }
    }
}
