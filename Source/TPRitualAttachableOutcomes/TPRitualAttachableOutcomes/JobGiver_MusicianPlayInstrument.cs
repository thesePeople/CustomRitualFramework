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
    class JobGiver_MusicianPlayInstrument : ThinkNode_JobGiver
    {

		public List<string> thingDefNames;
		public int maxDistance;

		protected override Job TryGiveJob(Pawn pawn)
		{
			// look for a thing of thingdefs supplied, for efficiency it'll pick the closest
			// we also look for this in the stage end trigger though, I feel like there has to be a way to prevent having to look for it twice

			Thing building_MusicalInstrument = null;
			
			LordJob_Ritual lordJob_Ritual = pawn.GetLord().LordJob as LordJob_Ritual;

			//Log.Message("found ritual");

			TargetInfo spot = lordJob_Ritual.selectedTarget;

			//Log.Message("found spot, going to look with maxDistance " + this.maxDistance);

			bool foundThing = false;
			foreach (IntVec3 item in CellRect.CenteredOn(spot.Cell, this.maxDistance))
			{
				if(foundThing)
                {
					break;
                }

				if(item == null)
                {
					//Log.Message("can't even find the location what a nerd");
                }
				if(this.thingDefNames == null)
                {
					//Log.Message("ok maybe we can't just pass thingDefNames to this");
                }
				foreach (string s in this.thingDefNames)
				{
					//Log.Message("looking for " + s);
					
					Thing thing = item.GetFirstThing(spot.Map, DefDatabase<ThingDef>.GetNamed(s));

					if (thing != null)
					{
						//Log.Message("found Thing with defName " + thing.def.defName);
					}

					if (thing != null && (thing is Building_MusicalInstrument))
					{
						//Log.Message("Thing found for playing");
						// thing with specified def was found 
						building_MusicalInstrument = thing;
						foundThing = true;
						break;
					}
				}
			}

			if(building_MusicalInstrument == null)
            {
				//Log.Message("we found the thing but it's still null");
            }

			if (building_MusicalInstrument != null && building_MusicalInstrument.Spawned)
			{
				if (!GatheringWorker_Concert.InstrumentAccessible(building_MusicalInstrument as Building_MusicalInstrument, pawn))
				{
					//Log.Message("instrument isn't accessible for some reason");
					return null;
				}
				//Log.Message("actually giving job");
				Job job = JobMaker.MakeJob(JobDefOf.Play_MusicalInstrument, building_MusicalInstrument, building_MusicalInstrument.InteractionCell);
				return job;
			}
			// maybe we should give a default job here just in case?
			return null;
		}

		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_MusicianPlayInstrument obj = (JobGiver_MusicianPlayInstrument)base.DeepCopy(resolve);
			obj.thingDefNames = thingDefNames;
			obj.maxDistance = maxDistance;
			return obj;
		}
	}
}
