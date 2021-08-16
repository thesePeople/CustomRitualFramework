using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace TPRitualAttachableOutcomes
{
	class CompAbilityEffect_StartRitual_OnlyObligation : CompAbilityEffect_StartRitual
	{
		public override bool GizmoDisabled(out string reason)
		{
			if (GatheringsUtility.AnyLordJobPreventsNewRituals(parent.pawn.Map))
			{
				reason = "AbilitySpeechDisabledAnotherGatheringInProgress".Translate();
				return true;
			}
			if (Ritual != null && Ritual.targetFilter != null && !Ritual.targetFilter.CanStart(parent.pawn, TargetInfo.Invalid, out var rejectionReason))
			{
				reason = rejectionReason;
				return true;
			}
			if (Ritual.activeObligations == null || Ritual.activeObligations.FirstOrDefault() == null)
            {
				reason = "NoActiveObligations".Translate();
				return true;
            }
			reason = null;
			return false;
		}
	}
}
