using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    class RitualObligationTrigger_Custom : RitualObligationTrigger
    {
		public override void Tick()
		{
			if (!ritual.isAnytime)
			{
				Map myMap = Find.Maps.First((Map m) => m.IsPlayerHome == true);
				foreach(Pawn p in myMap.mapPawns.AllPawns)
                {
					if(!p.NonHumanlikeOrWildMan() && !p.Faction.AllyOrNeutralTo(Find.FactionManager.OfPlayer))
                    {

                    }
                }
				ritual.AddObligation(new RitualObligation(ritual));
				
			}
		}
	}
}
