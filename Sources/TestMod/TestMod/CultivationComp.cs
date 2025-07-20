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
        public bool cultivateMultiplePaths = true;
        public float progressPerTick = 1f;
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
        public List<PathProgress> paths = new List<PathProgress>();
        public List<CultivationTechnique> knownTechniques = new List<CultivationTechnique>();
        public int activePathIndex;
        public int CurrentRealm => paths.Any() ? paths.Max(p => p.stageIndex) : 0;
        #endregion

        #region Ticking / Qi regeneration
        public override void CompTick()
        {
            base.CompTick();
            if (parent.Map == null) return;
            RegenerateQi();
            HandlePathProgress();
            foreach (var tech in knownTechniques) tech.Tick();
        }

        private CompQiSource FindNearbyQiSource(float radius = 5f)
        {
            if (parent.Map == null) return null;
            Map map = parent.Map;
            foreach (var cell in GenRadial.RadialCellsAround(parent.Position, radius, true))
            {
                if (!cell.InBounds(map)) continue;
                var things = cell.GetThingList(map);
                foreach (var t in things)
                {
                    var comp = t.TryGetComp<CompQiSource>();
                    if (comp != null && comp.QiAmount > 0f)
                        return comp;
                }
            }
            return null;
        }

        private void RegenerateQi()
        {
            // Use pawn's BodySize if available; else fall back to 1f (for non‑pawn things).
            Pawn pawn = parent as Pawn;
            float bodySize = pawn?.BodySize ?? 1f;

            var source = FindNearbyQiSource();
            if (source == null) return; // cannot cultivate away from Qi

            foreach (var p in paths)
            {
                float multiplier = 1f;
                if (p.stageIndex >= 0 && p.stageIndex < p.pathDef.stageDefs.Count)
                {
                    var stage = p.pathDef.stageDefs[p.stageIndex];
                    multiplier *= stage.baseRegenMultiplier;
                }

                var elem = p.pathDef.element;
                if (source.Element == elem)
                    multiplier *= 1.5f;
                else if (source.Element.Feeds(elem))
                    multiplier *= 1.2f;
                else if (source.Element.Suppresses(elem))
                    multiplier *= 0.5f;

                float regen = 0.01f * bodySize * multiplier;
                p.currentQi = Mathf.Min(p.currentQi + regen, p.maxQi);
            }
        }

        private void HandlePathProgress()
        {
            List<PathProgress> active = new List<PathProgress>();
            if (Props.cultivateMultiplePaths)
            {
                active.AddRange(paths);
            }
            else if (activePathIndex >= 0 && activePathIndex < paths.Count)
            {
                active.Add(paths[activePathIndex]);
            }

            if (active.Count == 0) return;

            float amount = Props.progressPerTick / active.Count;
            foreach (var p in active)
            {
                if (p.stageIndex < 0 || p.stageIndex >= p.pathDef.stageDefs.Count) continue;
                if (p.currentQi < p.maxQi) continue;
                var stage = p.pathDef.stageDefs[p.stageIndex];
                p.xp += amount;
                if (p.xp >= stage.needProgressToNextStage && p.stageIndex < p.pathDef.stageDefs.Count - 1)
                {
                    p.xp -= stage.needProgressToNextStage;
                    p.stageIndex++;
                    var nextStage = p.pathDef.stageDefs[p.stageIndex];
                    if (nextStage.innateTechniques != null)
                    {
                        foreach (var tech in nextStage.innateTechniques)
                            if (!knownTechniques.Contains(tech))
                                knownTechniques.Add(tech);
                    }
                }
            }
            // Realm is derived dynamically from paths
        }
        #endregion

        #region API helpers
        public bool ConsumeQi(float amount, QiType? type = null)
        {
            foreach (var p in paths)
            {
                if (type.HasValue)
                {
                    if (p.stageIndex < 0 || p.stageIndex >= p.pathDef.stageDefs.Count) continue;
                    var stage = p.pathDef.stageDefs[p.stageIndex];
                    if (stage.qiType != type.Value) continue;
                }

                if (p.currentQi >= amount)
                {
                    p.currentQi -= amount;
                    return true;
                }
            }
            return false;
        }

        public bool HasQi(float amount, QiType? type = null)
        {
            foreach (var p in paths)
            {
                if (type.HasValue)
                {
                    if (p.stageIndex < 0 || p.stageIndex >= p.pathDef.stageDefs.Count) continue;
                    var stage = p.pathDef.stageDefs[p.stageIndex];
                    if (stage.qiType != type.Value) continue;
                }

                if (p.currentQi >= amount)
                    return true;
            }
            return false;
        }

        public void GainQi(float amount, QiType? type = null)
        {
            foreach (var p in paths)
            {
                if (type.HasValue)
                {
                    if (p.stageIndex < 0 || p.stageIndex >= p.pathDef.stageDefs.Count) continue;
                    var stage = p.pathDef.stageDefs[p.stageIndex];
                    if (stage.qiType != type.Value) continue;
                }

                p.currentQi = Mathf.Min(p.currentQi + amount, p.maxQi);
            }
            // TODO: breakthrough check.
        }
        #endregion

        #region Gizmos
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_Action
            {
                defaultLabel = "Cultivation",
                defaultDesc = "View cultivation progress",
                action = () => Find.WindowStack.Add(new Window_CultivationProgress(this))
            };
            foreach (var tech in knownTechniques)
                yield return tech.GetGizmo(parent as Pawn);
        }
        #endregion

        #region Save / load
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref activePathIndex, "activePathIndex", 0);
            Scribe_Collections.Look(ref paths, "paths", LookMode.Deep);
            Scribe_Collections.Look(ref knownTechniques, "knownTechniques", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (activePathIndex >= paths.Count)
                    activePathIndex = paths.Count - 1;
            }
        }
        #endregion
    }


}
