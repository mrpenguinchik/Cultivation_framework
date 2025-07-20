using CultivationFramework;
using Verse;

namespace TestMod
{
    public class CompProperties_QiSource : CompProperties
    {
        public QiElement element = QiElement.None;
        public float qiAmount = 100f;
        public CompProperties_QiSource()
        {
            compClass = typeof(CompQiSource);
        }
    }

    public class CompQiSource : ThingComp
    {
        public CompProperties_QiSource Props => (CompProperties_QiSource)props;
        public QiElement Element => Props.element;
        public float QiAmount => Props.qiAmount;
    }
}
