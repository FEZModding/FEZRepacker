namespace FEZEngine.Structure.Scripting
{
    class Script
    {
        public string Name = "";
        public TimeSpan? Timeout;
        public List<ScriptTrigger> Triggers = new();
        public List<ScriptCondition> Conditions = new();
        public List<ScriptAction> Actions = new();
        public bool? OneTime;
        public bool? Triggerless;
        public bool? IgnoreEndTriggers;
        public bool? LevelWideOneTime;
        public bool? Disabled;
        public bool? IsWinCondition;
    }
}
