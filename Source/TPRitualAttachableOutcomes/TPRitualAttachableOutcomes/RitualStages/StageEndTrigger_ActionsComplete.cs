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
    class StageEndTrigger_ActionsComplete : StageEndTrigger
    {
        // this class serves as an endtrigger for stages where the only thing you want to do is run an action
        public override Trigger MakeTrigger(LordJob_Ritual ritual, TargetInfo spot, IEnumerable<TargetInfo> foci, RitualStage stage)
        {
            return new Trigger_Custom(delegate
            {
                return true;
            });
        }
    }
}
