using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace TestMod
{
    public class ITab_Cultivation : ITab
    {
        public ITab_Cultivation()
        {
            size = new Vector2(400f, 300f);
            labelKey = "Cultivation";
        }

        private CompCultivator CultivatorComp => SelPawn?.TryGetComp<CompCultivator>();

        protected override void FillTab()
        {
            var comp = CultivatorComp;
            if (comp == null) return;
            Rect inRect = new Rect(10f, 10f, size.x - 20f, size.y - 20f);
            float curY = 0f;
            foreach (var p in comp.paths)
            {
                if (p.stageIndex < 0 || p.stageIndex >= p.pathDef.stageDefs.Count) continue;
                var stage = p.pathDef.stageDefs[p.stageIndex];
                float rowHeight = 60f;
                Rect row = new Rect(inRect.x, inRect.y + curY, inRect.width, rowHeight);
                DrawPathRow(row, p, stage);
                curY += rowHeight + 10f;
                if (curY > inRect.height - rowHeight)
                    break;
            }
        }

        private void DrawPathRow(Rect rect, CultivationPath p, CultivationStageDef stage)
        {
            Widgets.Label(new Rect(rect.x, rect.y, rect.width, 25f), p.pathDef.label + ": " + stage.label);
            float fillPercent = stage.needProgressToNextStage > 0 ? p.xp / stage.needProgressToNextStage : 1f;
            Widgets.FillableBar(new Rect(rect.x, rect.y + 30f, rect.width - 10f, 15f), fillPercent);
        }
    }
}
