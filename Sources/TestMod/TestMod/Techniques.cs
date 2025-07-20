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
        public CultivationRealm minRealm = CultivationRealm.Mortal;
        public float qiCost = 10f;
        public int cooldownTicks = 60;
        protected int nextUsableTick;
        public Texture2D icon;

        #region Serialization
        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref defName, "defName");
            Scribe_Values.Look(ref label, "label");
            Scribe_Values.Look(ref qiCost, "qiCost");
            Scribe_Values.Look(ref cooldownTicks, "cooldownTicks");
            Scribe_Values.Look(ref nextUsableTick, "nextUsableTick");
        }
        #endregion

        #region Logic helpers
        public bool CanCast(Pawn pawn, CompCultivator comp)
        {
            if (Find.TickManager.TicksGame < nextUsableTick) return false;
            if (comp.currentQi < qiCost) return false;
            if (comp.currentRealm < minRealm) return false;
            if (pathRequirement.HasValue && !comp.paths.Any(p => p.pathDef.pathType == pathRequirement.Value))
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
                    if (comp != null && CanCast(pawn, comp) && comp.ConsumeQi(qiCost))
                    {
                        Activate(pawn);
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

        public abstract void Activate(Pawn caster);
        public virtual void Tick() { }
    }

    // Example subclasses
    public class TechniqueProjectile : CultivationTechnique
    {
        public ThingDef projectileDef;
        public float damage = 20f;
        public float range = 24f;

        public override void Activate(Pawn caster)
        {
            LocalTargetInfo target = caster; // TODO: заменить системой наведения цели.
            if (caster.jobs?.curJob?.targetA.HasThing ?? false)
                target = caster.jobs.curJob.targetA;
            Projectile projectile = (Projectile)GenSpawn.Spawn(projectileDef, caster.Position, caster.Map);
            projectile.Launch(caster, target, target, ProjectileHitFlags.IntendedTarget);
        }
    }

    public class TechniqueBuffHediff : CultivationTechnique
    {
        public HediffDef hediffDef;
        public int durationTicks = 600;

        public override void Activate(Pawn caster)
        {
            Hediff hediff = HediffMaker.MakeHediff(hediffDef, caster);
            hediff.Severity = 1f;
            caster.health.AddHediff(hediff);
            // TODO: duration tracking.
        }
    }

    public class TechniqueTeleport : CultivationTechnique
    {
        public int range = 12;

        public override void Activate(Pawn caster)
        {
            IntVec3 dest = CellFinder.RandomClosewalkCellNear(caster.Position, caster.Map, range);
            caster.Position = dest;
            caster.Notify_Teleported();
        }
    }


}
