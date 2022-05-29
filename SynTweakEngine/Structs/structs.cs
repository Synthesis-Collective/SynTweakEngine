using System.Collections.Generic;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace SynTweakEngine.Structs
{
    public struct ChangeSPELMGEF {
        public FormLinkNullable<IMagicEffectGetter> SetTo;
        public int Position;
        public int Area;
        public float Mag;
        public int Duration;
    }
    public struct SPELTweak {
        public bool ClearDescription;
        public FormLink<ISpellGetter> Target;
        public List<ChangeSPELMGEF>? Change;
        public List<ChangeSPELMGEF>? Add;
    }
    public struct ENCHTweak {
        public FormLink<IObjectEffectGetter> Target;
        public List<ChangeSPELMGEF>? Change;
        public List<ChangeSPELMGEF>? Add;
    }
    public struct MGEFTweak {
        public FormLink<IMagicEffectGetter> Target;
        public IFormLink<IEffectShader>? HitShader;
    }
    public struct FLSTTweak {
        public FormLink<IFormListGetter> Target;
        public List<IFormLinkGetter<ISkyrimMajorRecordGetter>>? Inject;
    }
    public class FileData
    {
        public List<SPELTweak>? SPEL;
        public List<ENCHTweak>? ENCH;
        public List<MGEFTweak>? MGEF;
        public List<FLSTTweak>? FLST;
    }
}