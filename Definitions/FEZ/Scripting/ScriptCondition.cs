using FEZRepacker.XNB.Attributes;

namespace FEZRepacker.Definitions.FezEngine.Structure.Scripting
{
    [XNBType("FezEngine.Readers.ScriptConditionReader")]
    class ScriptCondition
    {
        [XNBProperty(UseConverter = true)]
        public Entity Object { get; set; }

        [XNBProperty(UseConverter = true)]
        public ComparisonOperator Operator { get; set; }

        [XNBProperty]
        public string Property { get; set; }

        [XNBProperty]
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
