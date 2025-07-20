using CultivationFramework;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace TestMod
{
    public class CultivationStageDef : Def
    {
        public QiType qiType;

        // Multiplier to the base Qi regeneration rate.
        public float baseRegenMultiplier = 1f;

        // Techniques unlocked automatically when reached this stage.
        public List<CultivationTechnique> innateTechniques;

        // XP required to reach the next stage.
        public float needProgressToNextStage;

        public virtual CultivationStage CreateStage()
        {
            return new CultivationStage(this);
        }
    }

    public class CultivationStage
    {
        public CultivationStageDef def;

        public CultivationStage(CultivationStageDef def)
        {
            this.def = def;
        }

        public virtual void Breakthrough(CultivationPath path, CompCultivator comp)
        {
            path.currentQi = 0f;
            path.maxQi += 10f;

            if (def.innateTechniques != null)
            {
                foreach (var tech in def.innateTechniques)
                    if (!comp.knownTechniques.Contains(tech))
                        comp.knownTechniques.Add(tech);
            }

            Messages.Message(comp.parent.LabelShort + " совершил прорыв: " + def.label,
                comp.parent, MessageTypeDefOf.PositiveEvent);
        }
    }
}
