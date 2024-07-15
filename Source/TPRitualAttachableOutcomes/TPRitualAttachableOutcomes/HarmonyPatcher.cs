using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;



namespace TPRitualAttachableOutcomes
{
    [StaticConstructorOnStartup]
    static class HarmonyPatcher
    {

        static HarmonyPatcher()
        {
            Harmony harmony = new Harmony("TPRitualAttachableOutcomes.HarmonyPatches");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(RitualOutcomeEffectWorker_FromQuality))]
    [HarmonyPatch("ApplyAttachableOutcome")]
    public static class Patch_RitualOutcomeEffectWorker_FromQuality
    {
        public static void Prefix(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, RitualOutcomePossibility outcomeChance,  string extraLetterText, ref LookTargets letterLookTargets)
        {
            if(jobRitual == null || jobRitual.Ritual == null || jobRitual.Ritual.attachableOutcomeEffect == null || jobRitual.Ritual.attachableOutcomeEffect.GetModExtension<RitualAttachableOutcomeEffectDef_TP_Custom>() == null)
            {
                return;
            }

            RitualAttachableOutcomeEffectDef_TP_Custom customRitualAttachableOutcomeEffect = jobRitual.Ritual.attachableOutcomeEffect.GetModExtension<RitualAttachableOutcomeEffectDef_TP_Custom>();
            if(customRitualAttachableOutcomeEffect != null)
            {
                jobRitual.Ritual.attachableOutcomeEffect.onlyPositiveOutcomes = customRitualAttachableOutcomeEffect.onlyPositiveOutcomes;
            }

            return;
        }
    }

    [HarmonyPatch(typeof(Precept_Ritual), "get_RepeatPenaltyProgress")]
    internal class Patch_RepeatPenaltyProgress
    {
        private static bool Prefix(Precept_Ritual __instance, ref float __result)
        {
            int coolDownDays = 20;
            Precept_Ritual_Custom customPreceptRitual = __instance.def.GetModExtension<Precept_Ritual_Custom>();
            if(customPreceptRitual != null)
            {
                //Log.Message("customCooldown: " + customPreceptRitual.coolDownDays);
                if (customPreceptRitual.coolDownDays != coolDownDays)
                {
                    coolDownDays = customPreceptRitual.coolDownDays;
                }
            }
            __result = (float)__instance.TicksSinceLastPerformed / (60000f * (float)coolDownDays);
            //Log.Message("RepeatPenaltyProgress: " + __result);
            return false;
        }
    }

    [HarmonyPatch(typeof(Precept_Ritual), "get_RepeatPenaltyTimeLeft")]
    internal class Patch_RepeatPenaltyTimeLeft
    {
        private static bool Prefix(Precept_Ritual __instance, ref string __result)
        {
            int coolDownDays = 20;
            Precept_Ritual_Custom customPreceptRitual = __instance.def.GetModExtension<Precept_Ritual_Custom>();
            if (customPreceptRitual != null)
            {
                //Log.Message("customCooldown: " + customPreceptRitual.coolDownDays);
                if (customPreceptRitual.coolDownDays != 20)
                {
                    coolDownDays = customPreceptRitual.coolDownDays;
                }
            }
            int myTicks = 60000 * coolDownDays;
            __result = (myTicks - __instance.TicksSinceLastPerformed).ToStringTicksToPeriod();
            //Log.Message("RepeatPenaltyTimeLeft: " + __result);
            return false;
        }
    }

    [HarmonyPatch(typeof(Precept_Ritual), "get_RepeatPenaltyActive")]
    internal class Patch_RepeatPenaltyActive
    {
        private static bool Prefix(Precept_Ritual __instance, ref bool __result)
        {
            int coolDownDays = 20;
            Precept_Ritual_Custom customPreceptRitual = __instance.def.GetModExtension<Precept_Ritual_Custom>();
            if (customPreceptRitual != null)
            {
                //Log.Message("customCooldown: " + customPreceptRitual.coolDownDays);
                if (customPreceptRitual.coolDownDays != 20)
                {
                    coolDownDays = customPreceptRitual.coolDownDays;
                }
            }
            int myTicks = 60000 * coolDownDays;
            if (__instance.isAnytime && __instance.lastFinishedTick != -1 && __instance.def.useRepeatPenalty)
            {
                __result = __instance.TicksSinceLastPerformed < myTicks;
                //Log.Message("RitualPenaltyActive: " + __result);
            } else
            {
                return true;
            }
            return false;
        }
    }

