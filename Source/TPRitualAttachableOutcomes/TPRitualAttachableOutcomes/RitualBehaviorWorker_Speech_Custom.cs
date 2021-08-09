using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace TPRitualAttachableOutcomes
{
    class RitualBehaviorWorker_Speech_Custom : RitualBehaviorWorker
    {
		public RitualBehaviorWorker_Speech_Custom()
		{
		}

		public RitualBehaviorWorker_Speech_Custom(RitualBehaviorDef def)
			: base(def)
		{
		}

		protected override LordJob CreateLordJob(TargetInfo target, Pawn organizer, Precept_Ritual ritual, RitualObligation obligation, RitualRoleAssignments assignments)
		{
			Pawn organizer2 = assignments.AssignedPawns("speaker").First();
			return new LordJob_Joinable_Speech(target, organizer2, ritual, def.stages, assignments, titleSpeech: false);
		}

		protected override void PostExecute(TargetInfo target, Pawn organizer, Precept_Ritual ritual, RitualObligation obligation, RitualRoleAssignments assignments)
		{
			Pawn arg = assignments.AssignedPawns("speaker").First();
			Find.LetterStack.ReceiveLetter(def.letterTitle.Formatted(ritual.Named("RITUAL")), def.letterText.Formatted(arg.Named("SPEAKER"), ritual.Named("RITUAL"), ritual.ideo.MemberNamePlural.Named("IDEOMEMBERS")) + "\n\n" + ritual.outcomeEffect.ExtraAlertParagraph(ritual), LetterDefOf.PositiveEvent, target);
		}

		public override string CanStartRitualNow(TargetInfo target, Precept_Ritual ritual, Pawn selectedPawn = null, Dictionary<string, Pawn> forcedForRole = null)
		{
			List<RitualRole> realRequiredRoles = this.def.roles;
			
			if(realRequiredRoles.Count > 0)
            {
				foreach(RitualRole s in realRequiredRoles)
                {
					if (s.precept != null)
					{
						//Log.Message("requires role " + s.precept.defName );

						Precept_Role precept_Role = ritual.ideo.RolesListForReading.FirstOrFallback((Precept_Role r) => r.def.defName == s.precept.defName, null);
						if (precept_Role != null && precept_Role.ChosenPawnSingle() == null && precept_Role.ChosenPawns() == null && !s.substitutable)
						{
							return "CantStartRitualRoleNotAssigned".Translate(precept_Role.LabelCap);
						}
					}
				}
            }
			return base.CanStartRitualNow(target, ritual, selectedPawn, forcedForRole);
		}
	}
}
