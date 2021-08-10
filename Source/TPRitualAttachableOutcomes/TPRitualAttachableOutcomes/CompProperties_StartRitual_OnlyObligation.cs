using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    class CompProperties_StartRitual_OnlyObligation : CompProperties_AbilityStartRitual
    {
        public CompProperties_StartRitual_OnlyObligation()
        {
            compClass = typeof(CompAbilityEffect_StartRitual_OnlyObligation);
        }
    }
}
