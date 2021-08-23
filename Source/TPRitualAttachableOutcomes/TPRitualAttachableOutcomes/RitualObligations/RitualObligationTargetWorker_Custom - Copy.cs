using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    class RitualObligationTargetWorker_CustomGrave : RitualObligationTargetWorker_Custom
	{
		public RitualObligationTargetWorker_CustomGrave()
		{
		}

		public RitualObligationTargetWorker_CustomGrave(RitualObligationTargetFilterDef def)
			: base(def)
		{
		}

		public override IEnumerable<TargetInfo> GetTargets(RitualObligation obligation, Map map)
		{
			if (!ModLister.CheckIdeology("Execution target"))
			{
				yield break;
			}
			if (def != null && def.GetModExtension<TargetFilter_ModExtension>() != null)
			{
				TargetFilter_ModExtension modExtension = def.GetModExtension<TargetFilter_ModExtension>();
				foreach (string s in modExtension.extraCandidates)
				{
					List<Thing> newSpots = (List<Thing>)map.listerThings.ThingsInGroup(ThingRequestGroup.Grave).Where((Thing t) => ((Building_Grave)t).Corpse != null);
					for (int j = 0; j < newSpots.Count; j++)
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
			if (def != null && def.GetModExtension<TargetFilter_ModExtension>() != null)
			{
				TargetFilter_ModExtension modExtension = def.GetModExtension<TargetFilter_ModExtension>();
				foreach (string s in modExtension.extraCandidates)
				{
					if (target.Thing.def.defName == s)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override IEnumerable<string> GetTargetInfos(RitualObligation obligation)
		{
			if (def != null && def.GetModExtension<TargetFilter_ModExtension>() != null)
			{
				TargetFilter_ModExtension modExtension = def.GetModExtension<TargetFilter_ModExtension>();
				foreach (string s in modExtension.extraCandidates)
				{
					yield return s;
				}
			}
			else
            {
				yield return null;
            }
		}

	}
}

