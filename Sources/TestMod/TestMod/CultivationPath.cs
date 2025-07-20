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

        // List of display names for major stages (e.g. "Foundation", "Core Formation" …)
        public List<string> majorStages = new List<string>();

        // Qi cap per major stage. Key = stage index (0‑based), Value = max Qi.
        public Dictionary<int, float> stageQiCap = new Dictionary<int, float>();

        // Multiplier to the base Qi regeneration rate.
        public float baseRegenMultiplier = 1f;

        // Techniques unlocked automatically when entering this path (defNames).
        public List<string> startingTechniques = new List<string>();
    }

    #endregion
}
