using FEZEngine.Structure;

namespace FEZRepacker.XNB.Types.FEZ
{
    class MovementPathContentType : XNBContentType<MovementPath>
    {
        public MovementPathContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.MovementPathReader";

        public override object Read(BinaryReader reader)
        {
            MovementPath path = new MovementPath();

            path.Segments = Converter.ReadType<List<PathSegment>>(reader) ?? path.Segments;
            path.NeedsTrigger = reader.ReadBoolean();
            path.EndBehavior = Converter.ReadType<PathEndBehavior>(reader);
            path.SoundName = Converter.ReadType<string>(reader) ?? path.SoundName;
            path.IsSpline = reader.ReadBoolean();
            path.OffsetSeconds = reader.ReadSingle();
            path.SaveTrigger = reader.ReadBoolean();

            return path;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            MovementPath path = (MovementPath)data;

            Converter.WriteType(path.Segments, writer);
            writer.Write(path.NeedsTrigger);
            Converter.WriteType(path.EndBehavior, writer);
            Converter.WriteType(path.SoundName, writer);
            writer.Write(path.IsSpline);
            writer.Write(path.OffsetSeconds);
            writer.Write(path.SaveTrigger);

        }
    }
}
