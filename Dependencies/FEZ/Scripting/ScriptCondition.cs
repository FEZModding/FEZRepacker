namespace FEZEngine.Structure.Scripting
{
    class ScriptCondition
    {
        public Entity Object { get; set; }
        public ComparisonOperator Operator { get; set; }
        public string Property { get; set; }
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
