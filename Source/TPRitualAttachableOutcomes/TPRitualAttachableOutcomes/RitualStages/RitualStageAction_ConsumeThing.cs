using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    class RitualStageAction_ConsumeThing : RitualStageAction
    {
        public string thingDefName = "";
        public int amountToConsume = 0;
        public int originalAmount = 0;
        public int lookDistance = 8;

        // the user has to supply a stageId string that end/fail triggers for the current stage will look for to look at the enoughConsumed
        public string stageId = "";

        // the real issue is going to be exposing this to the current stage's end/fail triggers
        public bool enoughConsumed = false;

        public bool consumptionComplete = false;

        public override void Apply(LordJob_Ritual ritual)
        {
            enoughConsumed = false;
            consumptionComplete = false;

            originalAmount = amountToConsume;
            int amountPresent = 0;
            List<Thing> thingsInArea = GenRadial.RadialDistinctThingsAround(ritual.selectedTarget.Cell, ritual.Map, lookDistance, true).ToList();
            List<Thing> thingsToConsume = new List<Thing>();
            foreach (Thing item in thingsInArea)
            {
                Log.Message("thing " + item.def.defName + " and we're looking for " + thingDefName);
                if(item.def.defName == thingDefName)
                {
                    amountPresent += item.stackCount;
                    thingsToConsume.Add(item);
                    if(amountPresent >= amountToConsume)
                    {
                        break;
                    }
                }
            }

            if(amountPresent >= amountToConsume)
            {
                foreach(Thing item in thingsToConsume)
                {
                    if(item.stackCount >= amountToConsume)
                    {
                        item.stackCount -= amountToConsume;
                        amountToConsume = 0;
                        if(item.stackCount == 0)
                        {
                            item.Destroy();
                        }
                    }
                    else
                    {
                        amountToConsume -= item.stackCount;
                        item.stackCount = 0;
                        item.Destroy();
                    }

                    if(amountToConsume == 0)
                    {
                        break;
                    }
                }

                if (amountToConsume == 0)
                {
                    enoughConsumed = true;
                    amountToConsume = originalAmount;
                }
            }

            // regardless of if enough was consumed
            consumptionComplete = true;

            // pass the notifications up?
            
        }

        public override void ApplyToPawn(LordJob_Ritual ritual, Pawn pawn)
        {
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref thingDefName, "thingDefName");
            Scribe_Values.Look(ref amountToConsume, "amountToConsume");
            Scribe_Values.Look(ref originalAmount, "originalAmount");
            Scribe_Values.Look(ref lookDistance, "lookDistance");
            Scribe_Values.Look(ref stageId, "stageId");
            Scribe_Values.Look(ref enoughConsumed, "enoughConsumed");
            Scribe_Values.Look(ref consumptionComplete, "consumptionComplete");

        }



    }
}
