using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace TPRitualAttachableOutcomes
{
    class StageEndTrigger_ThingNotPresent : StageEndTrigger
    {
        // this is an "or" list. For "and" use multiple triggers
        public List<string> thingDefNames;
        public int maxDistance;
        public override Trigger MakeTrigger(LordJob_Ritual ritual, TargetInfo spot, IEnumerable<TargetInfo> foci, RitualStage stage)
        {
            return new Trigger_Custom(delegate
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
                //Log.Message("thing not found");
                return true;
            });
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref thingDefNames, "thingDefNames");
            Scribe_Values.Look(ref maxDistance, "maxDistance");
            base.ExposeData();
        }
    }
}
