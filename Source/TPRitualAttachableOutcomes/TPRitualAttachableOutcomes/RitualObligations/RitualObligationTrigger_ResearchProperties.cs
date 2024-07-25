using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TPRitualAttachableOutcomes
{
    class RitualObligationTrigger_ResearchProperties : RitualObligationTriggerProperties
    {
        public string researchDefName = "";
        public RitualObligationTrigger_ResearchProperties()
        {
            triggerClass = typeof(RitualObligationTrigger_Research);
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref researchDefName, "researchDefName");
        }
    }
}