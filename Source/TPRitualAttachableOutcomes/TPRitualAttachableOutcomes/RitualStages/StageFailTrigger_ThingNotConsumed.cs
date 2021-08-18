using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    class StageFailTrigger_ThingNotConsumed : StageFailTrigger
    {
        // StageFailTrigger_ThingNotConsumed must be placed in a stage that has a preAction with RitualStageAction_ConsumeThings
        // additionally, the stageId between the two must match
        // (you can have two consume actions if you have two of these fail triggers and separate stageId names, but it's not recommended)
        
        // you cannot reuse stageIds in the same ritual, even amount different stages. This is how it actually finds the right stage and action, since actions don't hold their parent stage
        public string stageId = "";

        public RitualStageAction_ConsumeThing targetAction = null;

        public override bool Failed(LordJob_Ritual ritual, TargetInfo spot, TargetInfo focus)
        {
            // get the matching ritualStageAction
            foreach(RitualStage rs in ritual.Ritual.behavior.def.stages)
            {
                List<RitualStageAction> ritualStageActions = new List<RitualStageAction>();
                // it seems unlikely they'd want to use any but the preAction here but whatever
                ritualStageActions.Add(rs.preAction);
                ritualStageActions.Add(rs.postAction);
                ritualStageActions.Add(rs.pawnLeaveAction);
                ritualStageActions.Add(rs.interruptedAction);

                foreach(RitualStageAction rsa in ritualStageActions)
                {
                    if(rsa is RitualStageAction_ConsumeThing rsa_ct)
                    {
                        if(rsa_ct.stageId == stageId)
                        {
                            targetAction = rsa_ct;
                            break;
                        }
                    }
                }

                if(targetAction.consumptionComplete)
                {
                    return !targetAction.enoughConsumed;
                }
            }
            return true;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref stageId, "stageId");
            base.ExposeData();
        }

    }
}
