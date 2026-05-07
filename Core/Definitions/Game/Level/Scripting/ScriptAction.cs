namespace FEZRepacker.Core.Definitions.Game.Level.Scripting
{
    [XnbType("FezEngine.Structure.Scripting.ScriptAction, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.ScriptActionReader, FezEngine")]
    public class ScriptAction
    {
        [XnbProperty(UseConverter = true)]
        public Entity Object { get; set; } = new();

        [XnbProperty]
        public string Operation { get; set; } = "";

        [XnbProperty(UseConverter = true)]
        public string[] Arguments { get; set; } = { };

        [XnbProperty]
        public bool Killswitch { get; set; } // whether action should kill this script

        [XnbProperty]
        public bool Blocking { get; set; } // should action be blocked from execution if another one is executing already
    }
}
