using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TestMod
{


    public class PathProgress : IExposable
    {
        public CultivationPathDef pathDef;
        public int stageIndex;
        public float xp;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref pathDef, "pathDef");
            Scribe_Values.Look(ref stageIndex, "stageIndex");
            Scribe_Values.Look(ref xp, "xp");
        }
    }

}
