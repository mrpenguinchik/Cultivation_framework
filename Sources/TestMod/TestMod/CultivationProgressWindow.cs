using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace TestMod
{
    public class Window_CultivationProgress : Window
    {
        private readonly CompCultivator comp;
        public override Vector2 InitialSize => new Vector2(400f, 300f);
        public Window_CultivationProgress(CompCultivator comp)
        {
            this.comp = comp;
            draggable = true;
            doCloseButton = true;
            doCloseX = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
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

        private void DrawPathRow(Rect rect, PathProgress p, CultivationStageDef stage)
        {
            Widgets.Label(new Rect(rect.x, rect.y, rect.width, 25f), p.pathDef.label + ": " + stage.label);
            float fillPercent = stage.needProgressToNextStage > 0 ? p.xp / stage.needProgressToNextStage : 1f;
            Widgets.FillableBar(new Rect(rect.x, rect.y + 30f, rect.width - 10f, 15f), fillPercent);
        }
    }
}
