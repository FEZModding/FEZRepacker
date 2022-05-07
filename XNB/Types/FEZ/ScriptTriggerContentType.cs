using FEZEngine.Structure.Scripting;

namespace FEZRepacker.XNB.Types.FEZ
{
    class ScriptTriggerContentType : XNBContentType<ScriptTrigger>
    {
        public ScriptTriggerContentType(XNBContentConverter converter) : base(converter) { }
        public override TypeAssemblyQualifier Name => "FezEngine.Readers.ScriptTriggerReader";

        public override object Read(BinaryReader reader)
        {
            ScriptTrigger trigger = new ScriptTrigger();

            trigger.Object = Converter.ReadType<Entity>(reader) ?? trigger.Object;
            trigger.Event = reader.ReadString();

            return trigger;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            ScriptTrigger trigger = (ScriptTrigger)data;

            Converter.WriteType(trigger.Object, writer);
            writer.Write(trigger.Event);
        }
    }
}