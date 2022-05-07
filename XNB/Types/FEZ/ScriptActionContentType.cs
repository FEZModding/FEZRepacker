using FEZEngine.Structure.Scripting;

namespace FEZRepacker.XNB.Types.FEZ
{
    class ScriptActionContentType : XNBContentType<ScriptAction>
    {
        public ScriptActionContentType(XNBContentConverter converter) : base(converter) { }
        public override TypeAssemblyQualifier Name => "FezEngine.Readers.ScriptActionReader";

        public override object Read(BinaryReader reader)
        {
            ScriptAction action = new ScriptAction();

            action.Object = Converter.ReadType<Entity>(reader) ?? action.Object;

            action.Operation = reader.ReadString();
            action.Arguments = Converter.ReadType<string[]>(reader) ?? action.Arguments;
            action.Killswitch = reader.ReadBoolean();
            action.Blocking = reader.ReadBoolean();

            return action;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            ScriptAction action = (ScriptAction)data;

            Converter.WriteType(action.Object, writer);
            writer.Write(action.Operation);
            Converter.WriteType(action.Arguments, writer);
            writer.Write(action.Killswitch);
            writer.Write(action.Blocking);
        }
    }
}