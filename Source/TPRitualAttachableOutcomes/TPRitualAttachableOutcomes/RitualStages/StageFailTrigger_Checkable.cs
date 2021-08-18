using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace TPRitualAttachableOutcomes
{
    public abstract class StageFailTrigger_Checkable: StageFailTrigger
    {
        public bool failed = false;
        public bool hasBeenChecked = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref failed, "failed");
            Scribe_Values.Look(ref hasBeenChecked, "hasBeenChecked");
            base.ExposeData();
        }
    }
}
