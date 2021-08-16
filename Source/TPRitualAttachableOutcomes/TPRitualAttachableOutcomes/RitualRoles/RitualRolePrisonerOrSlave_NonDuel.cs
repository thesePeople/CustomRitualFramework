using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
    class RitualRolePrisonerOrSlave_NonDuel : RitualRole
    {
		public override bool AppliesToPawn(Pawn p, out string reason, LordJob_Ritual ritual = null, RitualRoleAssignments assignments = null, Precept_Ritual precept = null, bool skipReason = false)
		{
			reason = null;
			if (p.IsPrisonerOfColony || p.IsSlaveOfColony)
			{
				return true;
			}
			if (!skipReason)
			{
				reason = "MessageRitualRoleMustBePrisonerOrSlave".Translate(base.LabelCap);
			}
			return false;
		}

		public override bool AppliesToRole(Precept_Role role, out string reason, Precept_Ritual ritual = null, Pawn p = null, bool skipReason = false)
		{
			reason = null;
			return false;
		}
	}
}
