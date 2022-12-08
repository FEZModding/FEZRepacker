using System.Numerics;

using FEZEngine;
using FEZEngine.Structure;
using FEZEngine.Structure.Input;

namespace FEZRepacker.XNB.Types.FEZ
{
    class CameraNodeDataContentType : XNBContentType<CameraNodeData>
    {
        public CameraNodeDataContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.CameraNodeDataReader";

        public override object Read(BinaryReader reader)
        {
            CameraNodeData data = new CameraNodeData();

            data.Perspective = reader.ReadBoolean();
            data.PixelsPerTrixel = reader.ReadInt32();
            data.SoundName = Converter.ReadType<string>(reader) ?? data.SoundName;

            return data;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            CameraNodeData cameraData = (CameraNodeData)data;

            writer.Write(cameraData.Perspective);
            writer.Write(cameraData.PixelsPerTrixel);
            Converter.WriteType(cameraData.SoundName, writer);
        }
    }
}
