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
       public List<CultivationStageDef> stageDefs;
   
        public float progress;
    }

    #endregion
}
