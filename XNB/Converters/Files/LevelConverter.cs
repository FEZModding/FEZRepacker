using FEZEngine;
using FEZEngine.Structure;
using FEZEngine.Structure.Input;
using FEZEngine.Structure.Scripting;
using FEZRepacker.XNB.Types;
using FEZRepacker.XNB.Types.FEZ;
using FEZRepacker.XNB.Types.System;

namespace FEZRepacker.XNB.Converters.Files
{
    class LevelConverter : JsonStorageConverter<Level>
    {
        public override XNBContentType[] Types => new XNBContentType[]
        {
            new LevelContentType(this),
            new StringContentType(this),
            new TrileFaceContentType(this),
            new TrileEmplacementContentType(this),
            new EnumContentType<FaceOrientation>(this),
            new Int32ContentType(this),
            new EnumContentType<LiquidType>(this),
            new DictionaryContentType<int, Volume>(this),
            new VolumeContentType(this),
            new ArrayContentType<FaceOrientation>(this),
            new VolumeActorSettingsContentType(this),
            new ListContentType<DotDialogueLine>(this),
            new DotDialogueLineContentType(this),
            new ArrayContentType<CodeInput>(this),
            new EnumContentType<CodeInput>(this),
            new DictionaryContentType<int, Script>(this),
            new ScriptContentType(this),
            new ListContentType<ScriptTrigger>(this),
            new ScriptTriggerContentType(this),
            new EntityContentType(this),
            new ListContentType<ScriptCondition>(this),
            new ScriptConditionContentType(this),
            new EnumContentType<ComparisonOperator>(this),
            new ListContentType<ScriptAction>(this),
            new ScriptActionContentType(this),
            new ArrayContentType<string>(this, false),
            new DictionaryContentType<TrileEmplacement, TrileInstance>(this, true, false),
            new TrileInstanceContentType(this),
            new InstanceActorSettingsContentType(this),
            new ListContentType<TrileInstance>(this),
            new BooleanContentType(this),
            new ArrayContentType<bool>(this, true),
            new DictionaryContentType<int, ArtObjectInstance>(this),
            new ArtObjectInstanceContentType(this),
            new ArtObjectActorSettingsContentType(this),
            new EnumContentType<ActorType>(this),
            new EnumContentType<Viewpoint>(this),
            new ArrayContentType<VibrationMotor>(this),
            new EnumContentType<VibrationMotor>(this),
            new PathSegmentContentType(this),
            new TimeSpanContentType(this),
            new CameraNodeDataContentType(this),
            new DictionaryContentType<int, BackgroundPlane>(this),
            new BackgroundPlaneContentType(this),
            new EnumContentType<ActorType>(this),
        };
        public override string FileFormat => "fezlvl";
    }
}
