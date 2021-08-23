using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using Verse.AI;

namespace TPRitualAttachableOutcomes
{
    class RitualTargetFilter_Custom : RitualTargetFilter
    {
		public RitualTargetFilter_Custom()
		{
		}

		public RitualTargetFilter_Custom(RitualTargetFilterDef def)
			: base(def)
		{
		}

		public override bool CanStart(TargetInfo initiator, TargetInfo selectedTarget, out string rejectionReason)
		{
			TargetInfo targetInfo = BestTarget(initiator, selectedTarget);
			rejectionReason = "";
			if (!targetInfo.IsValid)
			{
				TargetFilter_ModExtension modExtension = def.GetModExtension<TargetFilter_ModExtension>();
				string missingDesc = modExtension.missingDesc;
				if (!String.IsNullOrEmpty(missingDesc))
				{
					rejectionReason = missingDesc;
				}
				else
				{
					rejectionReason = "RequiredBuildingsNotFound".Translate();
				}
				return false;
			}
			return true;
		}

		public override TargetInfo BestTarget(TargetInfo initiator, TargetInfo selectedTarget)
		{
			Pawn pawn = initiator.Thing as Pawn;
			if (pawn == null)
			{
				return null;
			}
			List<Building> buildingList = new List<Building>();

			TargetFilter_ModExtension modExtension = def.GetModExtension<TargetFilter_ModExtension>();
			if (modExtension.extraCandidates != null && modExtension.extraCandidates.Count > 0)
            {
				foreach(string s in modExtension.extraCandidates)
                {
					buildingList.AddRange(initiator.Thing.Map.listerBuildings.AllBuildingsColonistOfDef(DefDatabase<ThingDef>.GetNamed(s)));
                }
            }
            else
            {
				return null;
            }

			Thing thing = null;
			float num = 99999f;
			
			foreach (Building item2 in buildingList)
			{
				if (item2.def.isAltar && pawn.CanReach(item2, PathEndMode.Touch, pawn.NormalMaxDanger()))
				{
					int lengthHorizontalSquared = (pawn.Position - item2.Position).LengthHorizontalSquared;
					if ((float)lengthHorizontalSquared < num)
					{
						thing = item2;
						num = lengthHorizontalSquared;
					}
				}
			}
			
			if (thing == null && def.fallbackToRitualSpot)
			{
				foreach (Thing item3 in pawn.Map.listerThings.ThingsOfDef(ThingDefOf.RitualSpot))
				{
					if (pawn.CanReach(item3, PathEndMode.Touch, pawn.NormalMaxDanger()))
					{
						int lengthHorizontalSquared2 = (pawn.Position - item3.Position).LengthHorizontalSquared;
						if ((float)lengthHorizontalSquared2 < num)
						{
							thing = item3;
							num = lengthHorizontalSquared2;
						}
					}
				}
			}
			return thing;
		}

		public override IEnumerable<string> GetTargetInfos(TargetInfo initiator)
		{
			List<Building> buildingList = new List<Building>();

			TargetFilter_ModExtension modExtension = def.GetModExtension<TargetFilter_ModExtension>();
			if (modExtension.extraCandidates != null && modExtension.extraCandidates.Count > 0)
			{
				foreach (string s in modExtension.extraCandidates)
				{
					buildingList.AddRange(initiator.Thing.Map.listerBuildings.AllBuildingsColonistOfDef(DefDatabase<ThingDef>.GetNamed(s)));
				}
				foreach (Building b in buildingList)
				{
					yield return b.LabelCap;
				}
			}
			else
            {
				yield return null;
            }
			
		}
	}
}

