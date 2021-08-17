using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    class StageFailTrigger_ThingNotPresentInArea : StageFailTrigger
    {
        // this is an "or" list. For "and" use multiple triggers.
		public List<string> thingDefNames;
		public int maxDistance;

		public override bool Failed(LordJob_Ritual ritual, TargetInfo spot, TargetInfo focus)
		{
            foreach (IntVec3 item in CellRect.CenteredOn(spot.Cell, this.maxDistance))
            {
                foreach (string s in this.thingDefNames)
                {
                    if (item.GetFirstThing(spot.Map, DefDatabase<ThingDef>.GetNamed(s)) != null)
                    {
                        // thing with specified def was found 
                        //Log.Message("thing found");
                        // check for accessibility maybe?
                        return false;
                    }
                }
            }
            return true;
		}

		public override void ExposeData()
		{
			Scribe_Collections.Look(ref thingDefNames, "thingDefNames");
			Scribe_Values.Look(ref maxDistance, "maxDistance");
			base.ExposeData();
		}
	}
}
