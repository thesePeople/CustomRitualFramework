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
    class JobDriver_HaulToRitualSpot : JobDriver 
    {
		public Thing ThingToCarry => (Thing)job.GetTarget(TargetIndex.A);

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
		//	Log.Message("making pretoil reservations");

			if (!pawn.Reserve(job.GetTarget(TargetIndex.A), job, 1, -1, null, errorOnFailed))
			{
				Log.Message("Error reserving " + TargetA.Label);
				return false;
			}
			
			// don't think we need to reserve this
			/*if (!pawn.Reserve(job.GetTarget(TargetIndex.B), job, 1, -1, null, errorOnFailed))
			{
				Log.Message("Error reserving " + TargetB.Label);
				return false;
			}*/
			pawn.ReserveAsManyAsPossible(job.GetTargetQueue(TargetIndex.A), job);
			
			// do we really need to reserve a cell?
			//pawn.ReserveAsManyAsPossible(job.GetTargetQueue(TargetIndex.B), job);
			return true;
		}

		private static bool ErrorCheckForCarry(Pawn pawn, Thing haulThing)
		{
			if (!haulThing.Spawned)
			{
				Log.Message(string.Concat(pawn, " tried to start carry ", haulThing, " which isn't spawned."));
				pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
				return true;
			}
			if (haulThing.stackCount == 0)
			{
				Log.Message(string.Concat(pawn, " tried to start carry ", haulThing, " which had stackcount 0."));
				pawn.jobs.EndCurrentJob(JobCondition.Incompletable);
				return true;
			}
			if (pawn.jobs.curJob.count <= 0)
			{
				Log.Error("Invalid count: " + pawn.jobs.curJob.count + ", setting to 1. Job was " + pawn.jobs.curJob);
				pawn.jobs.curJob.count = 1;
			}
			return false;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			//Log.Message("making toils - pawn target is " + pawn.CurJob.targetA);

			this.FailOnDestroyedOrNull(TargetIndex.A);
			// targetB is just a cell
			//this.FailOnDestroyedNullOrForbidden(TargetIndex.B);
			
			Toil getToHaulTarget = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
			Toil startCarryingThing = new Toil(); // Toils_Haul.StartCarryThing(TargetIndex.A, putRemainderInQueue: false, subtractNumTakenFromJobCount: true, failIfStackCountLessThanJobCount: false);
			startCarryingThing.initAction = delegate
			{
			//	Log.Message("initAction for carrying thing");
				Pawn actor = startCarryingThing.actor;
				Job curJob = actor.jobs.curJob;
				Thing thing = curJob.GetTarget(TargetIndex.A).Thing;
				if (!ErrorCheckForCarry(actor, thing))
				{
					Log.Message("well we're here");
					if (curJob.count == 0)
					{
						Log.Message("StartCarryThing job had count = " + curJob.count + ". Job: " + curJob);
						throw new Exception("StartCarryThing job had count = " + curJob.count + ". Job: " + curJob);
					}
					int num = actor.carryTracker.AvailableStackSpace(thing.def);
					if (num == 0)
					{
						Log.Message(string.Concat("StartCarryThing got availableStackSpace ", num, " for haulTarg ", thing, ". Job: ", curJob));
						throw new Exception(string.Concat("StartCarryThing got availableStackSpace ", num, " for haulTarg ", thing, ". Job: ", curJob));
					}
					int num2 = Mathf.Min(curJob.count, num, thing.stackCount);
						if (num2 <= 0)
						{
							Log.Message("StartCarryThing desiredNumToTake = " + num2);
							throw new Exception("StartCarryThing desiredNumToTake = " + num2);
						}
					int stackCount = thing.stackCount;
				//	Log.Message("Actually picking thing up");
						int num3 = actor.carryTracker.TryStartCarry(thing, num2);
					Log.Message("num3 = " + num3);
					if (num3 == 0)
						{
							//Log.Message("ending job");
							actor.jobs.EndCurrentJob(JobCondition.Incompletable);
						}
						if (num3 < stackCount)
						{
							int num4 = curJob.count - num3;
							if (actor.Map.reservationManager.ReservedBy(thing, actor, curJob))
							{
								actor.Map.reservationManager.Release(thing, actor, curJob);
							}
						}
						
							curJob.count -= num3;
						
						curJob.SetTarget(TargetIndex.A, actor.carryTracker.CarriedThing);
						actor.records.Increment(RecordDefOf.ThingsHauled);
					//Log.Message("getting out of carrying thing");
					}
				
			};
		

			// maybe at some point we'll have them collect as much as they can but I gotta figure this out
			//Toil jumpIfAlsoCollectingNextTarget = Toils_Haul.JumpIfAlsoCollectingNextTargetInQueue(getToHaulTarget, TargetIndex.A);
			
			// new Toils_Haul here to carry to ritual spot instead of a container
			//Toil carryToRitualSpot = Toils_Haul.CarryHauledThingToCell(TargetIndex.B);
			Toil gotoRitualSpot = Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
			//yield return Toils_Jump.JumpIf(jumpIfAlsoCollectingNextTarget, () => pawn.IsCarryingThing(ThingToCarry));
			yield return getToHaulTarget;
			yield return startCarryingThing;
			//yield return jumpIfAlsoCollectingNextTarget;
			//yield return carryToRitualSpot;
			yield return gotoRitualSpot;

			//find a cell next to targetIndex.A
			Toil gotoNextCell = new Toil();
			gotoNextCell.initAction = delegate
			{
				Pawn actor = gotoNextCell.actor;
				IntVec3 cell = TargetB.Cell;
				IntVec3 adjCell = cell.RandomAdjacentCell8Way();
				actor.jobs.curJob.targetB = adjCell;
				actor.pather.StartPath(actor.jobs.curJob.targetB, PathEndMode.OnCell);
			};
			Toil placeHauledThingInCell = Toils_Haul.PlaceHauledThingInCell(TargetIndex.B, gotoNextCell, false);
			gotoNextCell.AddFinishAction( delegate
			{
				Pawn actor = gotoNextCell.actor;
				actor.jobs.curDriver.JumpToToil(placeHauledThingInCell);
			});

			gotoNextCell.defaultCompleteMode = ToilCompleteMode.PatherArrival;

			// drop at ritual spot (and notify stageId that Things should be checked)
			yield return placeHauledThingInCell;

			Toil notifyStage = new Toil();
			notifyStage.initAction = delegate
			{
				Pawn pawn = notifyStage.actor;
				LordJob_Ritual lordJob_Ritual = pawn.GetLord().LordJob as LordJob_Ritual;
				if(lordJob_Ritual != null)
                {
					JobGiver_HaulToRitualSpot jobGiver_HaulToRitualSpot = pawn.CurJob.jobGiver as JobGiver_HaulToRitualSpot;
					
					// TODO refactor this with LINQ, although I'm not sure it's technically any more optimal, just shorter
					// CodeGolf is a pointless endeavor 
					if(jobGiver_HaulToRitualSpot != null)
                    {
						string stageId = jobGiver_HaulToRitualSpot.stageId;
						if (lordJob_Ritual.Ritual != null && lordJob_Ritual.Ritual.behavior != null && lordJob_Ritual.Ritual.behavior.def != null && lordJob_Ritual.Ritual.behavior.def.stages != null)
						{
							foreach (RitualStage rs in lordJob_Ritual.Ritual.behavior.def.stages)
							{
								foreach (StageEndTrigger set in rs.endTriggers)
								{
									if(set is StageEndTrigger_ThingDeliveredOrTimeout set_t)
                                    {
										if(set_t.stageId == stageId)
                                        {
											set_t.checkThings = true;
                                        }
                                    }
								}
							}
						}
                    }

				}
			};

			yield return notifyStage;
			
		}
	}
}
