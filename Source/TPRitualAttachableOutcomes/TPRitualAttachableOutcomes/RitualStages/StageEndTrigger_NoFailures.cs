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
    class StageEndTrigger_NoFailures : StageEndTrigger
    {
        public override Trigger MakeTrigger(LordJob_Ritual ritual, TargetInfo spot, IEnumerable<TargetInfo> foci, RitualStage stage)
        {
            // before making the trigger, reset the failtriggers
            foreach (StageFailTrigger failTrigger in stage.failTriggers)
            {
                if (failTrigger is StageFailTrigger_Checkable checkable)
                {
                    checkable.failed = false;
                    checkable.hasBeenChecked = false;
                }
            }

            return new Trigger_Custom(delegate
            {
                // we want to try to end but...
                bool returnVal = true;
                foreach (StageFailTrigger failTrigger in stage.failTriggers)
                {
                    if (failTrigger is StageFailTrigger_Checkable checkable)
                    {
                        // ...if at least one of the failTriggers is not checked, don't end
                        if (!checkable.hasBeenChecked)
                        {
                            returnVal = false;
                        }
                    }
                }

                // check and reset the failtriggers here for the next run
                if(returnVal)
                {
                    foreach (StageFailTrigger failTrigger in stage.failTriggers)
                    {
                        if (failTrigger is StageFailTrigger_Checkable checkable)
                        {
                            checkable.failed = false;
                            checkable.hasBeenChecked = false;
                        }
                    }
                }
                return returnVal;
            });
        }
    }
}
