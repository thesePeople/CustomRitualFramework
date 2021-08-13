using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    class IncidentParmsCustom : IncidentParms
    {
        public string myTarget = "";
        public string myFaction = "";
        public List<string> myLetterHyperLinkedThingsDefs = new List<string>();
        public List<string> myLetterHyperLinkedHediffsDefs = new List<string>();
        public string mySpawnCenter = "";
        public string mySpawnRotation = "";
        public string myRaidStrategy = "";
        public string myRaidArrivalMode = "";

        // I think XML syntax supports this
        /* <myPawnGroups>
         *   <li Class="Dictionary">
         *     <li Class="string">pawn1</li>
         *     <li Class="int">1</li>
         *   </li>
         *  </myPawnGroups>
         */
        // something like that?
        public Dictionary<string, int> myPawnGroups = new Dictionary<string, int>();

        public string myIdeo = "";
        public string myPawnKind = "";
        public string myTraderKind = "";
        public string myQuest = "";
        public string myMechClusterSketch = "";
        public string myControllerPawn = "";
        public string myInfestationLocOverride = "";
        public List<string> myAttackTargets = new List<string>();
        public List<string> myGifts = new List<string>();

        public bool useStoryteller = true;
        public bool scalePoints;

        public new void ExposeData()
        {
            Scribe_Values.Look(ref myTarget, "myTarget");
            Scribe_Values.Look(ref myFaction, "myFaction");
            Scribe_Values.Look(ref myFaction, "myFaction");
            Scribe_Values.Look(ref myTarget, "myTarget");
            Scribe_Values.Look(ref myFaction, "myFaction");
            Scribe_Values.Look(ref myLetterHyperLinkedThingsDefs, "myLetterHyperLinkedThingsDefs");
            Scribe_Values.Look(ref myLetterHyperLinkedHediffsDefs, "myLetterHyperLinkedHediffsDefs");
            Scribe_Values.Look(ref mySpawnCenter, "mySpawnCenter");
            Scribe_Values.Look(ref myRaidStrategy, "myRaidStrategy");
            Scribe_Values.Look(ref myRaidArrivalMode, "myRaidArrivalMode");
            Scribe_Values.Look(ref myPawnGroups, "myPawnGroups");
            Scribe_Values.Look(ref myIdeo, "myIdeo");
            Scribe_Values.Look(ref myPawnKind, "myPawnKind");
            Scribe_Values.Look(ref myTraderKind, "myTraderKind");
            Scribe_Values.Look(ref myFaction, "myFaction");
            Scribe_Values.Look(ref myTarget, "myTarget");
            Scribe_Values.Look(ref myMechClusterSketch, "myMechClusterSketch");
            Scribe_Values.Look(ref myControllerPawn, "myControllerPawn");
            Scribe_Values.Look(ref myInfestationLocOverride, "myInfestationLocOverride");
            Scribe_Values.Look(ref myAttackTargets, "myAttackTargets");
            Scribe_Values.Look(ref myGifts, "myGifts");
            Scribe_Values.Look(ref useStoryteller, "useStoryteller");
            Scribe_Values.Look(ref scalePoints, "scalePoints");

            base.ExposeData();
        }
    }
}
