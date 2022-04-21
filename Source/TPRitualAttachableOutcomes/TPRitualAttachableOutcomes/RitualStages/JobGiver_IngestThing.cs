using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace TPRitualAttachableOutcomes
{
    class JobGiver_IngestThing : ThinkNode_JobGiver
    {

        //Thing defName of Thing expected in Pawn's inventory to ingest. It will have to have been picked up elsewhere
        public string thingDefName = "";
        public string stageId = "";

        protected override Job TryGiveJob(Pawn pawn)
        {

            return null;
        }

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_IngestThing obj = (JobGiver_IngestThing)base.DeepCopy(resolve);
            obj.thingDefName = thingDefName;
            obj.stageId = stageId;
            return obj;
        }
    }
}
