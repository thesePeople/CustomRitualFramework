using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TPRitualAttachableOutcomes
{
    class RitualObligationTrigger_Research : RitualObligationTrigger
    {

        public string researchDefName = "";

        private RitualObligation ro = null;

        public override void Init(RitualObligationTriggerProperties props)
        {
            base.Init(props);
            researchDefName = ((RitualObligationTrigger_ResearchProperties)props).researchDefName;
        }
        public void Notify_Research(ResearchProjectDef research)
        {
            if (this.researchDefName == research.defName && (ritual.activeObligations == null || ritual.activeObligations.Count == 0))
            {
                ro = new RitualObligation(ritual);
                ritual.AddObligation(ro);
            }
        }
        public override void Notify_GameStarted()
        {
            ResearchProjectDef research = DefDatabase<ResearchProjectDef>.GetNamed(researchDefName);
            if (research.IsFinished)
            {
                Notify_Research(research);
            }
        }
        public override void ExposeData()
        {
            Scribe_Values.Look(ref researchDefName, "researchDefName");
            base.ExposeData();
        }
    }
}
