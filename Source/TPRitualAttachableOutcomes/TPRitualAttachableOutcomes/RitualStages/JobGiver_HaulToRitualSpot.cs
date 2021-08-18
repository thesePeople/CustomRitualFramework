using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace TPRitualAttachableOutcomes
{
    class JobGiver_HaulToRitualSpot : ThinkNode_JobGiver
    {
        public string thingDefName = "";
        public string stageId = "";
        protected override Job TryGiveJob(Pawn pawn)
        {
            LordJob_Ritual lordJob_Ritual = pawn.GetLord().LordJob as LordJob_Ritual;
            // find the StageEndTrigger_DeliveredOrTimeOut with matching stageId (maybe I should have a StageEndTrigger abstract class for stageIds?)

            Predicate<Thing> validator = delegate (Thing t)
            {
                if (t.IsForbidden(pawn))
                {
                    return false;
                }
                if (!HaulAIUtility.PawnCanAutomaticallyHaulFast(pawn, t, forced: false))
                {
                    return false;
                }
                if (pawn.carryTracker.MaxStackSpaceEver(t.def) <= 0)
                {
                    return false;
                }
                IntVec3 foundCell = lordJob_Ritual.selectedTarget.Cell;
                if (foundCell.IsValid)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };
            List<Thing> thingsOfDefName = pawn.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(thingDefName)).FindAll((Thing t) => !t.IsForbidden(pawn));
            Thing thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, thingsOfDefName, PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, validator);
            Job job = JobMaker.MakeJob(JobDefOf.HaulToCell, thing, lordJob_Ritual.selectedTarget.Cell);
            job.count = thing.def.stackLimit - thing.stackCount;
            job.haulMode = HaulMode.ToCellStorage;

            // we also need to signal the stageendtrigger and probably do something about the count and merging?
            // and ideally make the Things dropped forbiddable. We may need a new JobDef and do some of this in individual Toils?
            return job;
        }

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_HaulToRitualSpot obj = (JobGiver_HaulToRitualSpot)base.DeepCopy(resolve);
            obj.thingDefName = thingDefName;
            obj.stageId = stageId;
            return obj;
        }
    }
}
