using System.Numerics;

using FEZEngine;
using FEZEngine.Structure;
using FEZEngine.Structure.Input;

namespace FEZRepacker.XNB.Types.FEZ
{
    class PathSegmentContentType : XNBContentType<PathSegment>
    {
        public PathSegmentContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.PathSegmentReader";

        public override object Read(BinaryReader reader)
        {
            PathSegment path = new PathSegment();

            path.Destination.X = reader.ReadSingle();
            path.Destination.Y = reader.ReadSingle();
            path.Destination.Z = reader.ReadSingle();
            path.Duration = Converter.ReadType<TimeSpan>(reader);
            path.WaitTimeOnStart = Converter.ReadType<TimeSpan>(reader);
            path.WaitTimeOnFinish = Converter.ReadType<TimeSpan>(reader);
            path.Acceleration = reader.ReadSingle();
            path.Deceleration = reader.ReadSingle();
            path.JitterFactor = reader.ReadSingle();

            path.Orientation.W = reader.ReadSingle();
            path.Orientation.X = reader.ReadSingle();
            path.Orientation.Y = reader.ReadSingle();
            path.Orientation.Z = reader.ReadSingle();

            if (reader.ReadBoolean())
            {
                path.CustomData = Converter.ReadType<CameraNodeData>(reader);
            }

            return path;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            PathSegment path = (PathSegment)data;

            writer.Write(path.Destination.X);
            writer.Write(path.Destination.Y);
            writer.Write(path.Destination.Z);
            Converter.WriteType(path.Duration, writer);
            Converter.WriteType(path.WaitTimeOnStart, writer);
            Converter.WriteType(path.WaitTimeOnFinish, writer);
            writer.Write(path.Acceleration);
            writer.Write(path.Deceleration);
            writer.Write(path.JitterFactor);
            writer.Write(path.Orientation.W);
            writer.Write(path.Orientation.X);
            writer.Write(path.Orientation.Y);
            writer.Write(path.Orientation.Z);

            writer.Write(path.CustomData != null);
            if(path.CustomData != null)
            {
                Converter.WriteType(path.CustomData, writer);
            }
        }
    }
}