    //RepeatQualityPenalty
    [HarmonyPatch(typeof(Precept_Ritual), "get_RepeatQualityPenalty")]
    internal class Patch_RepeatQualityPenalty
    {
        private static bool Prefix(Precept_Ritual __instance, ref float __result)
        {
            int coolDownDays = 20;
            Precept_Ritual_Custom customPreceptRitual = __instance.def.GetModExtension<Precept_Ritual_Custom>();
               
           // Log.Message("getting the real cooldown for " + __instance.def.defName);
            
            if (customPreceptRitual != null)
            {
                //Log.Message("customCooldown: " + customPreceptRitual.coolDownDays + " for ritual " + __instance.def.label); 
                if (customPreceptRitual.coolDownDays != coolDownDays)
                {
                    coolDownDays = customPreceptRitual.coolDownDays;
                }
            }
            float penaltyProgress = (float)__instance.TicksSinceLastPerformed / (60000f * (float)coolDownDays); 
            __result = Mathf.Lerp(-0.95f, 0f, penaltyProgress);
            float origResult = Mathf.Lerp(-0.95f, 0f, (float)__instance.TicksSinceLastPerformed / (60000f * 20f));
            //Log.Message("RepeatQualityPenalty: " + __result + " (instead of " + origResult + ")");
            return false;
        }
    }

   
    // TipMainPart
    [HarmonyPatch(typeof(Precept_Ritual), "TipMainPart")]
    internal class Patch_TipMainPart
    {
        private static string ColorizeWarning(TaggedString title)
        {
            return title.Resolve().Colorize(ColoredText.ThreatColor);
        }

