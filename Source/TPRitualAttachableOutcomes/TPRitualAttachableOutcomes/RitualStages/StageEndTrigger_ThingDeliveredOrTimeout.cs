using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace TPRitualAttachableOutcomes
{
    class StageEndTrigger_ThingDeliveredOrTimeout : StageEndTrigger
    {
        // wait for Things to be delivered
        // do we need a RitualStage to deliver Things? 
        // I think pawn delivery is handled with duties

        // does this need the Thing that should be delivered in addition to the duty containing it? Is there an easy way we can link them?

        public string thingDefName = "";
        public int amount = 1;
        public int curAmount = 0;
        public int lookDistance = 8;
        public int ticksRemaining = 600;

        // now the only real issue is going to be accessing this - we may need to make a new RitualStage class that holds this  
        public bool checkThings = false;

        public override Trigger MakeTrigger(LordJob_Ritual ritual, TargetInfo spot, IEnumerable<TargetInfo> foci, RitualStage stage)
        {
            return new Trigger_TickCondition(delegate {
                ticksRemaining--;
                
                // end if they run out of time
                // this may cause issues in later parts of the ritual if it's expecting these things, in most cases a failtrigger should be used
                if(ticksRemaining <= 0)
                {
                    return true;
                }

                // end if all the things have been delivered
                if(curAmount >= amount)
                {
                    return true;
                }

                return false;
            });
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref thingDefName, "thingDefName");   
            Scribe_Values.Look(ref amount, "amount");
            Scribe_Values.Look(ref curAmount, "curAmount");
            Scribe_Values.Look(ref lookDistance, "lookDistance");
            Scribe_Values.Look(ref ticksRemaining, "ticksRemaining");
            base.ExposeData();
        }
    }
}
