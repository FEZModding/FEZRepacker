namespace FEZRepacker.Core.Definitions.Game.Level.Scripting
{
    [XnbType("FezEngine.Structure.Scripting.ScriptAction")]
    [XnbReaderType("FezEngine.Readers.ScriptActionReader")]
    class ScriptAction
    {
        [XnbProperty(UseConverter = true)]
        public Entity Object { get; set; }

        [XnbProperty]
        public string Operation { get; set; }

        [XnbProperty(UseConverter = true)]
        public string[] Arguments { get; set; }

        [XnbProperty]
        public bool Killswitch { get; set; } // whether action should kill this script

        [XnbProperty]
        public bool Blocking { get; set; } // should action be blocked from execution if another one is executing already


        public ScriptAction()
        {
            Object = new();
            Operation = "";
            Arguments = new string[0];
        }
    }
}
