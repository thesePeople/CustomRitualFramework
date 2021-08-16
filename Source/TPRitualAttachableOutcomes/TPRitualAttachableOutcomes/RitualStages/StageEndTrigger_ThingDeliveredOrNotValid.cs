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
    class StageEndTrigger_ThingDeliveredOrNotValid : StageEndTrigger
    {
        // wait for things to be delivered
        // do we need a RitualStage to deliver Things? 
        // I think pawn delivery is handled with duties

        // does this need the list of Things that should be delivered in addition to the duty containing it? Is there an easy way we can link them?
        public override Trigger MakeTrigger(LordJob_Ritual ritual, TargetInfo spot, IEnumerable<TargetInfo> foci, RitualStage stage)
        {
            return new Trigger_Custom(delegate {
                return true;
            });
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
