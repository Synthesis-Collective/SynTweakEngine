using System.Collections.Generic;

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace SynTweakEngine.Structs
{
    public struct ChangeSPELMGEF {
        public int Position;
        public float Mag;
        public int Duration;
    }
    public struct AddSPELMGEF {
        public FormLink<IMagicEffectGetter> Add;
        public float Mag;
        public int Area;
        public int Duration;
    }
    public struct SPELTweak {
        public FormLink<ISpellGetter> Target;
        public List<ChangeSPELMGEF>? Change;
        public List<AddSPELMGEF>? Add;
    }
    public struct ENCHTweak {
        public FormLink<IObjectEffectGetter> Target;
        public List<ChangeSPELMGEF>? Change;
        public List<AddSPELMGEF>? Add;
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