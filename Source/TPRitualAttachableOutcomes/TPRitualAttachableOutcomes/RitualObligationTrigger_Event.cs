using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    class RitualObligationTrigger_Event : RitualObligationTrigger
    {

		public string eventDefName = "";
		public int removeAfterTicks = -1;

		private RitualObligation ro = null;

		public override void Init(RitualObligationTriggerProperties props)
		{
			base.Init(props);
			eventDefName = ((RitualObligationTrigger_EventProperties)props).eventDefName;
			removeAfterTicks = ((RitualObligationTrigger_EventProperties)props).removeAfterTicks;
		}

        public override void Tick()
        {
			base.Tick();
			if(removeAfterTicks > 0)
            {
				removeAfterTicks--;
            }
            else if(ro != null)
            {
				ritual.activeObligations.Remove(ro);
            }
        }
        public void Notify_Event(IncidentDef incident)
        {
			//Log.Message("this event defName " + this.eventDefName + ", incident defName " + incident.defName);
			// create an obligation if the event matches, but only if the ritual doesn't have any current obligations
			if (this.eventDefName == incident.defName && (ritual.activeObligations == null || ritual.activeObligations.Count == 0) )
			{
				//Log.Message("adding obligation");
				ro = new RitualObligation(ritual);
				ritual.AddObligation(ro); //maybe something about only triggering it on a map that the ideo has pawns on. And only player pawns ofc
			}
		}
		public override void ExposeData()
		{
			Scribe_Values.Look(ref eventDefName, "eventDefName");
			Scribe_Values.Look(ref removeAfterTicks, "removeAfterTicks");
			base.ExposeData();
		}


	}
}
