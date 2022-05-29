using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Synthesis;

using Noggog;

using Newtonsoft.Json.Linq;

using SynTweakEngine.Structs;

namespace SynTweakEngine
{
    class Program
    {
        public static Lazy<FileData> LazySettings = new();
        public static FileData data => LazySettings.Value;
        public static JsonMergeSettings merge = new() { MergeArrayHandling = MergeArrayHandling.Union, MergeNullValueHandling = MergeNullValueHandling.Merge };
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings<FileData>("User Tweaks", "User_TWEAKS.json", out LazySettings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "SynTweakEngine.esp")
                .Run(args);
        }
        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var files = Directory.GetFiles(state.DataFolderPath).Where(x => x.EndsWith("_TWEAKS.json"));
            var JObj = JObject.FromObject(data);
            files.ForEach(f =>
            {
                JObj.Merge(JObject.Parse(File.ReadAllText(Path.Combine(state.DataFolderPath, f))), merge);
            });
            var fdata = JObj.ToObject<FileData>();
            if (fdata != null)
            {
                if (fdata.FLST != null)
                {
                    foreach (var itm in fdata.FLST)
                    {
                        var Target = itm.Target.Resolve<IFormListGetter>(state.LinkCache);
                        if (Target != null)
                        {
                            if (itm.Inject != null)
                            {
                                var tgt = state.PatchMod.FormLists.GetOrAddAsOverride(Target);
                                foreach (var Form in itm.Inject)
                                {
                                    tgt.Items.Add(Form.Resolve<ISkyrimMajorRecordGetter>(state.LinkCache));
                                }
                            }
                        }
                    }
                }
                if (fdata.MGEF != null)
                {
                    foreach (var itm in fdata.MGEF)
                    {
                        var Target = itm.Target.Resolve<IMagicEffectGetter>(state.LinkCache);
                        if (Target != null)
                        {
                            if (itm.HitShader != null)
                            {
                                var tgt = state.PatchMod.MagicEffects.GetOrAddAsOverride(Target);
                                tgt.HitShader.SetTo(itm.HitShader.Resolve<IEffectShaderGetter>(state.LinkCache).FormKey);
                            }
                        }
                    }
                }
                //Spell Handlers
                if (fdata.SPEL != null)
                {
                    foreach (var itm in fdata.SPEL)
                    {
                        var Target = itm.Target.Resolve<ISpellGetter>(state.LinkCache);
                        if (Target != null)
                        {
                            if (itm.Add != null || itm.Change != null)
                            {
                                var tgt = state.PatchMod.Spells.GetOrAddAsOverride(Target);
                                if (itm.ClearDescription)
                                {
                                    tgt.Description.Set(Language.English, null);
                                }
                                //Change Existing Effects
                                if (itm.Change != null)
                                {
                                    foreach (var change in itm.Change)
                                    {
                                        //Change Effect
                                        if (!change.SetTo.IsNull)
                                        {
                                            tgt.Effects[change.Position].BaseEffect.SetTo(change.SetTo);
                                        }
                                        //Area
                                        if (change.Area > 0)
                                        {
                                            tgt.Effects[change.Position].Data!.Area = change.Area;
                                        }
                                        //Duration
                                        if (change.Duration > 0)
                                        {
                                            tgt.Effects[change.Position].Data!.Duration = change.Duration;
                                        }
                                        //Mag
                                        if (change.Mag > 0)
                                        {
                                            tgt.Effects[change.Position].Data!.Magnitude = change.Mag;
                                        }
                                    }
                                }
                                if (itm.Add != null)
                                {
                                    foreach (var add in itm.Add)
                                    {
                                        if (!add.SetTo.IsNull)
                                        {
                                            tgt.Effects.Add(new Effect()
                                            {
                                                BaseEffect = add.SetTo.Resolve<IMagicEffectGetter>(state.LinkCache).AsNullableLink(),
                                                Data = new()
                                                {
                                                    Area = add.Area,
                                                    Magnitude = add.Mag,
                                                    Duration = add.Duration
                                                }
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //Object Effects
                if (fdata.ENCH != null)
                {
                    foreach (var itm in fdata.ENCH)
                    {
                        var Target = itm.Target.Resolve<IObjectEffectGetter>(state.LinkCache);
                        if (Target != null)
                        {
                            if (itm.Add != null || itm.Change != null)
                            {
                                var tgt = state.PatchMod.ObjectEffects.GetOrAddAsOverride(Target);
                                //Change Existing Effects
                                if (itm.Change != null)
                                {
                                    foreach (var change in itm.Change)
                                    {
                                        //Duration
                                        if (change.Duration > 0)
                                        {
                                            tgt.Effects[change.Position].Data!.Duration = change.Duration;
                                        }
                                        //Mag
                                        if (change.Mag > 0)
                                        {
                                            tgt.Effects[change.Position].Data!.Magnitude = change.Mag;
                                        }
                                    }
                                }
                                if (itm.Add != null)
                                {
                                    foreach (var add in itm.Add)
                                    {
                                        if (!add.SetTo.IsNull)
                                        {
                                            tgt.Effects.Add(new Effect()
                                            {
                                                BaseEffect = add.SetTo.Resolve<IMagicEffectGetter>(state.LinkCache).AsNullableLink(),
                                                Data = new()
                                                {
                                                    Area = add.Area,
                                                    Magnitude = add.Mag,
                                                    Duration = add.Duration
                                                }
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}