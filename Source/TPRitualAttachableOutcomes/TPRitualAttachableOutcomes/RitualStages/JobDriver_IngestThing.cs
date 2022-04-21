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

namespace TPRitualAttachableOutcomes.RitualStages
{
    class JobDriver_IngestThing : JobDriver
    {
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			// it'd be really weird if they couldn't reserve a Thing in their own inventory

			// but do we need to check here to make sure it is in their inventory?
			if (!pawn.Reserve(job.GetTarget(TargetIndex.A), job, 1, -1, null, errorOnFailed))
			{
				Log.Message("Error reserving " + TargetA.Label);
				return false;
			}
			pawn.Reserve(job.GetTarget(TargetIndex.A), job);

			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedOrNull(TargetIndex.A);
			Toil notifyStage = new Toil();
			notifyStage.initAction = delegate
			{
				Pawn pawn = notifyStage.actor;
				LordJob_Ritual lordJob_Ritual = pawn.GetLord().LordJob as LordJob_Ritual;
				if (lordJob_Ritual != null)
				{
					JobGiver_IngestThing jobGiver_IngestThing = pawn.CurJob.jobGiver as JobGiver_IngestThing;

					// TODO refactor this with LINQ, although I'm not sure it's technically any more optimal, just shorter
					// CodeGolf is a pointless endeavor 
					if (jobGiver_IngestThing != null)
					{
						string stageId = jobGiver_IngestThing.stageId;
						if (lordJob_Ritual.Ritual != null && lordJob_Ritual.Ritual.behavior != null && lordJob_Ritual.Ritual.behavior.def != null && lordJob_Ritual.Ritual.behavior.def.stages != null)
						{
							foreach (RitualStage rs in lordJob_Ritual.Ritual.behavior.def.stages)
							{
								foreach (StageFailTrigger sft in rs.failTriggers)
								{
									if (sft is StageFailTrigger_Checkable sft_c)
									{
										if (sft_c.stageId == stageId)
										{
											sft_c.failed = false;
											sft_c.hasBeenChecked = true;
										}
									}
								}
							}
						}
					}
				}
			};

			// return the eating things Toils too

			// then just return the notifyStage, which will be a StageFailTrigger_Checkable
			// so in one stage, the pawn(s) will get the Thing to Ingest
			// in the next stage, they will Ingest the thing (and get this JobDriver)
			// and then in a third stage we will have a StageEndTrigger_NoFailures and a StageFailTrigger_Checkable (which may be StageFailTrigger_ThingNotIngested, but that may not be neccessary)
			yield return notifyStage;
		}

	}
}
