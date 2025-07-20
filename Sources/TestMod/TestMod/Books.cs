using CultivationFramework;
using RimWorld;
using Verse;

namespace TestMod
{
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
            if (comp != null && Props.technique != null && !comp.knownTechniques.Contains(Props.technique))
            {
                comp.knownTechniques.Add(Props.technique);
                Messages.Message(user.LabelShort + " выучил технику " + Props.technique.label, user, MessageTypeDefOf.PositiveEvent);
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
            if (comp != null && Props.pathDef != null && !comp.paths.Any(p => p.pathDef == Props.pathDef))
            {
                comp.paths.Add(new CultivationPath { pathDef = Props.pathDef, stageIndex = 0, xp = 0f });
                Messages.Message(user.LabelShort + " изучил путь " + Props.pathDef.label, user, MessageTypeDefOf.PositiveEvent);
            }
        }
    }
}
