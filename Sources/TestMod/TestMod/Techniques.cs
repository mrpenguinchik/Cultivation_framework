using CultivationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace TestMod
{


    /// <summary>
    /// Abstract base class for every cultivation ability.
    /// </summary>
    public abstract class CultivationTechnique : IExposable
    {
        public string defName;
        public string label;
        public CultivationPathType? pathRequirement;
        public QiType? qiTypeRequirement;
        public float qiCost = 10f;
        public int cooldownTicks = 60;
        public int minRealm;
        protected int nextUsableTick;
        public Texture2D icon;

        #region Serialization
        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref defName, "defName");
            Scribe_Values.Look(ref label, "label");
            Scribe_Values.Look(ref pathRequirement, "pathRequirement");
            Scribe_Values.Look(ref qiTypeRequirement, "qiTypeRequirement");
            Scribe_Values.Look(ref qiCost, "qiCost");
            Scribe_Values.Look(ref cooldownTicks, "cooldownTicks");
            Scribe_Values.Look(ref minRealm, "minRealm", 0);
            Scribe_Values.Look(ref nextUsableTick, "nextUsableTick");
            
        }
        #endregion

        #region Logic helpers
        public bool CanCast(Pawn pawn, CompCultivator comp)
        {
            if (Find.TickManager.TicksGame < nextUsableTick) return false;
            if (!comp.HasQi(qiCost, qiTypeRequirement)) return false;
            if (comp.CurrentRealm < minRealm) return false;
            if (pathRequirement.HasValue && !comp.paths.Any(p => p.pathDef.pathType == pathRequirement.Value))
                return false;
            if (qiTypeRequirement.HasValue && !comp.paths.Any(p =>
                    p.stageIndex >= 0 && p.stageIndex < p.pathDef.stageDefs.Count &&
                    p.pathDef.stageDefs[p.stageIndex].qiType == qiTypeRequirement.Value))
                return false;
            return true;
        }
        #endregion

        #region Gizmo builder
        public virtual Command_Action GetGizmo(Pawn pawn)
        {
            return new Command_Action
            {
                defaultLabel = label,
                icon = icon,
                action = () =>
                {
                    var comp = pawn.TryGetComp<CompCultivator>();
                    if (comp != null && CanCast(pawn, comp) && comp.ConsumeQi(qiCost, qiTypeRequirement))
                    {
                        PerformCast(pawn);
                        nextUsableTick = Find.TickManager.TicksGame + cooldownTicks;
                    }
                    else
                    {
                        Messages.Message("Недостаточно ци или способность на перезарядке", MessageTypeDefOf.RejectInput, false);
                    }
                }
            };
        }
        #endregion

        protected abstract void PerformCast(Pawn caster);
        public virtual void Tick() { }
    }

    public abstract class SelfCastTechnique : CultivationTechnique
    {
        protected override void PerformCast(Pawn caster)
        {
            ActivateSelf(caster);
        }

        protected abstract void ActivateSelf(Pawn caster);
    }

    public abstract class TargetCastTechnique : CultivationTechnique
    {
        protected override void PerformCast(Pawn caster)
        {
            LocalTargetInfo target = caster;
            if (caster.jobs?.curJob?.targetA != null)
                target = caster.jobs.curJob.targetA;
            ActivateTarget(caster, target);
        }

        protected abstract void ActivateTarget(Pawn caster, LocalTargetInfo target);
    }

    public abstract class TargetAreaTechnique : CultivationTechnique
    {
        public float areaRadius = 1f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref areaRadius, "areaRadius", 1f);
        }

        protected override void PerformCast(Pawn caster)
        {
            LocalTargetInfo target = caster;
            if (caster.jobs?.curJob?.targetA != null)
                target = caster.jobs.curJob.targetA;
            ActivateOnTargetArea(caster, target);
        }

        protected abstract void ActivateOnTargetArea(Pawn caster, LocalTargetInfo target);
    }

    public abstract class AreaAroundTechnique : CultivationTechnique
    {
        public float areaRadius = 1f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref areaRadius, "areaRadius", 1f);
        }

        protected override void PerformCast(Pawn caster)
        {
            ActivateAroundCaster(caster);
        }

        protected abstract void ActivateAroundCaster(Pawn caster);
    }

    public abstract class SimpleTechnique : CultivationTechnique
    {
        protected override void PerformCast(Pawn caster)
        {
            ActivateSimple(caster);
        }

        protected abstract void ActivateSimple(Pawn caster);
    }

    // Example subclasses

    /// <summary>
    /// Simple ability that grants Qi to the caster.
    /// </summary>
    public class GainQiTechnique : SimpleTechnique
    {
        // Amount of Qi restored when used.
        public float gainAmount = 10f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref gainAmount, "gainAmount", 10f);
        }

        protected override void ActivateSimple(Pawn caster)
        {
            var comp = caster.TryGetComp<CompCultivator>();
            comp?.GainQi(gainAmount);
            Messages.Message(caster.LabelShort + " восстанавливает ци", caster,
                MessageTypeDefOf.PositiveEvent);
        }
    }


 


}
