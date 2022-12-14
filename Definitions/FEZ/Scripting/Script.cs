using FEZRepacker.XNB.Attributes;

namespace FEZEngine.Structure.Scripting
{
    [XNBType("FezEngine.Readers.ScriptReader")]
    class Script
    {
        [XNBProperty]
        public string Name { get; set; }

        [XNBProperty(Optional = true)]
        public TimeSpan? Timeout { get; set; }

        [XNBProperty(UseConverter = true)]
        public List<ScriptTrigger> Triggers { get; set; }

        [XNBProperty(UseConverter = true)]
        public List<ScriptCondition> Conditions { get; set; }

        [XNBProperty(UseConverter = true)]
        public List<ScriptAction> Actions { get; set; }

        [XNBProperty]
        public bool OneTime { get; set; }

        [XNBProperty]
        public bool Triggerless { get; set; }

        [XNBProperty]
        public bool IgnoreEndTriggers { get; set; }

        [XNBProperty]
        public bool LevelWideOneTime { get; set; }

        [XNBProperty]
        public bool Disabled { get; set; }

        [XNBProperty]
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
