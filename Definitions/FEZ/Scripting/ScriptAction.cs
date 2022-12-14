using FEZRepacker.XNB.Attributes;

namespace FEZEngine.Structure.Scripting
{
    [XNBType("FezEngine.Readers.ScriptActionReader")]
    class ScriptAction
    {
        [XNBProperty(UseConverter = true)]
        public Entity Object { get; set; }

        [XNBProperty]
        public string Operation { get; set; }

        [XNBProperty(UseConverter = true)]
        public string[] Arguments { get; set; }

        [XNBProperty]
        public bool Killswitch { get; set; } // whether action should kill this script

        [XNBProperty]
        public bool Blocking { get; set; } // should action be blocked from execution if another one is executing already


        public ScriptAction()
        {
            Object = new();
            Operation = "";
            Arguments = new string[0];
        }
    }
}
