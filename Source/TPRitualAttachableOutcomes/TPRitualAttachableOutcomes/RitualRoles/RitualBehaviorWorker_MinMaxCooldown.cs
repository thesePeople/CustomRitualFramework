using RimWorld;
using RimWorld.Planet;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TPRitualAttachableOutcomes
{
    public class RitualBehaviorWorker_MinMaxCooldown : RitualBehaviorWorker
    {
        public RitualBehaviorWorker_MinMaxCooldown()
        {
        }
        public RitualBehaviorWorker_MinMaxCooldown(RitualBehaviorDef def)
            : base(def)
        {
        }

        public override string CanStartRitualNow(TargetInfo target, Precept_Ritual ritual, Pawn selectedPawn = null, Dictionary<string, Pawn> forcedForRole = null)
        {
            string result =  base.CanStartRitualNow(target, ritual, selectedPawn, forcedForRole);
            if (result != null)
            {
                return result;
            }
            if (ritual.def.HasModExtension<Precept_Ritual_Custom>() && ritual.isAnytime && ritual.lastFinishedTick != -1)
            {
                Precept_Ritual_Custom custom = ritual.def.GetModExtension<Precept_Ritual_Custom>();
                if (custom.coolDownDays > 0 && !custom.canStartRitualBeforeCooldown && ritual.TicksSinceLastPerformed < custom.coolDownDays * 60000)
                {
                    return "CantStartRitualNotEnoughTimePassed".Translate();
                }
            }
            return null;
        }

        public virtual IEnumerable<string> BlockingIssues(Precept_Ritual ritual, RitualRoleAssignments assignments)
        {   
            //roles
            foreach (RitualRole role in ritual.behavior.def.roles)
            {
                if (role is RitualRoleCustom customRole)
                {
                    if (customRole.minCount > 1)
                    {
                        int num = assignments.AssignedPawns(role).Count();
                        if (num < customRole.minCount)
                        {
                            yield return "CantStartRitualNotEnoughPawnForRole".Translate(role.LabelCap, customRole.minCount, num);
                        }
                    }
                }
            }
            //spectators
            if (ritual.behavior.def.spectatorFilter is RitualSpectatorFilter_Custom spectatorFilter)
            {
                int num = assignments.SpectatorsForReading.Count();
                if (spectatorFilter.minCount > 0 && num < spectatorFilter.minCount)
                {
                    yield return "CantStartRitualNotEnoughSpectators".Translate(spectatorFilter.minCount, num);
                }
                else if (spectatorFilter.maxCount > -1 && num > spectatorFilter.maxCount)
                {
                    yield return "CantStartRitualTooManySpectators".Translate(spectatorFilter.maxCount, num);
                }
            }
        }
    }
}


/*xml translation
    <CantStartRitualNotEnoughTimePassed>Not enough time has passed since the last ritual</CantStartRitualNotEnoughTimePassed>
    <CantStartRitualNotEnoughPawnForRole>Not enough {0} for the ritual ({2}/{1})</CantStartRitualNotEnoughPawnForRole>
    <CantStartRitualNotEnoughSpectators>Not enough spectators for the ritual ({0}/{1})</CantStartRitualNotEnoughSpectators>
    <CantStartRitualTooManySpectators>Too many spectators for the ritual ({0}/{1})</CantStartRitualTooManySpectators>
 
 
 */