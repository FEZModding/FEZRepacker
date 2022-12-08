using FEZEngine.Structure.Scripting;

namespace FEZRepacker.XNB.Types.FEZ
{
    class ScriptContentType : XNBContentType<Script>
    {
        public ScriptContentType(XNBContentConverter converter) : base(converter) { }
        public override FEZAssemblyQualifier Name => "FezEngine.Readers.ScriptReader";

        public override object Read(BinaryReader reader)
        {
            Script script = new Script();

            script.Name = reader.ReadString();
            if (reader.ReadBoolean()) script.Timeout = new TimeSpan(reader.ReadInt64());
            script.Triggers = Converter.ReadType<List<ScriptTrigger>>(reader) ?? script.Triggers;
            script.Conditions = Converter.ReadType<List<ScriptCondition>>(reader) ?? script.Conditions;
            script.Actions = Converter.ReadType<List<ScriptAction>>(reader) ?? script.Actions;
            script.OneTime = reader.ReadBoolean();
            script.Triggerless = reader.ReadBoolean();
            script.IgnoreEndTriggers = reader.ReadBoolean();
            script.LevelWideOneTime = reader.ReadBoolean();
            script.Disabled = reader.ReadBoolean();
            script.IsWinCondition = reader.ReadBoolean();

            return script;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            Script script = (Script)data;

            writer.Write(script.Name);
            writer.Write(script.Timeout.HasValue);
            if(script.Timeout.HasValue) writer.Write(script.Timeout.GetValueOrDefault().Ticks);
            Converter.WriteType(script.Triggers, writer);
            Converter.WriteType(script.Conditions, writer);
            Converter.WriteType(script.Actions, writer);
            writer.Write(script.OneTime);
            writer.Write(script.Triggerless);
            writer.Write(script.IgnoreEndTriggers);
            writer.Write(script.LevelWideOneTime);
            writer.Write(script.Disabled);
            writer.Write(script.IsWinCondition);
        }
    }
}