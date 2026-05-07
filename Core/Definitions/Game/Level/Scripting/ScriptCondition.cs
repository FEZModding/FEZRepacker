namespace FEZRepacker.Core.Definitions.Game.Level.Scripting
{
    [XnbType("FezEngine.Structure.Scripting.ScriptCondition, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.ScriptConditionReader, FezEngine")]
    public class ScriptCondition
    {
        [XnbProperty(UseConverter = true)]
        public Entity Object { get; set; } = new();

        [XnbProperty(UseConverter = true)]
        public ComparisonOperator Operator { get; set; } = ComparisonOperator.None;

        [XnbProperty]
        public string Property { get; set; } = "";

        [XnbProperty]
        public string Value { get; set; } = "";
    }
}
