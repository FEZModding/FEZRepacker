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
            if (reader.ReadBoolean()) script.OneTime = true;
            if (reader.ReadBoolean()) script.Triggerless = true;
            if (reader.ReadBoolean()) script.IgnoreEndTriggers = true;
            if (reader.ReadBoolean()) script.LevelWideOneTime = true;
            if (reader.ReadBoolean()) script.Disabled = true;
            if (reader.ReadBoolean()) script.IsWinCondition = true;

            return script;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            Script script = (Script)data;

            writer.Write(script.Name);
            if (!script.Timeout.HasValue) writer.Write(false);
            else
            {
                writer.Write(true);
                writer.Write(script.Timeout.GetValueOrDefault().Ticks);
            }
            Converter.WriteType(script.Triggers, writer);
            Converter.WriteType(script.Conditions, writer);
            Converter.WriteType(script.Actions, writer);
            writer.Write(script.OneTime.GetValueOrDefault());
            writer.Write(script.Triggerless.GetValueOrDefault());
            writer.Write(script.IgnoreEndTriggers.GetValueOrDefault());
            writer.Write(script.LevelWideOneTime.GetValueOrDefault());
            writer.Write(script.Disabled.GetValueOrDefault());
            writer.Write(script.IsWinCondition.GetValueOrDefault());
        }
    }
}