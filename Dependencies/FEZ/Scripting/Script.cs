namespace FEZEngine.Structure.Scripting
{
    class Script
    {
        public string Name { get; set; }
        public TimeSpan? Timeout { get; set; }
        public List<ScriptTrigger> Triggers { get; set; }
        public List<ScriptCondition> Conditions { get; set; }
        public List<ScriptAction> Actions { get; set; }
        public bool OneTime { get; set; }
        public bool Triggerless { get; set; }
        public bool IgnoreEndTriggers { get; set; }
        public bool LevelWideOneTime { get; set; }
        public bool Disabled { get; set; }
        public bool IsWinCondition { get; set; }

        public Script()
        {
            Name = "Untitled";
            Triggers = new();
            Conditions = new();
            Actions = new();
        }
    }
}
