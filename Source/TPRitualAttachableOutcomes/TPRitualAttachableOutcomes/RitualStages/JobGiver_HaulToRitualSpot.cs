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
            List<Thing> thingsOfDefName = pawn.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(thingDefName)).FindAll((Thing t) => !t.IsForbidden(pawn) && pawn.CanReserveAndReach(t, PathEndMode.Touch, Danger.Deadly));
            Thing thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, thingsOfDefName, PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, validator);
            if (thing != null && pawn.CanReserveAndReach(thing, PathEndMode.Touch, Danger.Deadly, 1, -1, null, false))
            {
                //Log.Message("making job - but pretty sure this succeeds");
                Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("HaulToRitualSpot"), thing, lordJob_Ritual.selectedTarget.Cell);
                job.jobGiver = this;
                job.count = thing.stackCount;
                job.haulMode = HaulMode.ToCellStorage;
                
                return job;
            }
            // couldn't find thing, or couldn't reserve it, or couldn't reach it
            return null;
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
