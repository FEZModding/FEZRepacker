namespace FEZEngine.Structure.Scripting
{
    class ScriptAction
    {
        public Entity Object { get; set; }
        public string Operation { get; set; }
        public string[] Arguments { get; set; }
        public bool Killswitch { get; set; } // whether action should kill this script
        public bool Blocking { get; set; } // should action be blocked from execution if another one is executing already


        public ScriptAction()
        {
            Object = new();
            Operation = "";
            Arguments = new string[0];
        }
    }
}
