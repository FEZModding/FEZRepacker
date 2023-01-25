namespace FEZRepacker.Converter.Definitions.FezEngine.Structure.Scripting
{
    [XnbType("FezEngine.Structure.Scripting.ScriptCondition")]
    [XnbReaderType("FezEngine.Readers.ScriptConditionReader")]
    class ScriptCondition
    {
        [XnbProperty(UseConverter = true)]
        public Entity Object { get; set; }

        [XnbProperty(UseConverter = true)]
        public ComparisonOperator Operator { get; set; }

        [XnbProperty]
        public string Property { get; set; }

        [XnbProperty]
        public string Value { get; set; }

        
        public ScriptCondition()
        {
            Object = new();
            Operator = ComparisonOperator.None;
            Property = "";
            Value = "";
        }
    }
}
