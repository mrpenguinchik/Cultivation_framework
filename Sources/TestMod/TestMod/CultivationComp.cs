using CultivationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TestMod
{

    public class CompProperties_Cultivator : CompProperties
    {
        public CompProperties_Cultivator()
        {
            compClass = typeof(CompCultivator);
        }
    }


    /// <summary>
    /// Core component attached to Pawn when they become a cultivator.
    /// Handles Qi, realms, techniques and per‑tick logic.
    /// </summary>
    public class CompCultivator : ThingComp
    {
        public CompProperties_Cultivator Props => (CompProperties_Cultivator)props;

        #region Fields
        public float currentQi;
        public float maxQi;

        public CultivationRealm currentRealm = CultivationRealm.Mortal;
        public int minorStage; // 0‑9 for small steps inside a realm.
            
        public List<PathProgress> paths = new List<PathProgress>();
        public List<CultivationTechnique> knownTechniques = new List<CultivationTechnique>();
        #endregion

        #region Ticking / Qi regeneration
        public override void CompTick()
        {
            base.CompTick();
            if (parent.Map == null) return;
            RegenerateQi();
            foreach (var tech in knownTechniques) tech.Tick();
        }

        private void RegenerateQi()
        {
            // Use pawn's BodySize if available; else fall back to 1f (for non‑pawn things).
            Pawn pawn = parent as Pawn;
            float bodySize = pawn?.BodySize ?? 1f;

            float multiplier = 1f;
            if (currentStage != null)
            {
                foreach (var p in paths)
                {
                    var stage = p.pathDef.stageDefs[p.stageIndex];
                    multiplier *= stage.baseRegenMultiplier;
                }
            }

            float regen = 0.01f * bodySize * multiplier;
            currentQi = Mathf.Min(currentQi + regen, maxQi);
        }
        #endregion

        #region API helpers
        public bool ConsumeQi(float amount)
        {
            if (currentQi < amount) return false;
            currentQi -= amount;
            return true;
        }

        public void GainQi(float amount)
        {
            currentQi = Mathf.Min(currentQi + amount, maxQi);
            // TODO: breakthrough check.
        }
        #endregion

        #region Gizmos
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var tech in knownTechniques)
                yield return tech.GetGizmo(parent as Pawn);
        }
        #endregion

        #region Save / load
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref currentQi, "currentQi");
            Scribe_Values.Look(ref maxQi, "maxQi");
            Scribe_Values.Look(ref currentRealm, "currentRealm", CultivationRealm.Mortal);
            Scribe_Values.Look(ref minorStage, "minorStage");
            Scribe_Defs.Look(ref currentStage, "currentStage");
            Scribe_Collections.Look(ref paths, "paths", LookMode.Deep);
            Scribe_Collections.Look(ref knownTechniques, "knownTechniques", LookMode.Deep);
        }
        #endregion
    }


}
