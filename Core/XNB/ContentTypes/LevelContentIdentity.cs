
using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.Level;
using FEZRepacker.Core.Definitions.Game.Level.Scripting;
using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class LevelContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentSerializersFactory => new()
        {
            new LevelContentSerializer(),
            new StringContentSerializer(),
            new TrileFaceContentSerializer(),
            new TrileEmplacementContentSerializer(),
            new EnumContentSerializer<FaceOrientation>(),
            new Int32ContentSerializer(),
            new BooleanContentSerializer(),
            new EnumContentSerializer<LiquidType>(),
            new DictionaryContentSerializer<int, Volume>(),
            new VolumeContentSerializer(),
            new ArrayContentSerializer<FaceOrientation>(),
            new VolumeActorSettingsContentSerializer(),
            new ListContentSerializer<DotDialogueLine>(),
            new DotDialogueLineContentSerializer(),
            new ArrayContentSerializer<CodeInput>(),
            new EnumContentSerializer<CodeInput>(),
            new DictionaryContentSerializer<int, Script>(),
            new ScriptContentSerializer(),
            new ListContentSerializer<ScriptTrigger>(),
            new ScriptTriggerContentSerializer(),
            new EntityContentSerializer(),
            new ListContentSerializer<ScriptCondition>(),
            new ScriptConditionContentSerializer(),
            new EnumContentSerializer<ComparisonOperator>(),
            new ListContentSerializer<ScriptAction>(),
            new ScriptActionContentSerializer(),
            new ArrayContentSerializer<string>(false),
            new DictionaryContentSerializer<TrileEmplacement, TrileInstance>(true, false),
            new TrileInstanceContentSerializer(),
            new TrileInstanceActorSettingsContentSerializer(),
            new ListContentSerializer<TrileInstance>(),
            new ArrayContentSerializer<bool>(true),
            new DictionaryContentSerializer<int, ArtObjectInstance>(),
            new ArtObjectInstanceContentSerializer(),
            new ArtObjectActorSettingsContentSerializer(),
            new EnumContentSerializer<ActorType>(),
            new EnumContentSerializer<Viewpoint>(),
            new ArrayContentSerializer<VibrationMotor>(),
            new EnumContentSerializer<VibrationMotor>(),
            new PathSegmentContentSerializer(),
            new TimeSpanContentSerializer(),
            new CameraNodeDataContentSerializer(),
            new DictionaryContentSerializer<int, BackgroundPlane>(),
            new BackgroundPlaneContentSerializer(),
            new DictionaryContentSerializer<int, TrileGroup>(),
            new TrileGroupContentSerializer(),
            new MovementPathContentSerializer(),
            new ListContentSerializer<PathSegment>(),
            new EnumContentSerializer<PathEndBehavior>(),
            new SpeechLineContentSerializer(),
            new DictionaryContentSerializer<int, NpcInstance>(),
            new NpcInstanceContentSerializer(),
            new NpcActionContentContentSerializer(),
            new ListContentSerializer<SpeechLine>(),
            new DictionaryContentSerializer<NpcAction, NpcActionContent>(true, false),
            new EnumContentSerializer<NpcAction>(),
            new DictionaryContentSerializer<int, MovementPath>(),
            new ListContentSerializer<string>(),
            new ListContentSerializer<AmbienceTrack>(),
            new AmbienceTrackContentSerializer(),
            new EnumContentSerializer<LevelNodeType>(),
        };
    }
}
