using FEZEngine.Structure;
using FEZRepacker.Dependencies;

namespace FEZRepacker.XNB.Types.FEZ
{
    class PathSegmentContentType : XNBContentType<PathSegment>
    {
        public PathSegmentContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.PathSegmentReader";

        public override object Read(BinaryReader reader)
        {
            PathSegment path = new PathSegment();

            path.Destination = reader.ReadVector3();
            path.Duration = Converter.ReadType<TimeSpan>(reader);
            path.WaitTimeOnStart = Converter.ReadType<TimeSpan>(reader);
            path.WaitTimeOnFinish = Converter.ReadType<TimeSpan>(reader);
            path.Acceleration = reader.ReadSingle();
            path.Deceleration = reader.ReadSingle();
            path.JitterFactor = reader.ReadSingle();

            path.Orientation = reader.ReadQuaternion();

            if (reader.ReadBoolean()) path.CustomData = Converter.ReadType<CameraNodeData>(reader);

            return path;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            PathSegment path = (PathSegment)data;

            writer.Write(path.Destination);
            Converter.WriteType(path.Duration, writer);
            Converter.WriteType(path.WaitTimeOnStart, writer);
            Converter.WriteType(path.WaitTimeOnFinish, writer);
            writer.Write(path.Acceleration);
            writer.Write(path.Deceleration);
            writer.Write(path.JitterFactor);
            writer.Write(path.Orientation);

            writer.Write(path.CustomData != null);
            if(path.CustomData != null) Converter.WriteType(path.CustomData, writer);
            
        }
    }
}
