using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TPRitualAttachableOutcomes
{
    [StaticConstructorOnStartup]
    class Precept_Ritual_Custom : DefModExtension
    {
        public int coolDownDays = 20;
        public string building = "";
        public bool useRoom = true;
        public int maxDistance = 18;

        public void ExposeData()
        {
            Scribe_Values.Look(ref coolDownDays, "coolDownDays");
            Scribe_Values.Look(ref building, "building");
            Scribe_Values.Look(ref useRoom, "useRoom");
            Scribe_Values.Look(ref maxDistance, "maxDistance");
        }
    }
}
