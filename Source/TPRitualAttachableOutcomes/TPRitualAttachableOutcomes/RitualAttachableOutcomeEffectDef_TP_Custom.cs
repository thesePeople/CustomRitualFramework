using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    [StaticConstructorOnStartup]
    class RitualAttachableOutcomeEffectDef_TP_Custom : DefModExtension
    {
        public List<int> triggerPositivityIndex =  new List<int>();
        public int cooldownDays = 20;
        public List<string> hediffToAdd;
        public float hediffSeverity = 0f;
        public string bodyPart;
        public List<string> hediffToRemove;
        public string thought;
        public string inspiration;
        public List<string> appliesTo;
        public bool appliesToRandom = false;
        public bool invertApply = false;
        public bool forEachPawn = false;
        public bool onlyPositiveOutcomes = true;
        public string item;
        public IntRange baseAmount;
        public IntRange amountPerPawn;
        public bool spawnNearRitual = false;
        public string weather;
        public string abilityToAdd;
        public string letterLabel;
        public string letterText;
        public string letterType = "positive";
        public bool randomFromNode = false;
        public float weight = 1f;
        public string incident = "";
        public IncidentParmsCustom incidentParms = new IncidentParmsCustom();
        public List<RitualAttachableOutcomeEffectDef_TP_Custom_Node> node;
    }

    [StaticConstructorOnStartup]
    class RitualAttachableOutcomeEffectDef_TP_Custom_Node : RitualAttachableOutcomeEffectDef_TP_Custom
    {

    }
}
