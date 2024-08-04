using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    public class RitualSpectatorFilter_Custom : RitualSpectatorFilter
    {   
        public int minCount = 0;
        public int maxCount = -1;
        private int psylinkLevel = 0;
        private List<XenotypeDef> allowedXenotypes = new List<XenotypeDef>();
        private List<GeneDef> requiredGenes = new List<GeneDef>();
        private List<HediffDef> requiredHediffs = new List<HediffDef>();
        private List<TraitDef> requiredTrait = new List<TraitDef>();
        private Dictionary<SkillDef, int> requiredSkillsMin = new Dictionary<SkillDef, int>();
        private RoyalTitleDef minTitle;

        private bool psylinkLevelMaxMode = false;
        private bool excludeXenotypeMode = false;
        private bool excludeGenesMode = false;
        private bool excludeTraitMode = false;
        private bool excludeHediffMode = false;
        private bool skillsMaxMode = false;
        private bool titleMaxMode = false;

        private bool allowColonist = true;
        private bool allowSlave = true;
        private bool allowPrisoner = true;
        private bool allowAdult = true;
        private bool allowChild = true;
        

        public override bool Allowed(Pawn p)
        {
            if (!p.RaceProps.Humanlike)
            {
                return false;
            }
            if (p.IsFreeNonSlaveColonist && !allowColonist)
            {
                return false;
            }
            if (p.IsSlaveOfColony && !allowSlave)
            {
                return false;
            }
            if (p.IsPrisonerOfColony && !allowPrisoner)
            {
                return false;
            }

            //adult check
            if (!allowAdult && p.DevelopmentalStage.Adult())
            {
                return false;
            }

            //child check
            if (!allowChild && !p.DevelopmentalStage.Adult())
            {
                return false;
            }

            //Pyslink level check
            if (p.GetPsylinkLevel() < psylinkLevel ^ psylinkLevelMaxMode)
            {
                return false;
            }

            //Xenotype check
            if (allowedXenotypes.Count > 0 && 
               (!allowedXenotypes.Contains(p.genes.Xenotype) ^ excludeXenotypeMode))
            {
                return false;
            }

            //Genes check
            if (requiredGenes.Count > 0 &&
               (!requiredGenes.All(g => p.genes.HasActiveGene(g) ^ excludeGenesMode)))
            {
                return false;
            }

            //Hediff check
            if (requiredHediffs.Count > 0 &&
               (!requiredHediffs.All(h => p.health.hediffSet.HasHediff(h) ^ excludeHediffMode)))
            {
                return false;
            }

            //Trait check
            if (requiredTrait.Count > 0 &&
               (!requiredTrait.All(t => p.story.traits.HasTrait(t) ^ excludeTraitMode)))
            {
                return false;
            }

            //Skills check
            if (requiredSkillsMin.Count > 0 && (skillsMaxMode ?
                !requiredSkillsMin.All(s => p.skills.GetSkill(s.Key).Level < s.Value) :
                !requiredSkillsMin.All(s => p.skills.GetSkill(s.Key).Level >= s.Value)))
            {
                return false;
            }

            //Title check
            if (minTitle != null &&
                ((p.royalty?.MainTitle()?.seniority ?? 0) < minTitle.seniority ^ titleMaxMode))
            {
                return false;
            }

            return true;
        }

        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref this.psylinkLevel, "minPsylinkLevel");
            Scribe_Values.Look<int>(ref this.minCount, "minCount");
            Scribe_Values.Look<int>(ref this.maxCount, "maxCount");

            Scribe_Collections.Look<XenotypeDef>(ref this.allowedXenotypes, "allowedXenotypes", LookMode.Def);
            Scribe_Collections.Look<GeneDef>(ref this.requiredGenes, "requiredGenes", LookMode.Def);
            Scribe_Collections.Look<HediffDef>(ref this.requiredHediffs, "requiredHediffs", LookMode.Def);
            Scribe_Collections.Look<TraitDef>(ref this.requiredTrait, "requiredTrait", LookMode.Def);
            Scribe_Collections.Look<SkillDef, int>(ref this.requiredSkillsMin, "requiredSkillsMin", LookMode.Def, LookMode.Value);
            Scribe_Defs.Look<RoyalTitleDef>(ref this.minTitle, "minTitle");

            Scribe_Values.Look<bool>(ref this.psylinkLevelMaxMode, "psylinkLevelMaxMode");
            Scribe_Values.Look<bool>(ref this.excludeXenotypeMode, "excludeXenotypeMode");
            Scribe_Values.Look<bool>(ref this.excludeGenesMode, "excludeGenesMode");
            Scribe_Values.Look<bool>(ref this.excludeTraitMode, "excludeTraitMode");
            Scribe_Values.Look<bool>(ref this.excludeHediffMode, "excludeHediffMode");
            Scribe_Values.Look<bool>(ref this.skillsMaxMode, "skillsMaxMode");
            Scribe_Values.Look<bool>(ref this.titleMaxMode, "titleMaxMode");

            Scribe_Values.Look<bool>(ref this.allowColonist, "allowColonist");
            Scribe_Values.Look<bool>(ref this.allowSlave, "allowSlave");
            Scribe_Values.Look<bool>(ref this.allowPrisoner, "allowPrisoner");
            Scribe_Values.Look<bool>(ref this.allowAdult, "allowAdult");
            Scribe_Values.Look<bool>(ref this.allowChild, "AllowChild");
        }
    }
}
