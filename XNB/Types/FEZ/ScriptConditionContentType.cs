using FEZEngine.Structure.Scripting;

namespace FEZRepacker.XNB.Types.FEZ
{
    class ScriptConditionContentType : XNBContentType<ScriptCondition>
    {
        public ScriptConditionContentType(XNBContentConverter converter) : base(converter) { }
        public override FEZAssemblyQualifier Name => "FezEngine.Readers.ScriptConditionReader";

        public override object Read(BinaryReader reader)
        {
            ScriptCondition condition = new ScriptCondition();

            condition.Object = Converter.ReadType<Entity>(reader) ?? condition.Object;
            condition.Operator = Converter.ReadType<ComparisonOperator>(reader);
            condition.Property = reader.ReadString();
            condition.Value = reader.ReadString();

            return condition;
        }

        public override void Write(object data, BinaryWriter writer)
        {
            ScriptCondition condition = (ScriptCondition)data;

            Converter.WriteType(condition.Object, writer);
            Converter.WriteType(condition.Operator, writer);
            writer.Write(condition.Property);
            writer.Write(condition.Value);
        }
    }
}