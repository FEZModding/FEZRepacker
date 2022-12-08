using FEZEngine;
using FEZEngine.Structure;
using System.Numerics;

namespace FEZRepacker.XNB.Types.FEZ
{
    class NpcActionContentContentType : XNBContentType<NpcActionContent>
    {
        public NpcActionContentContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.NpcActionContentReader";

        public override object Read(BinaryReader reader)
        {
            NpcActionContent content = new NpcActionContent();

            content.AnimationName = Converter.ReadType<string>(reader) ?? "";
            content.SoundName = Converter.ReadType<string>(reader) ?? "";

            return content;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            NpcActionContent content = (NpcActionContent)data;
            Converter.WriteType(content.AnimationName, writer);
            Converter.WriteType(content.SoundName, writer);
        }
    }
}
