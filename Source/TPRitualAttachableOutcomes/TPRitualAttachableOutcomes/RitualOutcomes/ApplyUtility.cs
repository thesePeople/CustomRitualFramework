using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    static class ApplyUtility
    {
        public static void ApplyNode(RitualAttachableOutcomeEffectDef_TP_Custom nodeToProcess, Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, RitualOutcomePossibility outcome, ref LookTargets letterLookTargets, List<Pawn> pawnsToApplyTo = null)
        {
           // wowee jeez I feel like this needs a refactor

          //  Log.Message("somehow I'm suspicious about the totalPresence");
           // Log.Message("totalPresence count is " + totalPresence.Count);

            List<int> thisTriggerPositivityIndex = nodeToProcess.triggerPositivityIndex;

            bool runThis = false;
            if (thisTriggerPositivityIndex != null && thisTriggerPositivityIndex.Count > 0)
            {
                foreach (int i in thisTriggerPositivityIndex)
                {
                    //Log.Message("checking positivityIndex " + i);
                    if (outcome.positivityIndex == i)
                    {
                        //Log.Message("foundit! running this node");
                        runThis = true;
                    }
                }
            }
            else
            {
                // default to running it if a positivity index list is not supplied
                runThis = true;
            }

            if (!runThis)
            {
                return;
            }

            List<string> thisAppliesTo = nodeToProcess.appliesTo;
            bool thisAppliesToRandom = nodeToProcess.appliesToRandom;
            bool thisInvertApply = nodeToProcess.invertApply;

            List<string> thisHediff = nodeToProcess.hediffToAdd ?? new List<string>();
            string thisAbilityToAdd = nodeToProcess.abilityToAdd ?? "";
            string thisTraitToAdd = nodeToProcess.trait ?? "";
            bool removeTrait = nodeToProcess.removeTrait;
            float thisTraitDegree = nodeToProcess.traitDegree;
            bool thisTraitForced = nodeToProcess.traitForced;
            int thisPsylinkLevel = nodeToProcess.psylinkLevel;
            float thisHediffSeverity = nodeToProcess.hediffSeverity;
            string thisBodyPart = nodeToProcess.bodyPart ?? "";
            List<string> thisHediffToRemove = nodeToProcess.hediffToRemove ?? new List<string>();

            string thisThought = nodeToProcess.thought ?? "";
            string thisInspiration = nodeToProcess.inspiration ?? "";

            float thisSuppression = nodeToProcess.suppression;
            float thisWillReduction = nodeToProcess.willReduction;
            float thisResistanceReduction = nodeToProcess.resistanceReduction;
            float thisCertainty = nodeToProcess.certainty;

            string thisItem = nodeToProcess.item ?? "";
            int thisBaseAmount = nodeToProcess.baseAmount.RandomInRange;
            int thisAmountPerPawn = nodeToProcess.amountPerPawn.RandomInRange;
            bool thisSpawnNearRitual = nodeToProcess.spawnNearRitual;
            bool thisInstaDrop = nodeToProcess.instaDrop;
            bool thisForbid = nodeToProcess.forbid;
            bool thisRoofPunch = nodeToProcess.roofPunch;

            string thisWeather = nodeToProcess.weather ?? "";

            string thisIncident = nodeToProcess.incident ?? "";

            string thisLetterLabel = nodeToProcess.letterLabel ?? "";
            string thisLetterText = nodeToProcess.letterText ?? "";
            string thisLetterType = nodeToProcess.letterType ?? "";

            bool thisRandomfromNode = nodeToProcess.randomFromNode;
            bool thisForEachPawn = nodeToProcess.forEachPawn;

            bool thisAddInnerPawn = nodeToProcess.addInnerPawn;
            bool thisApplyToInnerPawn = nodeToProcess.applyToInnerPawn;
            bool thisResurrect = nodeToProcess.resurrect;

            List<string> thisRelationshipsToAdd = nodeToProcess.relationshipsToAdd ?? new List<string>();
            List<string> thisRelationshipsToRemove = nodeToProcess.relationshipsToRemove ?? new List<string>();
            List<string> thisRelationshipOperationTargets = nodeToProcess.relationshipOperationTargets ?? new List<string>();

            string thisDefaultXenotypeToSet = nodeToProcess.defaultXenotypeToSet ?? "";
            bool thisClearEndogenes = nodeToProcess.clearEndogenes;
            bool thisGenesInheritable = nodeToProcess.genesInheritable;
            List<string> thisGenesToAdd = nodeToProcess.genesToAdd ?? new List<string>();
            List<string> thisGenesToRemove = nodeToProcess.genesToRemove ?? new List<string>();
            Dictionary<string, string> thisXenotypesTransformationTable = nodeToProcess.xenotypesTransformationTable ?? new Dictionary<string, string>();
            string thisResearchProject = nodeToProcess.researchProject ?? "";
            int thisResearchProjectProgressPercentage = nodeToProcess.researchProjectProgressPercentage;
            Dictionary<string, int> thisSkillsToAdd = nodeToProcess.skillsToAdd ?? new Dictionary<string, int>();

            List < RitualAttachableOutcomeEffectDef_TP_Custom_Node> subNodes = nodeToProcess.node;

            Log.Message("Finished loading in the outcome effect data");


            bool thisTargetLetter = false;
            Map map = totalPresence.First().Key.Map;
            IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
            if(thisSpawnNearRitual)
            {
                intVec = jobRitual.Spot;
            }
            if (pawnsToApplyTo == null)
            {
                pawnsToApplyTo = new List<Pawn>();
                if (thisInvertApply)
                {
                    // this is going to look redundant and yes, could probably be refactored. But it also provides more graceful fallback
                    if (thisAppliesTo != null && thisAppliesTo.Count > 0)
                    {
                        pawnsToApplyTo = totalPresence.Keys.ToList<Pawn>();

                        foreach (string s in thisAppliesTo)
                        {
                           // Log.Message("Removing " + s + " to the apply to list");
                            if (jobRitual.PawnWithRole(s) != null)
                            {
                                pawnsToApplyTo.Remove(jobRitual.PawnWithRole(s));
                            }
                        }
                    }


                }
                else if (thisAppliesTo != null && thisAppliesTo.Count > 0)
                {
                    //Log.Message("checking who to apply the outcome to");
                    // if they've supplied a list of roleIds to apply to, get those
                    foreach (string s in thisAppliesTo)
                    {
                        //Log.Message("Adding " + s + " to the apply to list");
                        if (jobRitual.PawnWithRole(s) != null)
                        {
                            pawnsToApplyTo.Add(jobRitual.PawnWithRole(s));
                        }
                    }
                }
                else if (!thisApplyToInnerPawn)
                {
                   // Log.Message("Adding everyone involved to the apply to list");
                    // they didn't supply a list, so we apply to everyone involved unless they only want to apply it to the inner pawn being resurrected
                    pawnsToApplyTo = totalPresence.Keys.ToList<Pawn>();
                }

                Pawn innerPawn = null;
                // add the innerPawn for the ritual's target if it's a grave or corpse or something
                if (thisAddInnerPawn)
                {
                    Log.Message("attempting to add innerPawn");
                    if (jobRitual.selectedTarget != null && jobRitual.selectedTarget.HasThing)
                    {
                        Thing thingWithInnerPawn = jobRitual.selectedTarget.Thing;
                        if (thingWithInnerPawn is Corpse)
                        {
                            Log.Message("Adding dead pawn from Corpse");
                            innerPawn = ((Corpse)thingWithInnerPawn).InnerPawn;
                        }
                        else if (thingWithInnerPawn is Building_Casket)
                        {
                            Log.Message("Adding a dead pawn");
                            innerPawn = ((Corpse)((Building_Casket)thingWithInnerPawn).ContainedThing).InnerPawn;
                            ((Building_Casket)thingWithInnerPawn).Open();

                        }

                        if (thisResurrect && innerPawn != null)
                        {
                            Log.Message("Attempting to resurrect pawn");
                            ResurrectionUtility.TryResurrect(innerPawn);
                        }

                        if (thisApplyToInnerPawn)
                        {
                            pawnsToApplyTo.Add(innerPawn);
                        }
                    }
                }              

                if (thisAppliesToRandom)
                {
                    // if they set the flag that it should be a random pawn it'll pick a random pawn from the valid list of appliesTo
                    // if they don't include the appliesTo list, it'll be a random pawn from the totalPresence
                    List<Pawn> tmpList = new List<Pawn>();
                    tmpList.Add(pawnsToApplyTo.RandomElement<Pawn>());
                    pawnsToApplyTo = tmpList;

                    // TODO refactor this ;)
                }
            }

            

            // Log.Message("Iterating through pawns to apply to");
            foreach (Pawn pawn in pawnsToApplyTo)
            {
                if (thisRelationshipsToAdd.Count > 0)
                {
                    foreach (string r in thisRelationshipsToAdd)
                    {
                        if (!String.IsNullOrEmpty(r))
                        {
                            if (thisRelationshipOperationTargets.Count > 0)
                            {
                                foreach (string t in thisRelationshipOperationTargets)
                                {
                                    if (!String.IsNullOrEmpty(t))
                                    {
                                        try
                                        {
                                            //Log.Message("CRF ApplyUtility rel try{}: pawn = " + pawn.Name);
                                            //Log.Message("CRF ApplyUtility rel try{}: relationshipToAdd = " + r);
                                            //Log.Message("CRF ApplyUtility rel try{}: relationshipOperationTarget = " + jobRitual.PawnWithRole(t).Name);
                                            foreach (PawnRelationDef relation in pawn.GetRelations(jobRitual.PawnWithRole(t)))
                                            {
                                                if (relation == null)
                                                {
                                                    //Log.Warning("CRF ApplyUtility rel try{}: No relation to remove from " + pawn.Name);
                                                }
                                                else
                                                {
                                                    //Log.Message("CRF ApplyUtility rel try{}: Removing pre-existing relation " + relation + " from " + pawn.Name);
                                                    pawn.relations.RemoveDirectRelation(relation, jobRitual.PawnWithRole(t));
                                                }
                                            }
                                            pawn.relations.AddDirectRelation(DefDatabase<PawnRelationDef>.GetNamed(r), jobRitual.PawnWithRole(t));
                                        }
                                        catch
                                        {
                                            Log.Error("CRF ApplyUtility rel catch {}: FAILED pawn.relations.AddDirectRelation(" + r + ", jobRitual.PawnWithRole(" + t + ")");
                                        }
                                        finally
                                        {
                                            //Log.Message("CRF ApplyUtility rel finally {}: SUCCESS pawn.relations.AddDirectRelation(" + r + ", jobRitual.PawnWithRole(" + t + ") SUCCESS");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (thisRelationshipsToRemove.Count > 0)
                {
                    foreach (string r in thisRelationshipsToRemove)
                    {
                        if (!String.IsNullOrEmpty(r))
                        {
                            if (thisRelationshipOperationTargets.Count > 0)
                            {
                                foreach (string t in thisRelationshipOperationTargets)
                                {
                                    if (!String.IsNullOrEmpty(t))
                                    {
                                        try
                                        {
                                            Log.Message("CRF ApplyUtility rel try{}: pawn = " + pawn.Name);
                                            Log.Message("CRF ApplyUtility rel try{}: relationshipToRemove = " + r);
                                            Log.Message("CRF ApplyUtility rel try{}: relationshipOperationTarget = " + jobRitual.PawnWithRole(t).Name);
                                            foreach (PawnRelationDef relation in pawn.GetRelations(jobRitual.PawnWithRole(t)))
                                            {
                                                if (relation == null)
                                                {
                                                    Log.Warning("CRF ApplyUtility rel try{}: No relation to remove from " + pawn.Name);
                                                }
                                                else if (relation.defName == r)
                                                {
                                                    Log.Message("CRF ApplyUtility rel try{}: Removing relation " + relation + " from " + pawn.Name);
                                                    pawn.relations.RemoveDirectRelation(relation, jobRitual.PawnWithRole(t));
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            Log.Error("CRF ApplyUtility rel catch {}: FAILED pawn.relations.RemoveDirectRelation(" + r + ", jobRitual.PawnWithRole(" + t + ")");
                                        }
                                        finally
                                        {
                                            Log.Message("CRF ApplyUtility rel finally {}: SUCCESS pawn.relations.RemoveDirectRelation(" + r + ", jobRitual.PawnWithRole(" + t + ") SUCCESS");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Log.Message("Pawn " + pawn.Name + " is actually in the list");

                // now if they add a hediff it only gets added to pawns obeying the whole "appliesTo" thing
                // I feel like this whole thing would be a lot shorter if I remembered LINQ better

                // Log.Message("thisHediff is " + thisHediff + ". If you see a blank space then something's wrong");
                if (thisHediff.Count > 0)
                {
                    foreach (string h in thisHediff)
                    {
                        //Log.Message("Applying " + h + " to pawn...");
                        Hediff hediffToAdd = HediffMaker.MakeHediff(DefDatabase<HediffDef>.GetNamed(h), pawn, null);

                        BodyPartRecord bpr = new BodyPartRecord();
                        if (!String.IsNullOrEmpty(thisBodyPart))
                        {
                            bpr = pawn.RaceProps.body.GetPartsWithDef(DefDatabase<BodyPartDef>.GetNamed(thisBodyPart)).First();
                            pawn.health.AddHediff(hediffToAdd, bpr, null, null);
                        }
                        else
                        {
                            pawn.health.AddHediff(hediffToAdd);
                        }

                        if (thisHediffSeverity > 0)
                        {
                            List<Hediff> tmpHediffs = new List<Hediff>();
                            tmpHediffs.AddRange(pawn.health.hediffSet.hediffs);
                            for (int i = 0; i < tmpHediffs.Count; i++)
                            {
                                Hediff tmpHediff = tmpHediffs[i];

                                if (tmpHediff.def.defName == h)
                                {
                                    tmpHediff.Severity += thisHediffSeverity;
                                }
                            }
                        }
                    }
                }

                if (!String.IsNullOrEmpty(thisAbilityToAdd))
                {
                    // Log.Message("Applying " + thisAbilityToAdd + " to pawn...");
                    pawn.abilities.GainAbility(DefDatabase<AbilityDef>.GetNamed(thisAbilityToAdd));
                }

                if (thisPsylinkLevel != 0)
                {
                    pawn.ChangePsylinkLevel(thisPsylinkLevel, true);
                }

                if (!String.IsNullOrEmpty(thisTraitToAdd))
                {
                    if (!removeTrait)
                    {
                        // Log.Message("Applying " + thisTraitToAdd + " to pawn...");
                        Trait traitToAdd = new Trait(DefDatabase<TraitDef>.GetNamed(thisTraitToAdd), (int)thisTraitDegree, thisTraitForced);
                        pawn.story.traits.allTraits.Add(traitToAdd);
                    }
                    else
                    {
                        int t_index = 0;
                        foreach (Trait t in pawn.story.traits.allTraits)
                        {
                            if(t.Label.EqualsIgnoreCase(thisTraitToAdd))
                            {
                                pawn.story.traits.allTraits.RemoveAt(t_index);
                            }
                            t_index++;
                        }
                    }
                }

                if (thisHediffToRemove.Count > 0)
                {
                    foreach (string h in thisHediffToRemove)
                    {
                        List<Hediff> tmpHediffs = new List<Hediff>();
                        tmpHediffs.AddRange(pawn.health.hediffSet.hediffs);
                        for (int i = 0; i < tmpHediffs.Count; i++)
                        {
                            Hediff tmpHediff = tmpHediffs[i];

                            if (tmpHediff.def.defName == h)
                            {
                                pawn.health.RemoveHediff(tmpHediff);
                            }
                        }
                    }
                    
                }

                if(!String.IsNullOrEmpty(thisThought))
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed(thisThought));
                }

                if (!String.IsNullOrEmpty(thisInspiration))
                {
                    pawn.mindState.inspirationHandler.TryStartInspiration(DefDatabase<InspirationDef>.GetNamed(thisInspiration));
                }

                if(thisSuppression != 0 && pawn.IsSlaveOfColony)
                {
                    Need_Suppression need_Suppression = pawn.needs.TryGetNeed<Need_Suppression>();
                    if (need_Suppression != null)
                    {
                        need_Suppression.CurLevel += thisSuppression;
                    }
                }

                if(thisWillReduction != 0 && pawn.IsPrisonerOfColony)
                {
                    if(pawn.guest != null)
                    {
                        pawn.guest.will -= thisWillReduction;
                    }
                }

                if (thisResistanceReduction != 0 && pawn.IsPrisonerOfColony)
                {
                    if (pawn.guest != null)
                    {
                        pawn.guest.will -= thisResistanceReduction;
                    }
                }

                if (thisCertainty != 0)
                {
                    if (pawn.Ideo == jobRitual.Ritual.ideo)
                    {
                        pawn.ideo.OffsetCertainty(thisCertainty);
                    } 
                    else
                    {
                        pawn.ideo.OffsetCertainty(-1f * thisCertainty);
                    }
                }

                if (thisClearEndogenes)
                {   
                    List<Gene> endogenesToRemove = new List<Gene>();
                    foreach (Gene gene in pawn.genes.Endogenes)
                    {
                        endogenesToRemove.Add(gene);
                    }
                    foreach (Gene gene in endogenesToRemove)
                    {
                        pawn.genes.RemoveGene(gene);
                    }
                }
                
                bool applyDefaultXenotype = true;
                if (thisXenotypesTransformationTable.ContainsKey(pawn.genes.Xenotype.defName))
                { 
                    XenotypeDef xenotypeDef = DefDatabase<XenotypeDef>.
                        GetNamed(thisXenotypesTransformationTable[pawn.genes.Xenotype.defName]);
                    bool xenotypeInheritable = xenotypeDef.inheritable;
                    if (thisGenesInheritable)
                    {
                        xenotypeDef.inheritable = true;
                    }
                    pawn.genes.SetXenotype(xenotypeDef);
                    pawn.genes.xenotypeName = xenotypeDef.label;
                    xenotypeDef.inheritable = xenotypeInheritable;
                    applyDefaultXenotype = false;
                }

                if (!String.IsNullOrEmpty(thisDefaultXenotypeToSet) && applyDefaultXenotype)
                {
                    XenotypeDef xenotypeDef = DefDatabase<XenotypeDef>.GetNamed(thisDefaultXenotypeToSet);
                    bool xenotypeInheritable = xenotypeDef.inheritable;
                    if (thisGenesInheritable)
                    {
                        xenotypeDef.inheritable = true;
                    }
                    pawn.genes.SetXenotype(xenotypeDef);
                    pawn.genes.xenotypeName = xenotypeDef.label;
                    xenotypeDef.inheritable = xenotypeInheritable;
                }

                if (thisGenesToRemove.Count > 0)
                {
                    foreach (string geneName in thisGenesToRemove)
                    {
                        GeneDef geneDef = DefDatabase<GeneDef>.GetNamed(geneName);
                        if (pawn.genes.HasActiveGene(geneDef))
                        {
                            Gene gene = pawn.genes.GetGene(geneDef);
                            pawn.genes.RemoveGene(gene);
                        }
                    }
                }

                if (thisGenesToAdd.Count > 0)
                {
                    foreach (string geneName in thisGenesToAdd)
                    {
                        GeneDef geneDef = DefDatabase<GeneDef>.GetNamed(geneName);
                        if (!pawn.genes.HasActiveGene(geneDef))
                        {
                            pawn.genes.AddGene(geneDef, !thisGenesInheritable);
                        }
                    }
                }

                if (thisSkillsToAdd.Count > 0)
                {
                    foreach (KeyValuePair<string, int> kvp in thisSkillsToAdd)
                    {
                        SkillDef skillDef = DefDatabase<SkillDef>.GetNamed(kvp.Key);
                        pawn.skills.GetSkill(skillDef).Level += kvp.Value;
                    }
                }


            }

            

            // if they want to spawn items
            if (!String.IsNullOrEmpty(thisItem))
            {
                // get the things to add to the list

                List<Thing> things = new List<Thing>();
                int numPawns = totalPresence.Count();

                int remainingAmount = thisBaseAmount + thisAmountPerPawn * numPawns;

                while (remainingAmount > 0)
                {
                    Thing thing = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed(thisItem));
                    if (remainingAmount > thing.def.stackLimit)
                    {
                        remainingAmount -= thing.def.stackLimit;
                        thing.stackCount = thing.def.stackLimit;
                    }
                    else
                    {
                        thing.stackCount = remainingAmount;
                        remainingAmount = 0;
                    }
                    things.Add(thing);
                }

                thisTargetLetter = true;

                // actually drop the things
                if (thisInstaDrop && thisSpawnNearRitual)
                {
                    IntVec3 cellDrop = CellFinder.FindNoWipeSpawnLocNear(intVec, map, things.First().def, Rot4.North, 2, 
                        (IntVec3 x) => x.Walkable(map) && x.GetFirstItem(map) == null);
                    for (int i = 0; i < things.Count; i++)
                    {
                        GenPlace.TryPlaceThing(things[i], cellDrop, map, ThingPlaceMode.Near);
                    }
                }
                else
                {
                    DropPodUtility.DropThingsNear(intVec, map, things, 110, canInstaDropDuringInit: thisInstaDrop,
                    leaveSlag: false, canRoofPunch: thisRoofPunch, forbid: thisForbid);
                }
            }

            //research ?
            if (!String.IsNullOrEmpty(thisResearchProject))
            {
                ResearchProjectDef researchProjectDef;
                if (thisResearchProject.Equals("current"))
                {
                    researchProjectDef = Find.ResearchManager.GetProject();
                }
                else if (thisResearchProject.Equals("random"))
                {
                    researchProjectDef = DefDatabase<ResearchProjectDef>.AllDefsListForReading.Where((ResearchProjectDef x) 
                        => x.baseCost > 0f && x.CanStartNow).RandomElement();
                }
                else
                {
                    researchProjectDef = DefDatabase<ResearchProjectDef>.GetNamed(thisResearchProject);
                }

                if (thisResearchProjectProgressPercentage > 0)
                {
                    float amount = (float)researchProjectDef.baseCost * (float)thisResearchProjectProgressPercentage / 100f;
                    Find.ResearchManager.AddProgress(researchProjectDef, amount);
                }
                else
                {
                    Find.ResearchManager.FinishProject(researchProjectDef, doCompletionDialog: true);
                }
            }

            // weather?
            if (!String.IsNullOrEmpty(thisWeather))
            {
                Find.CurrentMap.weatherManager.TransitionTo(DefDatabase<WeatherDef>.GetNamed(thisWeather));
            }

            // force other event?
            if(!String.IsNullOrEmpty(thisIncident))
            {
                IncidentParmsCustom incidentParms = nodeToProcess.incidentParms;
                incidentParms.target = Find.CurrentMap;
                DefDatabase<IncidentDef>.GetNamed(thisIncident).Worker.TryExecute(ConvertCustomIncidentParms(incidentParms));
            }

            // send a letter
            LetterDef letterType = LetterDefOf.PositiveEvent;

            //since thisLetterType is a string and not a String we should be able to do this
            if (thisLetterType == "neutral")
            {
                letterType = LetterDefOf.NeutralEvent;
            }
            else if (thisLetterType == "negative")
            {
                letterType = LetterDefOf.NegativeEvent;
            }

            if (!String.IsNullOrEmpty(thisLetterLabel))
            {
                if (thisTargetLetter)
                {
                    Find.LetterStack.ReceiveLetter(LetterMaker.MakeLetter(new TaggedString(thisLetterLabel), new TaggedString(thisLetterText), letterType, new TargetInfo(intVec, map)));
                }
                else
                {
                    Find.LetterStack.ReceiveLetter(LetterMaker.MakeLetter(new TaggedString(thisLetterLabel), new TaggedString(thisLetterText), letterType));
                }
            }

            // apply subnodes
            if (subNodes != null && subNodes.Count > 0)
            {
                if (thisForEachPawn)
                {
                    foreach (Pawn p in pawnsToApplyTo)
                    {
                        List<Pawn> tmpPawnList = new List<Pawn>();
                        tmpPawnList.Add(p);
                        if (thisRandomfromNode)
                        {
                            ApplyNode(subNodes.RandomElementByWeight((RitualAttachableOutcomeEffectDef_TP_Custom_Node r) => r.weight), totalPresence, jobRitual, outcome, ref letterLookTargets, tmpPawnList);
                        }
                        else
                        {
                            foreach (RitualAttachableOutcomeEffectDef_TP_Custom_Node node in subNodes)
                            {
                                ApplyNode(node, totalPresence, jobRitual, outcome, ref letterLookTargets, tmpPawnList);
                            }
                        }
                    }
                }
                else
                {

                    if (thisRandomfromNode)
                    {
                        ApplyNode(subNodes.RandomElementByWeight((RitualAttachableOutcomeEffectDef_TP_Custom_Node r) => r.weight), totalPresence, jobRitual, outcome, ref letterLookTargets);
                    }
                    else
                    {
                        foreach (RitualAttachableOutcomeEffectDef_TP_Custom_Node node in subNodes)
                        {
                            ApplyNode(node, totalPresence, jobRitual, outcome, ref letterLookTargets);
                        }
                    }
                }
            }
        }
        private static IncidentParms ConvertCustomIncidentParms (IncidentParmsCustom incidentParms)
        {
            IncidentParms newIncidentParms = (IncidentParms)incidentParms;

            if(incidentParms.useStoryteller)
            {
                IncidentCategoryDef icd = IncidentCategoryDefOf.ThreatSmall;
                if(!String.IsNullOrEmpty(incidentParms.threatLevel))
                {
                    icd = DefDatabase<IncidentCategoryDef>.GetNamed(incidentParms.threatLevel);
                }
                newIncidentParms = StorytellerUtility.DefaultParmsNow(icd, Find.CurrentMap);
                newIncidentParms.forced = true;
                return newIncidentParms;
            }

            // some defaults
            newIncidentParms.spawnRotation = Rot4.Random;
            newIncidentParms.target = Find.CurrentMap;
            //this.points = (float)((int)(Find.CurrentMap.strengthWatcher.StrengthRating * 50f));

            //Log.Message("scalePoints is " + incidentParms.scalePoints);
            if (incidentParms.scalePoints)
            {
                // this.points = (float)((int)(Find.CurrentMap.strengthWatcher.StrengthRating * 50f)); 
                float newPoints = StorytellerUtility.DefaultThreatPointsNow(Find.CurrentMap);

              //  Log.Message("setting points to " + newPoints);
                newIncidentParms.points = newPoints;
            }
            //Log.Message("myRaidStrategy is " + incidentParms.myRaidStrategy);
            if (!String.IsNullOrEmpty(incidentParms.myRaidStrategy))
            {
                newIncidentParms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamed(incidentParms.myRaidStrategy);
            }

            if (!String.IsNullOrEmpty(incidentParms.myRaidArrivalMode))
            {
                newIncidentParms.raidArrivalMode = DefDatabase<PawnsArrivalModeDef>.GetNamed(incidentParms.myRaidArrivalMode);

                // it looks like spawnCenter is set by the PawnsArrivalModeWorker. If they try to override the spawnCenter should we do something?
            }

            if (!String.IsNullOrEmpty(incidentParms.myFaction))
            {
                newIncidentParms.faction = Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed(incidentParms.myFaction));
            }

            if (!String.IsNullOrEmpty(incidentParms.myPawnKind))
            {
                newIncidentParms.pawnKind = DefDatabase<PawnKindDef>.GetNamed(incidentParms.myPawnKind);
            }

            if (!String.IsNullOrEmpty(incidentParms.myTraderKind))
            {
                newIncidentParms.traderKind = DefDatabase<TraderKindDef>.GetNamed(incidentParms.myTraderKind);
            }



            newIncidentParms.forced = true;

            return newIncidentParms;
        }
    }

   
}
