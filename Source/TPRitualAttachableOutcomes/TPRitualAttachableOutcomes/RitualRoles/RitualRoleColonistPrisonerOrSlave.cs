using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    class RitualRoleColonistPrisonerOrSlave : RitualRole
    {
		public WorkTypeDef requiredWorkType;

		public override bool AppliesToPawn(Pawn p, out string reason, LordJob_Ritual ritual = null, RitualRoleAssignments assignments = null, Precept_Ritual precept = null, bool skipReason = false)
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
			if (requiredWorkType != null && p.WorkTypeIsDisabled(requiredWorkType))
			{
				if (!skipReason)
				{
					reason = "MessageRitualRoleMustBeCapableOfGeneric".Translate(base.LabelCap, requiredWorkType.gerundLabel);
				}
				return false;
			}
			if (!p.Faction.IsPlayerSafe() && !p.IsSlaveOfColony && !p.IsPrisonerOfColony)
			{
				if (!skipReason)
				{
					reason = "MessageRitualRoleMustBeColonistPrisonerOrSlave".Translate(base.Label);
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
			Scribe_Defs.Look(ref requiredWorkType, "requiredWorkType");
		}
	}
}
