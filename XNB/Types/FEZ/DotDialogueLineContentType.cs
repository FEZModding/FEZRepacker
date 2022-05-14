using FEZEngine;
using FEZEngine.Structure;

namespace FEZRepacker.XNB.Types.FEZ
{
    class DotDialogueLineContentType : XNBContentType<DotDialogueLine>
    {
        public DotDialogueLineContentType(XNBContentConverter converter) : base(converter){}
        public override FEZAssemblyQualifier Name => "FezEngine.Readers.DotDialogueLineReader";

        public override object Read(BinaryReader reader)
        {
            DotDialogueLine line = new DotDialogueLine();
            line.ResourceText = Converter.ReadType<string>(reader);
            line.Grouped = reader.ReadBoolean();

            return line;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            DotDialogueLine line = (DotDialogueLine)data;

            Converter.WriteType(line.ResourceText, writer);
            writer.Write(line.Grouped);
        }
    }
}
