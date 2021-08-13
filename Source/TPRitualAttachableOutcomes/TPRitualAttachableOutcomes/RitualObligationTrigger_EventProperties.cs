using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace TPRitualAttachableOutcomes
{
    class RitualObligationTrigger_EventProperties : RitualObligationTriggerProperties
    {
        public string eventDefName = "";
        public int removeAfterTicks = -1;
        public RitualObligationTrigger_EventProperties()
        {
            triggerClass = typeof(RitualObligationTrigger_Event);
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref eventDefName, "eventDefName");
            Scribe_Values.Look(ref removeAfterTicks, "removeAfterTicks");
        }
    }
}
