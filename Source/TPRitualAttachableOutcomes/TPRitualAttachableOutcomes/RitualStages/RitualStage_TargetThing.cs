using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;

namespace TPRitualAttachableOutcomes
{
    class RitualStage_TargetThing : RitualStage
    {
        // a class for targetting a thing in the area
        // can the fail and end triggers access their parent? if so we could use this to simplify the thingpresence triggers
        // then can we get the target from stage in the ritual?

        // we could use this to change the ritual's secondary target, but it'd be nice if we could add a tertiary target to the ritual somehow

        // could we pick the targets in the stageEnd/Fail triggers instead? This RitualStage may be redundant
    }
}

