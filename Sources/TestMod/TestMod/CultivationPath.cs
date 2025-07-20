using CultivationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TestMod
{
    #region Def‑based data classes

    /// <summary>
    /// XML‑Def for describing a cultivation path (Spiritual, Body, etc.).
    /// Define in defs/Cultivation/CultivationPathDefs.xml and reference here automatically.
    /// </summary>
    public class CultivationPathDef : Def
    {
        public CultivationPathType pathType;
        public QiElement element = QiElement.None;

        // List of display names for major stages (e.g. "Foundation", "Core Formation" …)
       public List<CultivationStageDef> stageDefs;
    }

    #endregion

    public class CultivationPath : IExposable
    {
        public CultivationPathDef pathDef;
        public int stageIndex;
        public float xp;
        public float currentQi;
        public float maxQi;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref pathDef, "pathDef");
            Scribe_Values.Look(ref stageIndex, "stageIndex");
            Scribe_Values.Look(ref xp, "xp");
            Scribe_Values.Look(ref currentQi, "currentQi");
            Scribe_Values.Look(ref maxQi, "maxQi");
        }
    }
}
