using FEZEngine;
using FEZEngine.Structure;

namespace FEZRepacker.XNB.Types.FEZ
{
    class TrileFaceContentType : XNBContentType<TrileFace>
    {
        public TrileFaceContentType(XNBContentConverter converter) : base(converter){}
        public override TypeAssemblyQualifier Name => "FezEngine.Readers.TrileFaceReader";

        public override object Read(BinaryReader reader)
        {
            TrileFace face = new TrileFace();
            face.Id = _converter.ReadType<TrileEmplacement>(reader);
            face.Face = _converter.ReadType<FaceOrientation>(reader);

            return face;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            TrileFace face = (TrileFace)data;

            _converter.WriteType(face.Id, writer);
            _converter.WriteType(face.Face, writer);
        }
    }
}
