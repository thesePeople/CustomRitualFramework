using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    class RitualObligationTargetWorker_AnyRitualSpotOrAltar_Extra : RitualObligationTargetWorker_AnyRitualSpotOrAltar
	{
		public RitualObligationTargetWorker_AnyRitualSpotOrAltar_Extra()
		{
		}

		public RitualObligationTargetWorker_AnyRitualSpotOrAltar_Extra(RitualObligationTargetFilterDef def)
			: base(def)
		{
		}

		public override IEnumerable<TargetInfo> GetTargets(RitualObligation obligation, Map map)
		{
			if (!ModLister.CheckIdeology("Execution target"))
			{
				yield break;
			}
			List<Thing> ritualSpots = map.listerThings.ThingsOfDef(ThingDefOf.RitualSpot);
			for (int j = 0; j < ritualSpots.Count; j++)
			{
				yield return ritualSpots[j];
			}
			List<Thing> ideograms = map.listerThings.ThingsOfDef(ThingDefOf.Ideogram);
			for (int j = 0; j < ideograms.Count; j++)
			{
				yield return ideograms[j];
			}
			foreach (TargetInfo item in RitualObligationTargetWorker_Altar.GetTargetsWorker(obligation, map, parent.ideo))
			{
				yield return item;
			}
			if (def != null && def.GetModExtension<TargetFilter_ModExtension>() != null)
			{
				TargetFilter_ModExtension modExtension = def.GetModExtension<TargetFilter_ModExtension>();
				foreach(string s in modExtension.extraCandidates)
                {
					List<Thing> newSpots = map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(s));
					for(int j = 0; j < newSpots.Count; j++)
					{
						yield return newSpots[j];
					}
                }
			}
		}

		protected override RitualTargetUseReport CanUseTargetInternal(TargetInfo target, RitualObligation obligation)
		{
			if (!target.HasThing)
			{
				return false;
			}
			if (target.Thing.def == ThingDefOf.RitualSpot)
			{
				return true;
			}
			if (def != null && def.GetModExtension<TargetFilter_ModExtension>() != null)
			{
				TargetFilter_ModExtension modExtension = def.GetModExtension<TargetFilter_ModExtension>();
				foreach(string s in modExtension.extraCandidates)
                {
					if(target.Thing.def.defName == s)
                    {
						return true;
                    }
                }
			}
			return RitualObligationTargetWorker_Altar.CanUseTargetWorker(target, obligation, parent.ideo);
		}

		public override IEnumerable<string> GetTargetInfos(RitualObligation obligation)
		{
			foreach (string item in RitualObligationTargetWorker_Altar.GetTargetInfosWorker(parent.ideo))
			{
				yield return item;
			}
			if (def != null && def.GetModExtension<TargetFilter_ModExtension>() != null)
			{
				TargetFilter_ModExtension modExtension = def.GetModExtension<TargetFilter_ModExtension>();
				foreach(string s in modExtension.extraCandidates)
                {
					yield return s;
                }
			}
			yield return ThingDefOf.RitualSpot.LabelCap;
		}

	}
}
