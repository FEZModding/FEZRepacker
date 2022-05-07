using FEZEngine.Structure;
using FEZEngine.Structure.Scripting;
using System.Numerics;

namespace FEZRepacker.XNB.Types.FEZ
{
    class LevelContentType : XNBContentType<Level>
    {
        public LevelContentType(XNBContentConverter converter) : base(converter) { }

        public override TypeAssemblyQualifier Name => "FezEngine.Readers.LevelReader";

        public override object Read(BinaryReader reader)
        {
            Level level = new Level();

            level.Name = Converter.ReadType<string>(reader);
            level.Size = new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
            );
            level.StartingFace = Converter.ReadType<TrileFace>(reader) ?? level.StartingFace;
            level.SequenceSamplesPath = Converter.ReadType<string>(reader) ?? "";
            level.Flat = reader.ReadBoolean();
            level.SkipPostProcess = reader.ReadBoolean();
            level.BaseDiffuse = reader.ReadSingle();
            level.BaseAmbient = reader.ReadSingle();
            level.GomezHaloName = Converter.ReadType<string>(reader) ?? "";
            level.HaloFiltering = reader.ReadBoolean();
            level.BlinkingAlpha = reader.ReadBoolean();
            level.Loops = reader.ReadBoolean();
            level.WaterType = Converter.ReadType<LiquidType>(reader);
            level.WaterHeight = reader.ReadSingle();
            level.SkyName = reader.ReadString();
            level.TrileSetName = Converter.ReadType<string>(reader) ?? "";
            level.Volumes = Converter.ReadType<Dictionary<int, Volume>>(reader) ?? level.Volumes;
            level.Scripts = Converter.ReadType<Dictionary<int, Script>>(reader) ?? level.Scripts;
            level.SongName = Converter.ReadType<string>(reader) ?? "";
            level.FarAwayPlaceFadeOutStart = reader.ReadInt32();
            level.FarAwayPlaceFadeOutLength = reader.ReadInt32();
            return level;
        }

        public override void Write(object data, BinaryWriter writer)
        {

        }
    }
}
