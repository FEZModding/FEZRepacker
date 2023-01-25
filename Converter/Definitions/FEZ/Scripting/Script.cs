namespace FEZRepacker.Converter.Definitions.FezEngine.Structure.Scripting
{
    [XnbType("FezEngine.Structure.Scripting.Script")]
    [XnbReaderType("FezEngine.Readers.ScriptReader")]
    class Script
    {
        [XnbProperty]
        public string Name { get; set; }

        [XnbProperty(Optional = true)]
        public TimeSpan? Timeout { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<ScriptTrigger> Triggers { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<ScriptCondition> Conditions { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<ScriptAction> Actions { get; set; }

        [XnbProperty]
        public bool OneTime { get; set; }

        [XnbProperty]
        public bool Triggerless { get; set; }

        [XnbProperty]
        public bool IgnoreEndTriggers { get; set; }

        [XnbProperty]
        public bool LevelWideOneTime { get; set; }

        [XnbProperty]
        public bool Disabled { get; set; }

        [XnbProperty]
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
