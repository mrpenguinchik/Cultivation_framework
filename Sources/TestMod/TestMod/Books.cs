using CultivationFramework;
using RimWorld;
using Verse;

namespace TestMod
{
    /// <summary>
    /// Def class for a book that teaches a single cultivation technique.
    /// </summary>
    public class TechniqueBookDef : ThingDef
    {
        public CultivationTechnique technique;
    }

    /// <summary>
    /// Def class for a book that grants an entire cultivation path.
    /// </summary>
    public class PathBookDef : ThingDef
    {
        public CultivationPathDef pathDef;
    }

    public class CompProperties_LearnTechnique : CompProperties_UseEffect
    {
        public CultivationTechnique technique;
        public CompProperties_LearnTechnique()
        {
            compClass = typeof(CompUseEffect_LearnTechnique);
        }
    }

    public class CompUseEffect_LearnTechnique : CompUseEffect
    {
        public CompProperties_LearnTechnique Props => (CompProperties_LearnTechnique)props;
        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);
            var comp = user.TryGetComp<CompCultivator>();
            CultivationTechnique tech = Props.technique;
            if (tech == null && parent.def is TechniqueBookDef def)
                tech = def.technique;
            if (comp != null && tech != null && !comp.knownTechniques.Contains(tech))
            {
                comp.knownTechniques.Add(tech);
                Messages.Message(user.LabelShort + " выучил технику " + tech.label, user, MessageTypeDefOf.PositiveEvent);
            }
        }
    }

    public class CompProperties_LearnPath : CompProperties_UseEffect
    {
        public CultivationPathDef pathDef;
        public CompProperties_LearnPath()
        {
            compClass = typeof(CompUseEffect_LearnPath);
        }
    }

    public class CompUseEffect_LearnPath : CompUseEffect
    {
        public CompProperties_LearnPath Props => (CompProperties_LearnPath)props;
        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);
            var comp = user.TryGetComp<CompCultivator>();
            CultivationPathDef def = Props.pathDef;
            if (def == null && parent.def is PathBookDef pDef)
                def = pDef.pathDef;
            if (comp != null && def != null && !comp.paths.Any(p => p.pathDef == def))
            {
                comp.paths.Add(new CultivationPath { pathDef = def, stageIndex = 0, xp = 0f });
                Messages.Message(user.LabelShort + " изучил путь " + def.label, user, MessageTypeDefOf.PositiveEvent);
            }
        }
    }
}
