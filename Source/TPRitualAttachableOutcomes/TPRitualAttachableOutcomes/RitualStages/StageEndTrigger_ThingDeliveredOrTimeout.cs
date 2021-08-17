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
                // getting all the things on every tick is going to be pretty expensive, so we only check if the flag has been set by the duty handling deliveries
                if (checkThings)
                {
                    List<Thing> thingsNearby = GenRadial.RadialDistinctThingsAround(spot.Cell, spot.Map, lookDistance, true).ToList();
                    int checkAmount = 0;
                    foreach(Thing t in thingsNearby)
                    {
                        // if this proves to be laggy we might re-implement the RadialDistinctThingsAround method and check while getting the things
                        if(t.def.defName == thingDefName)
                        {
                            checkAmount += t.stackCount;
                        }

                        if(checkAmount >= amount)
                        {
                            return true;
                        }
                    }
                    // at the end we flip the flag again because we just did a check
                    checkThings = false;
                }
                return false;
            });
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref thingDefName, "thingDefName");   
            Scribe_Values.Look(ref amount, "amount");
            Scribe_Values.Look(ref lookDistance, "lookDistance");
            Scribe_Values.Look(ref ticksRemaining, "ticksRemaining");
            base.ExposeData();
        }
    }
}
