using FEZRepacker.Definitions.FezEngine;
using FEZRepacker.Definitions.FezEngine.Structure;
using FEZRepacker.Definitions.FezEngine.Structure.Input;
using FEZRepacker.Definitions.FezEngine.Structure.Scripting;
using FEZRepacker.Conversion.Json;
using FEZRepacker.Conversion.Json.CustomStructures;
using FEZRepacker.XNB.Types;
using FEZRepacker.XNB.Types.System;
using System.Text;

namespace FEZRepacker.XNB.Converters.Files
{
    class LevelConverter : XNBContentConverter
    {
        public override XNBContentType[] TypesFactory => new XNBContentType[]
        {
            //new LevelContentType(this),
            new GenericContentType<Level>(this),
            new StringContentType(this),
            new GenericContentType<TrileFace>(this),
            new GenericContentType<TrileEmplacement>(this),
            new EnumContentType<FaceOrientation>(this),
            new Int32ContentType(this),
            new BooleanContentType(this),
            new EnumContentType<LiquidType>(this),
            new DictionaryContentType<int, Volume>(this),
            new GenericContentType<Volume>(this),
            new ArrayContentType<FaceOrientation>(this),
            new GenericContentType<VolumeActorSettings>(this),
            new ListContentType<DotDialogueLine>(this),
            new GenericContentType<DotDialogueLine>(this),
            new ArrayContentType<CodeInput>(this),
            new EnumContentType<CodeInput>(this),
            new DictionaryContentType<int, Script>(this),
            new GenericContentType<Script>(this),
            new ListContentType<ScriptTrigger>(this),
            new GenericContentType<ScriptTrigger>(this),
            new GenericContentType<Entity>(this),
            new ListContentType<ScriptCondition>(this),
            new GenericContentType<ScriptCondition>(this),
            new EnumContentType<ComparisonOperator>(this),
            new ListContentType<ScriptAction>(this),
            new GenericContentType<ScriptAction>(this),
            new ArrayContentType<string>(this, false),
            new DictionaryContentType<TrileEmplacement, TrileInstance>(this, true, false),
            new GenericContentType<TrileInstance>(this),
            new GenericContentType<TrileInstanceActorSettings>(this),
            new ListContentType<TrileInstance>(this),
            new ArrayContentType<bool>(this, true),
            new DictionaryContentType<int, ArtObjectInstance>(this),
            new GenericContentType<ArtObjectInstance>(this),
            new GenericContentType<ArtObjectActorSettings>(this),
            new EnumContentType<ActorType>(this),
            new EnumContentType<Viewpoint>(this),
            new ArrayContentType<VibrationMotor>(this),
            new EnumContentType<VibrationMotor>(this),
            new GenericContentType<PathSegment>(this),
            new TimeSpanContentType(this),
            new GenericContentType<CameraNodeData>(this),
            new DictionaryContentType<int, BackgroundPlane>(this),
            new GenericContentType<BackgroundPlane>(this),
            new DictionaryContentType<int, TrileGroup>(this),
            new GenericContentType<TrileGroup>(this),
            new GenericContentType<MovementPath>(this),
            new ListContentType<PathSegment>(this),
            new EnumContentType<PathEndBehavior>(this),
            new GenericContentType<SpeechLine>(this),
            new DictionaryContentType<int, NpcInstance>(this),
            new GenericContentType<NpcInstance>(this),
            new GenericContentType<NpcActionContent>(this),
            new ListContentType<SpeechLine>(this),
            new DictionaryContentType<NpcAction, NpcActionContent>(this, true, false),
            new EnumContentType<NpcAction>(this),
            new DictionaryContentType<int, MovementPath>(this),
            new ListContentType<string>(this),
            new ListContentType<AmbienceTrack>(this),
            new GenericContentType<AmbienceTrack>(this),
            new EnumContentType<LevelNodeType>(this),
        };
        public override string FileFormat => "fezlvl";

        public override void FromBinary(BinaryReader xnbReader, BinaryWriter outWriter)
        {
            XNBContentType primaryType = Types[0];

            Level data = (Level)primaryType.Read(xnbReader);

            ModifiedLevel level = new ModifiedLevel(data);

            var json = CustomJsonSerializer.Serialize(level);

            outWriter.Write(Encoding.UTF8.GetBytes(json));

        }

        public override void ToBinary(BinaryReader inReader, BinaryWriter xnbWriter)
        {
            string json = new string(inReader.ReadChars((int)inReader.BaseStream.Length));
            ModifiedLevel level = CustomJsonSerializer.Deserialize<ModifiedLevel>(json);

            Level data = level.ToOriginal();

            PrimaryType.Write(data, xnbWriter);
        }
    }
}
