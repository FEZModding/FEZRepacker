
using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.Level;
using FEZRepacker.Core.Definitions.Game.Level.Scripting;
using FEZRepacker.Core.XNB.ContentSerialization;
using FEZRepacker.Core.XNB.ContentSerialization.System;

namespace FEZRepacker.Core.XNB.ContentTypes
{
    internal class LevelContentIdentity : XnbPrimaryContentIdentity
    {
        protected override List<XnbContentSerializer> ContentTypesFactory => new()
        {
            new GenericContentSerializer<Level>(),
            new StringContentSerializer(),
            new GenericContentSerializer<TrileFace>(),
            new GenericContentSerializer<TrileEmplacement>(),
            new EnumContentSerializer<FaceOrientation>(),
            new Int32ContentSerializer(),
            new BooleanContentSerializer(),
            new EnumContentSerializer<LiquidType>(),
            new DictionaryContentSerializer<int, Volume>(),
            new GenericContentSerializer<Volume>(),
            new ArrayContentSerializer<FaceOrientation>(),
            new GenericContentSerializer<VolumeActorSettings>(),
            new ListContentSerializer<DotDialogueLine>(),
            new GenericContentSerializer<DotDialogueLine>(),
            new ArrayContentSerializer<CodeInput>(),
            new EnumContentSerializer<CodeInput>(),
            new DictionaryContentSerializer<int, Script>(),
            new GenericContentSerializer<Script>(),
            new ListContentSerializer<ScriptTrigger>(),
            new GenericContentSerializer<ScriptTrigger>(),
            new GenericContentSerializer<Entity>(),
            new ListContentSerializer<ScriptCondition>(),
            new GenericContentSerializer<ScriptCondition>(),
            new EnumContentSerializer<ComparisonOperator>(),
            new ListContentSerializer<ScriptAction>(),
            new GenericContentSerializer<ScriptAction>(),
            new ArrayContentSerializer<string>(false),
            new DictionaryContentSerializer<TrileEmplacement, TrileInstance>(true, false),
            new GenericContentSerializer<TrileInstance>(),
            new GenericContentSerializer<TrileInstanceActorSettings>(),
            new ListContentSerializer<TrileInstance>(),
            new ArrayContentSerializer<bool>(true),
            new DictionaryContentSerializer<int, ArtObjectInstance>(),
            new GenericContentSerializer<ArtObjectInstance>(),
            new GenericContentSerializer<ArtObjectActorSettings>(),
            new EnumContentSerializer<ActorType>(),
            new EnumContentSerializer<Viewpoint>(),
            new ArrayContentSerializer<VibrationMotor>(),
            new EnumContentSerializer<VibrationMotor>(),
            new GenericContentSerializer<PathSegment>(),
            new TimeSpanContentSerializer(),
            new GenericContentSerializer<CameraNodeData>(),
            new DictionaryContentSerializer<int, BackgroundPlane>(),
            new GenericContentSerializer<BackgroundPlane>(),
            new DictionaryContentSerializer<int, TrileGroup>(),
            new GenericContentSerializer<TrileGroup>(),
            new GenericContentSerializer<MovementPath>(),
            new ListContentSerializer<PathSegment>(),
            new EnumContentSerializer<PathEndBehavior>(),
            new GenericContentSerializer<SpeechLine>(),
            new DictionaryContentSerializer<int, NpcInstance>(),
            new GenericContentSerializer<NpcInstance>(),
            new GenericContentSerializer<NpcActionContent>(),
            new ListContentSerializer<SpeechLine>(),
            new DictionaryContentSerializer<NpcAction, NpcActionContent>(true, false),
            new EnumContentSerializer<NpcAction>(),
            new DictionaryContentSerializer<int, MovementPath>(),
            new ListContentSerializer<string>(),
            new ListContentSerializer<AmbienceTrack>(),
            new GenericContentSerializer<AmbienceTrack>(),
            new EnumContentSerializer<LevelNodeType>(),
        };
    }
}