        private static bool Prefix(Precept_Ritual __instance, ref string __result)
        {
            int coolDownDays = 20;
            Precept_Ritual_Custom customPreceptRitual = __instance.def.GetModExtension<Precept_Ritual_Custom>();

            //Log.Message("getting the real cooldown for " + __instance.def.defName);

            if (customPreceptRitual != null)
            {
                //Log.Message("customCooldown: " + customPreceptRitual.coolDownDays + " for ritual " + __instance.def.label);
                if (customPreceptRitual.coolDownDays != coolDownDays)
                {
                    coolDownDays = customPreceptRitual.coolDownDays;
                } 
                else // so far the only reason to override this is for a non-default cooldown, so if it is the default cooldown just go back to the original
                {
                    return true;
                }
            }
            float penaltyProgress = (float)__instance.TicksSinceLastPerformed / (60000f * (float)coolDownDays);

            StringBuilder tmpCompsDesc = new StringBuilder();
            
            if (__instance.RepeatPenaltyActive)
            {
                float num = (float)Mathf.RoundToInt(__instance.RepeatPenaltyProgress * (float)coolDownDays * 10f) / 10f;
                float num2 = (float)Mathf.RoundToInt((1f - __instance.RepeatPenaltyProgress) * (float)coolDownDays * 10f) / 10f;
                tmpCompsDesc.AppendLine(ColorizeWarning("RitualRepeatPenaltyTip".Translate(coolDownDays, num, __instance.RepeatQualityPenalty.ToStringPercent(), num2)));
                tmpCompsDesc.AppendLine();
            }
            if (!__instance.Description.NullOrEmpty())
            {
                tmpCompsDesc.Append(__instance.Description);
            }
            if (__instance.outcomeEffect != null)
            {
                StringBuilder stringBuilder = new StringBuilder(__instance.outcomeEffect.def.Description);
                if (!__instance.outcomeEffect.def.extraPredictedOutcomeDescriptions.NullOrEmpty())
                {
                    foreach (string extraPredictedOutcomeDescription in __instance.outcomeEffect.def.extraPredictedOutcomeDescriptions)
                    {
                        stringBuilder.Append(" " + extraPredictedOutcomeDescription.Formatted(__instance.shortDescOverride ?? __instance.def.label));
                    }
                }
                if (__instance.attachableOutcomeEffect != null)
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.AppendLine();
                    }
                    stringBuilder.AppendInNewLine(__instance.attachableOutcomeEffect.DescriptionForRitualValidated(__instance));
                }
                if (stringBuilder.Length > 0)
                {
                    tmpCompsDesc.AppendLine();
                    tmpCompsDesc.AppendInNewLine(stringBuilder.ToString());
                }
            }
            __result = tmpCompsDesc.ToString();
            return false;
        }
    }

    [HarmonyPatch(typeof(IncidentWorker))]
    [HarmonyPatch("TryExecute")]
    public class Patch_IncidentWorker_TryExecute
    {
        public static void Postfix(IncidentParms parms, IncidentWorker __instance, ref bool __result)
        {
            if (__result)
            {
                //  Log.Message("Incident named " + __instance.def.defName + " successfully fired");
                int num3 = 0;
                foreach (Ideo ideo in Find.FactionManager.OfPlayer.ideos.AllIdeos)
                {
                    num3++;
                    int num2 = 0;
                    foreach (Precept p in ideo.PreceptsListForReading)
                    {
                        num2++;
                        if (p is Precept_Ritual ritual)
                        {
                            
                            foreach (RitualObligationTrigger rots in ritual.obligationTriggers)
                            {
                                int num = 0;
                                if (rots is RitualObligationTrigger_Event rotEvent)
                                {
                                    Log.Message("Incident being passed to trigger " + num + " of ritual " + ritual.def.LabelCap + ": " + ritual.Description + " ( ritual number " + num2 + " and " + num3 +" times we've seen this Ideology, named " + ideo.name + " which should be 1)");
                                    num++;
                                    rotEvent.Notify_Event(__instance.def);
                                }
                            }
                        }
                    }
                }

            }
        }
    }

    [HarmonyPatch(typeof(RitualOutcomeEffectWorker))]
    [HarmonyPatch("MakeMemory")]
    public class Patch_RitualOutcomeEffectWorker_MakeMemory
    {
        public static void Prefix(Pawn p, LordJob_Ritual ritual, ThoughtDef overrideDef = null)
        {
            int ii = 0;
            bool found = false;
            if(ritual == null || ritual.Ritual == null
                || ritual.Ritual.activeObligations == null || ritual.Ritual.sourcePattern == null
                || ritual.Ritual.activeObligations.Count == 0)
            {
                return;
            }
            foreach(RitualObligation r in ritual.Ritual.activeObligations)
            {
                foreach(RitualObligationTriggerProperties t in ritual.Ritual.sourcePattern.ritualObligationTriggers)
                {
                    if(t is RitualObligationTrigger_EventProperties rotEvent)
                    {
                        // I'm just going to go ahead and assume that if this ritual had an event trigger and the ritual occurred, we can remove at least one of the corresponding obligations
                        ritual.Ritual.activeObligations.RemoveAt(ii);
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    break;
                }
                ii++;
            }
        }
    }

    // Command_Ritual
    // DrawIcon
    [HarmonyPatch(typeof(Command_Ritual))]
    [HarmonyPatch("DrawIcon")]
    public class Patch_Command_Ritual_DrawIcon
    {
        private static void DrawIcon(Rect rect, Material buttonMat, GizmoRenderParms parms, Command_Ritual __instance)
        {
            bool isDisabled = (bool)Traverse.Create(__instance).Field("disabled").GetValue();
            Texture badTex = __instance.icon;
            if (badTex == null)
            {
                badTex = BaseContent.BadTex;
            }
            rect.position += new Vector2(__instance.iconOffset.x * rect.size.x, __instance.iconOffset.y * rect.size.y);
            if (!isDisabled || parms.lowLight)
            {
                GUI.color = __instance.IconDrawColor;
            }
            else
            {
                GUI.color = __instance.IconDrawColor.SaturationChanged(0f);
            }
            if (parms.lowLight)
            {
                GUI.color = GUI.color.ToTransparent(0.6f);
            }
            Widgets.DrawTextureFitted(rect, badTex, __instance.iconDrawScale * 0.85f, __instance.iconProportions, __instance.iconTexCoords, __instance.iconAngle, buttonMat);
            GUI.color = Color.white;
        }
        public static bool Prefix(Command_Ritual __instance, Rect rect, Material buttonMat, GizmoRenderParms parms, Precept_Ritual ___ritual, Texture2D ___CooldownBarTex, IntVec2 ___PenaltyIconSize)
        {
            int coolDownDays = 20;
            Precept_Ritual_Custom customPreceptRitual = ___ritual.def.GetModExtension<Precept_Ritual_Custom>();

            //Log.Message("getting the real cooldown for " + __instance.def.defName);

            if (customPreceptRitual != null)
            {
                //Log.Message("customCooldown: " + customPreceptRitual.coolDownDays + " for ritual " + __instance.def.label);
                if (customPreceptRitual.coolDownDays != coolDownDays)
                {
                    coolDownDays = customPreceptRitual.coolDownDays;
                }
                else // so far the only reason to override this is for a non-default cooldown, so if it is the default cooldown just go back to the original
                {
                    return true;
                }
            }

            // some day this may be cleaned up but more likely 20 years from now after I've been killed in a freak skydiving accident, some young modder will come across it and say 'wtf'
            Texture2D ___PenaltyArrowTex = ContentFinder<Texture2D>.Get("UI/Icons/Rituals/QualityPenalty");
            float cooldownTicks = 60000f * coolDownDays;
            //__instance.base.DrawIcon(rect, buttonMat, parms);

            Patch_Command_Ritual_DrawIcon.DrawIcon(rect, buttonMat, parms, __instance);
            if (___ritual.RepeatPenaltyActive)
            {
                float value = Mathf.InverseLerp(cooldownTicks, 0f, ___ritual.TicksSinceLastPerformed);
                Widgets.FillableBar(rect.ContractedBy(1f), Mathf.Clamp01(value), ___CooldownBarTex, null, doBorder: false);
                Verse.Text.Font = GameFont.Tiny;
                Verse.Text.Anchor = UnityEngine.TextAnchor.UpperCenter;
                float num = (float)(cooldownTicks - ___ritual.TicksSinceLastPerformed) / 60000f;
                Widgets.Label(label: "PeriodDays".Translate((!(num >= 1f)) ? ((float)(int)(num * 10f) / 10f) : ((float)Mathf.RoundToInt(num))), rect: rect);
                Text.Anchor = TextAnchor.UpperLeft;
                GUI.DrawTexture(new Rect(rect.xMax - (float)___PenaltyIconSize.x, rect.yMin + 4f, ___PenaltyIconSize.x, ___PenaltyIconSize.z), ___PenaltyArrowTex);
            }

            return false;
        }
    }

    /*
    //Dialog_BeginRitual
   [HarmonyPatch(typeof(Dialog_BeginRitual), MethodType.Constructor)]
   [HarmonyPatch(new Type[] { typeof(string),
       typeof(string),
       typeof(Precept_Ritual),
       typeof(TargetInfo),
       typeof(Map),
       typeof(Dialog_BeginRitual.ActionCallback),
       typeof(Pawn),
       typeof(RitualObligation),
       typeof(Func<Pawn, bool, bool, bool>),
       typeof(string),
       typeof(List<Pawn>),
       typeof(Dictionary<string, Pawn>),
       typeof(string),
       typeof(RitualOutcomeEffectDef),
       typeof(List<string>),
       typeof(Pawn)})]
    public static class Patch_Dialog_BeginRitual
    {
        public static void Prefix(
            ref Precept_Ritual ___ritual,
             ref TargetInfo ___target,
             ref RitualObligation ___obligation,
             ref List<string> ___extraInfos,
             ref Pawn ___selectedPawn,
             ref RitualRoleAssignments ___assignments,
             ref string ___ritualExplanation,
             ref Dialog_BeginRitual.ActionCallback ___action,
             ref Func<Pawn, bool, bool, bool> ___filter,
             ref Map ___map,
             ref string ___ritualLabel,
             ref string ___headerText,
             ref string ___confirmText,
             ref Pawn ___organizer,
             ref bool ___closeOnClickedOutside,
             ref bool ___absorbInputAroundWindow,
             ref bool ___forcePause,
             ref RitualOutcomeEffectDef ___outcome,
            string header,
            string ritualLabel,
            Precept_Ritual ritual,
            TargetInfo target,
            Map map,
            Dialog_BeginRitual.ActionCallback action,
            Pawn organizer,
            RitualObligation obligation,
            Func<Pawn, bool, bool, bool> filter = null,
            string confirmText = null,
            List<Pawn> requiredPawns = null,
            Dictionary<string, Pawn> forcedForRole = null,
            string ritualName = null,
            RitualOutcomeEffectDef outcome = null,
            List<string> extraInfoText = null,
            Pawn selectedPawn = null
            )
        {
    */
            /*ref Precept_Ritual ___ritual, 
             ref TargetInfo ___target,
             ref RitualObligation ___obligation,
             ref List<string> ___extraInfos,
             ref Pawn ___selectedPawn,
             ref RitualRoleAssignments ___assignments,
             ref string ___ritualExplanation,
             ref Dialog_BeginRitual.ActionCallback ___action,
             ref Func<Pawn, bool, bool, bool> ___filter,
             ref Map ___map,
             ref string ___ritualLabel,
             ref string ___headerText,
             ref string ___confirmText,
             ref Pawn ___organizer,
             ref bool ___closeOnClickedOutside, 
             ref bool ___absorbInputAroundWindow,
             ref bool ___forcePause,
             ref RitualOutcomeEffectDef ___outcome,*/

             /*if (!ModLister.CheckRoyaltyOrIdeology("Ritual"))
             {
                 return;
             }

             bool customRoleForOverrideFound = false;
             foreach(RitualRole r in ritual.behavior.def.roles)
             {
                 if(r is RitualRoleColonistDead)
                 {
                     customRoleForOverrideFound = true;
                     break;
                 }
             }

             if(!customRoleForOverrideFound)
             {
                 return;
             }
            
            ___ritual = ritual;
             ___target = target;
             ___obligation = obligation;
             ___extraInfos = extraInfoText;
             ___selectedPawn = selectedPawn;
             ___assignments = new RitualRoleAssignments(ritual);

            // since this is so far oddly specific to dead pawns, let's just make a list of dead pawns on the map

            List<Pawn> list = new List<Pawn>(map.mapPawns.FreeColonistsAndPrisonersSpawned);

            // we just have to hope this isn't woefully worse than anything I could write in LINQ

            List<Thing> thingList = Find.CurrentMap.listerThings.AllThings;
             foreach(Thing t in thingList)
             {
                */ /*if(t is Corpse corpse)
                 {
                     list.Add(corpse.InnerPawn);
                 }*/

                /* if(t is Building_Casket)
                 {
                    Log.Message("we found a casket. Looking for inner pawn. " + t.def.defName);
                    Thing thingWithInnerPawn = t;
                    if((thingWithInnerPawn as Building_Casket) != null)
                    {
                        Building_Casket casket = (Building_Casket)thingWithInnerPawn;
                        Thing contained = casket.ContainedThing;
                        if ((contained as Corpse) != null)
                        {
                            Corpse corpse = (Corpse)contained;
                            Pawn innerPawn = corpse.InnerPawn;
                            Log.Message("actually adding innerpawn");
                            list.Add(innerPawn);
                        }

                    }
                    
                 }
             }

            Log.Message("if we see this message we know that using the InnerPawn of a dead pawn is kind rough");

             for (int i = list.Count - 1; i >= 0; i--)
             {
                 Pawn pawn = list[i];
                if(pawn.Dead)
                {
                    // if this works it'd be such a huge hack and would need a lot more effort below to actually figure out all the filtering
                    // and I think I kinda hate how much effort just trying to do this one thing is becoming
                    // but I'm at least curious to see what will happen
                    break;
                }
                 if (filter != null && !filter(pawn, true, true))
                 {
                     list.RemoveAt(i);
                 }
                 else
                 {
                     bool flag2;
                     bool flag = RitualRoleAssignments.PawnNotAssignableReason(pawn, null, ritual, ___assignments, out flag2) == null || flag2;
                     if (!flag && ritual != null)
                     {
                         foreach (RitualRole ritualRole in ritual.behavior.def.roles)
                         {
                             if ((RitualRoleAssignments.PawnNotAssignableReason(pawn, ritualRole, ritual, ___assignments, out flag2) == null || flag2) && (filter == null || filter(pawn, !(ritualRole is RitualRoleForced), ritualRole.allowOtherIdeos)) && (ritualRole.maxCount > 1 || forcedForRole == null || !forcedForRole.ContainsKey(ritualRole.id)))
                             {
                                 flag = true;
                                 break;
                             }
                         }
                     }
                     if (!flag)
                     {
                         list.RemoveAt(i);
                     }
                 }
             }
             if (requiredPawns != null)
             {
                 foreach (Pawn item in requiredPawns)
                 {
                     if (!list.Contains(item))
                     {
                         list.Add(item);
                     }
                 }
             }
             if (forcedForRole != null)
             {
                 foreach (KeyValuePair<string, Pawn> keyValuePair in forcedForRole)
                 {
                     list.AddDistinct(keyValuePair.Value);
                 }
             }
             if (ritual != null)
             {
                 foreach (RitualRole role2 in ritual.behavior.def.roles)
                 {
                     if (role2.Animal)
                     {
                         list.AddRange(map.mapPawns.SpawnedColonyAnimals.Where((Pawn p) => filter == null || filter(p, arg2: true, arg3: true)));
                         break;
                     }
                 }
             }

             ___assignments.Setup(list, forcedForRole, requiredPawns, selectedPawn);
             ___ritualExplanation = ((ritual != null) ? ritual.ritualExplanation : null);
             ___action = action;
             ___filter = filter;
             ___map = map;
             ___ritualLabel = ritualLabel;
             ___headerText = header;
             ___confirmText = confirmText;
             ___organizer = organizer;
             ___closeOnClickedOutside = true;
             ___absorbInputAroundWindow = true;
             ___forcePause = true;
             ___outcome = ((ritual != null && ritual.outcomeEffect != null) ? ritual.outcomeEffect.def : outcome);
         }
     } */

     //RitualBehaviorWorker
     //CanStartRitualNow
     /*[HarmonyPatch(typeof(RitualBehaviorWorker))]
     [HarmonyPatch("TryExecuteOn")]
     public class Patch_RitualBehaviorWorker_TryExecuteOn
     {
         public static bool Prefix(TargetInfo target, Pawn organizer, Precept_Ritual ritual, RitualObligation obligation, RitualRoleAssignments assignments, bool playerForced = false)
         {
             if(ritual == null || ritual.def == null || ritual.def.GetModExtension<Precept_Ritual_Custom>() == null)
             {
                 return true;
             }
             Log.Message("The ritual has modExtensions");

             Precept_Ritual_Custom customPreceptRitual = ritual.def.GetModExtension<Precept_Ritual_Custom>();

             if(customPreceptRitual == null)
             {
                 Log.Message("maybe I spoke too soon about that modExtension");
                 return true;
             }

             if(customPreceptRitual != null && String.IsNullOrEmpty(customPreceptRitual.building))
             {
                 Log.Message("No building supplied");
                 return true;
             }
             if(target == null || target.Cell == null)
             {
                 Log.Message("Target is, for some reason, invalid now? It was valid before...");
                 return true;
             }

             if(target.Map == null)
             {
                 Log.Message("Now things are getting out of hand");
                 return true;
             }

             if (customPreceptRitual.useRoom && GatheringsUtility.UseWholeRoomAsGatheringArea(target.Cell, target.Map))
             {
                 foreach (IntVec3 cell in target.Cell.GetRoom(target.Map).Cells)
                 {
                     if (cell != null && Check(cell))
                     {
                         return true;
                     }
                 }
             }
             else
             {
                 //Log.Message("Checking general area");
                 foreach (IntVec3 item in CellRect.CenteredOn(target.Cell, customPreceptRitual.maxDistance))
                 {  
                     if (item != null && Check(item))
                     {
                         return true;
                     }
                 }
             }
             return false;
             bool Check(IntVec3 cell)
             {
                 try
                 {
                     //Log.Message("checking first thing");
                     if(cell == null || DefDatabase<ThingDef>.GetNamed(customPreceptRitual.building) == null)
                     {
                         //Log.Message("something is very wrong");
                         // I have no idea how this could be
                         return false;
                     }
                     Thing thing = cell.GetFirstThing(target.Map, DefDatabase<ThingDef>.GetNamed(customPreceptRitual.building));
                     if (thing != null && thing.def != null && thing.def.defName != null) // I don't think defName ever can be null but I'm reallly at a loss right now
                     {
                         Log.Message("Thing is " + thing.def.defName);
                     }
                     if (thing != null && GatheringsUtility.InGatheringArea(cell, target.Cell, target.Map))
                     {
                         return true;
                     }

                 } catch (Exception e)
                 {
                     Log.Message("Exception: " + e.Message);
                 }
                 return false;
             }
         }
     }*/

            /*[HarmonyPatch(typeof(Precept_Ritual), "get_RepeatPenaltyDurationDays")]
            internal class Patch_RepeatPenaltyDurationDays
            {
                private static bool Prefix(Precept_Ritual __instance, ref int __result)
                {
                    int coolDownDays = 20;
                    Precept_Ritual_Custom customPreceptRitual = __instance.def.GetModExtension<Precept_Ritual_Custom>();
                    if (customPreceptRitual != null)
                    {
                        if (customPreceptRitual.coolDownDays != 20)
                        {
                            coolDownDays = customPreceptRitual.coolDownDays;
                        }
                    }
                    __result = coolDownDays;
                    return false;
                }
            }*/
        }
