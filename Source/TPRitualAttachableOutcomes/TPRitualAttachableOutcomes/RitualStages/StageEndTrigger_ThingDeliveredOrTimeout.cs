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
        private int originalTicks = 600;

        // now the only real issue is going to be accessing this - we may need to make a new RitualStage class that holds this  
        public bool checkThings = true;

        // sadly I think we're going to need another stageId so the job can figure out which trigger needs to look for the amount being delivered
        // but this is still better than continuously checking the amount
        public string stageId = "";

        public override Trigger MakeTrigger(LordJob_Ritual ritual, TargetInfo spot, IEnumerable<TargetInfo> foci, RitualStage stage)
        {
            checkThings = true;
            ticksRemaining = originalTicks;
            return new Trigger_TickCondition(delegate {
                // check the things but only once
                if (checkThings)
                {
                    int amountPresent = 0;
                    List<Thing> thingsInArea = GenRadial.RadialDistinctThingsAround(ritual.selectedTarget.Cell, ritual.Map, lookDistance, true).ToList();
                    List<Thing> thingsToConsume = new List<Thing>();
                    foreach (Thing item in thingsInArea)
                    {
                        //Log.Message("thing " + item.def.defName + " and we're looking for " + thingDefName);
                        if (item.def.defName == thingDefName)
                        {
                            amountPresent += item.stackCount;
                        }
                    }
                    curAmount = amountPresent;
                    checkThings = false;
                }

                // end if all the things have been delivered
                if (curAmount >= amount)
                {
                    //Log.Message("enough things delivered");
                    ticksRemaining = originalTicks;
                    return true;
                }

                ticksRemaining--;
                
                // end if they run out of time
                // this may cause issues in later parts of the ritual if it's expecting these things, in most cases a failtrigger should be used
                if(ticksRemaining <= 0)
                {
                    //Log.Message("ritual has timed out");
                    ticksRemaining = originalTicks;
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
            Scribe_Values.Look(ref originalTicks, "originalTicks");
            base.ExposeData();
        }
    }
}
