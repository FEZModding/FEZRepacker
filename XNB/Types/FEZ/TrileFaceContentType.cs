using FEZEngine;
using FEZEngine.Structure;

namespace FEZRepacker.XNB.Types.FEZ
{
    class TrileFaceContentType : XNBContentType<TrileFace>
    {
        public TrileFaceContentType(XNBContentConverter converter) : base(converter){}
        public override FEZAssemblyQualifier Name => "FezEngine.Readers.TrileFaceReader";

        public override object Read(BinaryReader reader)
        {
            TrileFace face = new TrileFace();
            face.Id = Converter.ReadType<TrileEmplacement>(reader);
            face.Face = Converter.ReadType<FaceOrientation>(reader);

            return face;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            TrileFace face = (TrileFace)data;

            Converter.WriteType(face.Id, writer);
            Converter.WriteType(face.Face, writer);
        }
    }
}
