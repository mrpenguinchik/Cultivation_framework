using CultivationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TestMod
{
    public class CultivationStageDef : Def
    {
        public QiType qiType;

        // Multiplier to the base Qi regeneration rate.
        public float baseRegenMultiplier = 1f;

        // Techniques unlocked automatically when reached this stage.
       public List<CultivationTechnique> innateTechniques;
        public float needProgressToNextStage;
    }
}
