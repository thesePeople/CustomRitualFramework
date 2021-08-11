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
    class RitualTargetFilter_IdeoBuilding_Extra : RitualTargetFilter_IdeoBuilding
    {
        public RitualTargetFilter_IdeoBuilding_Extra()
        {
        }

        public RitualTargetFilter_IdeoBuilding_Extra(RitualTargetFilterDef def)
            : base(def)
        {
        }
        protected override IEnumerable<Thing> ExtraCandidates(TargetInfo initiator)
        {
            TargetFilter_ModExtension modExtension = def.GetModExtension<TargetFilter_ModExtension>();
            if (initiator != null && initiator.Thing != null)
            {
                Pawn pawn = (Pawn)initiator.Thing;
                if (pawn != null && modExtension.extraCandidates != null && modExtension.extraCandidates.Count > 0)
                {
                    List <Thing> extraCandidates = new List<Thing>();
                    foreach (string t in modExtension.extraCandidates)
                    {
                        Log.Message("Looking for " + t); 
                        extraCandidates.AddRange(from s in initiator.Map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(t))
                                     where pawn.CanReach(s, PathEndMode.Touch, pawn.NormalMaxDanger())
                                     select s);
                    }
                    return extraCandidates;
                }
            }
            return null;
        }

    }
}
