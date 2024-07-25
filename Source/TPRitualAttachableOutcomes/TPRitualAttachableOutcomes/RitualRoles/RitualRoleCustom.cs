using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    internal class RitualRoleCustom : RitualRoleTag
    {
        public int minCount = 1;
        private int psylinkLevel = 0;
        private List<XenotypeDef> allowedXenotypes = new List<XenotypeDef>();
        private List<GeneDef> requiredGenes = new List<GeneDef>();
        private List<HediffDef> requiredHediffs = new List<HediffDef>();
        private List<TraitDef> requiredTrait = new List<TraitDef>();
        private Dictionary<SkillDef, int> requiredSkillsMin = new Dictionary<SkillDef, int>();
        private RoyalTitleDef minTitle;
        private WorkTypeDef requiredWorkType;

        private bool psylinkLevelMaxMode = false;
        private bool excludeXenotypeMode = false;
        private bool excludeGenesMode = false;
        private bool excludeTraitMode = false;
        private bool excludeHediffMode = false;
        private bool skillsMaxMode = false;
        private bool titleMaxMode = false;

        private bool acceptColonist = true;
        private bool acceptSlave = true;
        private bool acceptPrisoner = true;
        
        public override bool AppliesToPawn(Pawn p, out string reason, TargetInfo selectedTarget, LordJob_Ritual ritual = null, RitualRoleAssignments assignments = null, Precept_Ritual precept = null, bool skipReason = false)
        {
            reason = null;
            if (!p.RaceProps.Humanlike)
            {
                if (!skipReason)
                {
                    reason = "MessageRitualRoleMustBeHumanlike".Translate(base.Label);
                }
                return false;
            }
            if (p.IsFreeNonSlaveColonist && !acceptColonist)
            {
                if (!skipReason)
                {
                    reason = "MessageRitualRoleMustNotBeColonist".Translate(base.Label); 
                        //"This role is not for colonists";
                }
                return false;
            }
            if (p.IsSlaveOfColony && !acceptSlave)
            {
                if (!skipReason)
                {
                    reason = "MessageRitualRoleMustNotBeSlave".Translate(base.Label);
                    //"This role is not for slaves";
                }
                return false;
            }
            if (p.IsPrisonerOfColony && !acceptPrisoner)
            {
                if (!skipReason)
                {
                    reason = "MessageRitualRoleMustNotBePrisoner".Translate(base.Label);
                        //"This role is not for prisoners";
                }
                return false;
            }
            //Pyslink level check
            if (p.GetPsylinkLevel() < psylinkLevel ^ psylinkLevelMaxMode)
            {
                if (!skipReason)
                {
                    if (psylinkLevelMaxMode)
                    {
                        reason = "MessageRitualRoleMustBeBelowPsylinkLevel".Translate(base.Label, psylinkLevel); 
                            //"The pawn must have a psylink level below " + psylinkLevel;
                    }
                    else
                    {
                        reason = "MessageRitualRoleMustHavePsylinkLevel".Translate(base.Label, psylinkLevel);
                            //"The pawn must have a psylink level of " + psylinkLevel;
                    }
                }
                return false;
            }

            //Xenotype check
            if (allowedXenotypes.Count > 0 && 
               (!allowedXenotypes.Contains(p.genes.Xenotype) ^ excludeXenotypeMode))
            {
                if (!skipReason)
                {
                    if (excludeXenotypeMode)
                    {
                        reason = "MessageRitualRoleMustNotBeXenotype".Translate(base.Label, p.genes.Xenotype.LabelCap);
                        //"The pawn must not be a " + p.genes.Xenotype.label;
                    }
                    else
                    {
                        reason = "MessageRitualRoleMustBeXenotype".Translate(base.Label);
                        //"The pawn don't have the required xenotype";
                    }
                }
                return false;
            }

            //Genes check
            if (requiredGenes.Count > 0 &&
               (!requiredGenes.All(g => p.genes.HasActiveGene(g) ^ excludeGenesMode)))
            {
                if (!skipReason)
                {
                    if (excludeGenesMode)
                    {
                        reason = "MessageRitualRoleMustNotHaveGenes".Translate(base.Label);
                            //"The pawn has a gene that is not allowed";
                    }
                    else
                    {
                        reason = "MessageRitualRoleMustHaveGenes".Translate(base.Label); 
                            //"The pawn does not have the required genes";
                    }
                }
                return false;
            }

            //Hediff check
            if (requiredHediffs.Count > 0 &&
               (!requiredHediffs.All(h => p.health.hediffSet.HasHediff(h) ^ excludeHediffMode)))
            {
                if (!skipReason)
                {
                    if (excludeHediffMode)
                    {
                        reason = "MessageRitualRoleMustNotHaveHediffs".Translate(base.Label); 
                            //"The pawn has a hediff that is not allowed";
                    }
                    else
                    {
                        reason = "MessageRitualRoleMustHaveHediffs".Translate(base.Label);
                            //"The pawn does not have the required hediffs";
                    }
                }
                return false;
            }

            //Trait check
            if (requiredTrait.Count > 0 &&
               (!requiredTrait.All(t => p.story.traits.HasTrait(t) ^ excludeTraitMode)))
            {
                if (!skipReason)
                {
                    if (excludeTraitMode)
                    {
                        reason = "MessageRitualRoleMustNotHaveTraits".Translate(base.Label);
                            //"The pawn has a trait that is not allowed";
                    }
                    else
                    {
                        reason = "MessageRitualRoleMustHaveTraits".Translate(base.Label);
                            //"The pawn does not have the required traits";
                    }
                }
                return false;
            }

            //Skills check
            if (requiredSkillsMin.Count > 0 && (skillsMaxMode ?
                !requiredSkillsMin.All(s => p.skills.GetSkill(s.Key).Level < s.Value) :
                !requiredSkillsMin.All(s => p.skills.GetSkill(s.Key).Level >= s.Value)))
            {
                if (!skipReason)
                {
                    if (skillsMaxMode)
                    {
                        reason = "MessageRitualRoleMustBeSkilledBelow".Translate(base.Label);
                        //"The pawn is too skilled for this role";
                    }
                    else
                    {
                        reason = "MessageRitualRoleMustBeSkilledAbove".Translate(base.Label);
                            //"The pawn is not skilled enough for this role";
                    }
                }
                return false;
            }

            //Title check
            if (minTitle != null &&
                ((p.royalty?.MainTitle()?.seniority ?? 0) < minTitle.seniority ^ titleMaxMode))
            {
                if (!skipReason)
                {
                    if (titleMaxMode)
                    {
                        reason = "MessageRitualRoleMustBeBelowTitle".Translate(base.Label, minTitle.LabelCap); 
                            //"The pawn must have a title below " + minTitle.label;
                    }
                    else
                    {
                        reason = "MessageRitualRoleMustHaveTitleAbove".Translate(base.Label, minTitle.LabelCap);
                            //"The pawn must have a title of " + minTitle.label + " or higher";
                    }
                }
                return false;
            }

            //Work check
            if (requiredWorkType != null && p.WorkTypeIsDisabled(requiredWorkType))
            {
                if (!skipReason)
                {
                    reason = "MessageRitualRoleMustBeCapableOfGeneric".Translate(base.LabelCap, this.requiredWorkType.gerundLabel);
                }
                return false;
            }

            //child check
            if (!AppliesIfChild(p, out reason, skipReason))
            {
                return false;
            }

            //additional check
            if (!additionalFilter(p, out reason, skipReason))
            {
                return false;
            }

            return tag == null || AppliesToRole(p.Ideo?.GetRole(p), out reason, ritual?.Ritual ?? assignments?.Ritual ?? precept, p, skipReason);
        }

        protected virtual bool additionalFilter(Pawn p, out string reason, bool skipReason = false)
        {
            reason = null;
            return true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.minCount, "minCount");
            Scribe_Values.Look<int>(ref this.psylinkLevel, "minPsylinkLevel");

            Scribe_Collections.Look<XenotypeDef>(ref this.allowedXenotypes, "allowedXenotypes", LookMode.Def);
            Scribe_Collections.Look<GeneDef>(ref this.requiredGenes, "requiredGenes", LookMode.Def);
            Scribe_Collections.Look<HediffDef>(ref this.requiredHediffs, "requiredHediffs", LookMode.Def);
            Scribe_Collections.Look<TraitDef>(ref this.requiredTrait, "requiredTrait", LookMode.Def);
            Scribe_Collections.Look<SkillDef, int>(ref this.requiredSkillsMin, "requiredSkillsMin", LookMode.Def, LookMode.Value);
            Scribe_Defs.Look<RoyalTitleDef>(ref this.minTitle, "minTitle");
            Scribe_Defs.Look<WorkTypeDef>(ref this.requiredWorkType, "requiredWorkType");

            Scribe_Values.Look<bool>(ref this.psylinkLevelMaxMode, "psylinkLevelMaxMode");
            Scribe_Values.Look<bool>(ref this.excludeXenotypeMode, "excludeXenotypeMode");
            Scribe_Values.Look<bool>(ref this.excludeGenesMode, "excludeGenesMode");
            Scribe_Values.Look<bool>(ref this.excludeTraitMode, "excludeTraitMode");
            Scribe_Values.Look<bool>(ref this.excludeHediffMode, "excludeHediffMode");
            Scribe_Values.Look<bool>(ref this.skillsMaxMode, "skillsMaxMode");
            Scribe_Values.Look<bool>(ref this.titleMaxMode, "titleMaxMode");

            Scribe_Values.Look<bool>(ref this.acceptColonist, "acceptColonist");
            Scribe_Values.Look<bool>(ref this.acceptSlave, "acceptSlave");
            Scribe_Values.Look<bool>(ref this.acceptPrisoner, "acceptPrisoner");
        }
    }
}