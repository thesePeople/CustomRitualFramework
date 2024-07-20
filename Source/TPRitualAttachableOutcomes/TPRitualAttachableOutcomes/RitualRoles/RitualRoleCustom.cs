using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    internal class RitualRoleCustom : RitualRole
    {
        public int minCount = 1;
        private int psylinkLevel = 0;
        private List<XenotypeDef> requiredXenotype = new List<XenotypeDef>();
        private List<GeneDef> requiredGenes = new List<GeneDef>();
        private List<TraitDef> requiredTrait = new List<TraitDef>();
        private List<HediffDef> requiredHediff = new List<HediffDef>();
        private Dictionary<SkillDef, int> requiredSkills = new Dictionary<SkillDef, int>();
        private List<RoyalTitleDef> requiredTitle = new List<RoyalTitleDef>();

        private bool psylinkLevelBelow = false;
        private bool excludeXenotype = false;
        private bool excludeGenes = false;
        private bool excludeTrait = false;
        private bool excludeHediff = false;
        private bool skillsBelow = false;
        private bool excludeTitle = false;

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
                }
                return false;
            }
            if (p.IsSlaveOfColony && !acceptSlave)
            {
                if (!skipReason)
                {
                    reason = "MessageRitualRoleMustNotBeSlave".Translate(base.Label);
                }
                return false;
            }
            if (p.IsPrisonerOfColony && !acceptPrisoner)
            {
                if (!skipReason)
                {
                    reason = "MessageRitualRoleMustNotBePrisoner".Translate(base.Label);
                }
                return false;
            }
            //Pyslink level check
            if (p.GetPsylinkLevel() < psylinkLevel ^ psylinkLevelBelow)
            {
                if (!skipReason)
                {
                    if (psylinkLevelBelow)
                    {
                        reason = "MessageRitualRoleMustBePsylinkLevelBelow".Translate(base.Label, psylinkLevel);
                    }
                    else
                    {
                        reason = "MessageRitualRoleMustBePsylinkLevelAbove".Translate(base.Label, psylinkLevel);
                    }
                }
                return false;
            }

            //Xenotype check
            if (requiredXenotype.Count > 0 && 
               (!requiredXenotype.Contains(p.genes.Xenotype) ^ excludeXenotype))
            {
                if (!skipReason)
                {
                    if (excludeXenotype)
                    {
                        reason = "MessageRitualRoleMustNotBeXenotype".Translate(base.Label);
                    }
                    else
                    {
                        reason = "MessageRitualRoleMustBeXenotype".Translate(base.Label);
                    }
                }
                return false;
            }

            //Genes check
            if (requiredGenes.Count > 0 &&
               (!requiredGenes.All(g => p.genes.HasActiveGene(g) ^ excludeGenes)))
            {
                if (!skipReason)
                {
                    if (excludeGenes)
                    {
                        reason = "MessageRitualRoleMustNotHaveGenes".Translate(base.Label);
                    }
                    else
                    {
                        reason = "MessageRitualRoleMustHaveGenes".Translate(base.Label);
                    }
                }
                return false;
            }

            //Trait check
            if (requiredTrait.Count > 0 &&
               (!requiredTrait.All(t => p.story.traits.HasTrait(t) ^ excludeTrait)))
            {
                if (!skipReason)
                {
                    if (excludeTrait)
                    {
                        reason = "MessageRitualRoleMustNotHaveTrait".Translate(base.Label);
                    }
                    else
                    {
                        reason = "MessageRitualRoleMustHaveTrait".Translate(base.Label);
                    }
                }
                return false;
            }

            //Hediff check
            if (requiredHediff.Count > 0 &&
               (!requiredHediff.All(h => p.health.hediffSet.HasHediff(h) ^ excludeHediff)))
            {
                if (!skipReason)
                {
                    if (excludeHediff)
                    {
                        reason = "MessageRitualRoleMustNotHaveHediff".Translate(base.Label);
                    }
                    else
                    {
                        reason = "MessageRitualRoleMustHaveHediff".Translate(base.Label);
                    }
                }
                return false;
            }

            //Skills check
            if (requiredSkills.Count > 0 &&
               (!requiredSkills.All(s => p.skills.GetSkill(s.Key).Level >= s.Value ^ skillsBelow)))
            {
                if (!skipReason)
                {
                    if (skillsBelow)
                    {
                        reason = "MessageRitualRoleMustHaveSkillsBelow".Translate(base.Label);
                    }
                    else
                    {
                        reason = "MessageRitualRoleMustHaveSkillsAbove".Translate(base.Label);
                    }
                }
                return false;
            }

            //Title check
            if (requiredTitle.Count > 0 &&
               (!requiredTitle.All(t => p.royalty.HasTitle(t) ^ excludeTitle)))
            {
                if (!skipReason)
                {
                    if (excludeTitle)
                    {
                        reason = "MessageRitualRoleMustNotHaveTitle".Translate(base.Label);
                    }
                    else
                    {
                        reason = "MessageRitualRoleMustHaveTitle".Translate(base.Label);
                    }
                }
                return false;
            }

            return true;
        }


        public override bool AppliesToRole(Precept_Role role, out string reason, Precept_Ritual ritual = null, Pawn p = null, bool skipReason = false)
        {
            reason = null;
            return false;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.minCount, "minCount");
            Scribe_Values.Look<int>(ref this.psylinkLevel, "minPsylinkLevel");

            Scribe_Collections.Look<XenotypeDef>(ref this.requiredXenotype, "requiredXenotype", LookMode.Def);
            Scribe_Collections.Look<GeneDef>(ref this.requiredGenes, "requiredGenes", LookMode.Def);
            Scribe_Collections.Look<TraitDef>(ref this.requiredTrait, "requiredTrait", LookMode.Def);
            Scribe_Collections.Look<HediffDef>(ref this.requiredHediff, "requiredHediff", LookMode.Def);
            Scribe_Collections.Look<SkillDef, int>(ref this.requiredSkills, "requiredSkills", LookMode.Def, LookMode.Value);
            Scribe_Collections.Look<RoyalTitleDef>(ref this.requiredTitle, "requiredTitle", LookMode.Def);

            Scribe_Values.Look<bool>(ref this.psylinkLevelBelow, "psylinkLevelBelow");
            Scribe_Values.Look<bool>(ref this.excludeXenotype, "excludeXenotype");
            Scribe_Values.Look<bool>(ref this.excludeGenes, "excludeGenes");
            Scribe_Values.Look<bool>(ref this.excludeTrait, "excludeTrait");
            Scribe_Values.Look<bool>(ref this.excludeHediff, "excludeHediff");
            Scribe_Values.Look<bool>(ref this.skillsBelow, "skillsBelow");
            Scribe_Values.Look<bool>(ref this.excludeTitle, "excludeTitle");

            Scribe_Values.Look<bool>(ref this.acceptColonist, "acceptColonist");
            Scribe_Values.Look<bool>(ref this.acceptSlave, "acceptSlave");
            Scribe_Values.Look<bool>(ref this.acceptPrisoner, "acceptPrisoner");
        }


    }
}
