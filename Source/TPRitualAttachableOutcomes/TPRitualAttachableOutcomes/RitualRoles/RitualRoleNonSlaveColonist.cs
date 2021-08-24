using RimWorld;
using Verse;

// created by Zezz, uploaded by these people on Zezz's behalf

namespace TPRitualAttachableOutcomes
{
    //To select a colonist but not a slave.
    public class RitualRoleNonSlaveColonist : RitualRole
	{
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
			if (this.requiredWorkType != null && p.WorkTypeIsDisabled(this.requiredWorkType))
			{
				if (!skipReason)
				{
					reason = "MessageRitualRoleMustBeCapableOfGeneric".Translate(base.LabelCap, this.requiredWorkType.gerundLabel);
				}
				return false;
			}
			if (!p.IsFreeNonSlaveColonist) //I just needed to change this from the vanilla one
			{
				if (!skipReason)
				{
					reason = "MessageRitualRoleMustBeColonist".Translate(base.Label);
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
			Scribe_Defs.Look<WorkTypeDef>(ref this.requiredWorkType, "requiredWorkType");
		}

		public WorkTypeDef requiredWorkType;
	}
}
