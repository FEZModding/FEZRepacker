using FEZEngine.Structure;
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

            level.Name = _converter.ReadType<string>(reader);
            level.Size = new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
            );
            level.StartingFace = _converter.ReadType<TrileFace>(reader);
            level.SequenceSamplesPath = _converter.ReadType<string>(reader);

            level.Flat = reader.ReadBoolean();
            level.SkipPostProcess = reader.ReadBoolean();

            level.BaseDiffuse = reader.ReadSingle();
            level.BaseAmbient = reader.ReadSingle();
            level.GomezHaloName = _converter.ReadType<string>(reader);
            level.HaloFiltering = reader.ReadBoolean();
            level.BlinkingAlpha = reader.ReadBoolean();
            level.Loops = reader.ReadBoolean();

            return level;
        }

        public override void Write(object data, BinaryWriter writer)
        {

        }
    }
}
