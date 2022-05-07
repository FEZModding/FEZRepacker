namespace FEZEngine.Structure.Scripting
{
    class ScriptCondition
    {
        public Entity Object = new Entity();
        public ComparisonOperator Operator;
        public string Property = "";
        public string Value = "";
    }
}
