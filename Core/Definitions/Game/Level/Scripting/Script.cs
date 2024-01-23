namespace FEZRepacker.Core.Definitions.Game.Level.Scripting
{
    [XnbType("FezEngine.Structure.Scripting.Script")]
    [XnbReaderType("FezEngine.Readers.ScriptReader")]
    public class Script
    {
        [XnbProperty]
        public string Name { get; set; } = "Untitled";

        [XnbProperty(Optional = true)]
        public TimeSpan? Timeout { get; set; }

        [XnbProperty(UseConverter = true)]
        public List<ScriptTrigger> Triggers { get; set; } = new();

        [XnbProperty(UseConverter = true)]
        public List<ScriptCondition> Conditions { get; set; } = new();

        [XnbProperty(UseConverter = true)]
        public List<ScriptAction> Actions { get; set; } = new();

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
    }
}
