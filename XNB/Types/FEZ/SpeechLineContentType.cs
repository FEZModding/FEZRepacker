using FEZEngine;
using FEZEngine.Structure;
using System.Numerics;

namespace FEZRepacker.XNB.Types.FEZ
{
    class SpeechLineContentType : XNBContentType<SpeechLine>
    {
        public SpeechLineContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.SpeechLineReader";

        public override object Read(BinaryReader reader)
        {
            SpeechLine line = new SpeechLine();

            line.Text = Converter.ReadType<string>(reader) ?? "";
            line.OverrideContent = Converter.ReadType<NpcActionContent>(reader) ?? line.OverrideContent;

            return line;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            SpeechLine line = (SpeechLine)data;

            Converter.WriteType(line.Text, writer);
            Converter.WriteType(line.OverrideContent, writer);
        }
    }
}
