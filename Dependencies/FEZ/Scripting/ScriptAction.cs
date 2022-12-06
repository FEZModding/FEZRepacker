namespace FEZEngine.Structure.Scripting
{
    class ScriptAction
    {
        public Entity Object = new Entity();
        public string Operation = "";
        public string[] Arguments = new string[0];
        public bool Killswitch; // whether action should kill this script
        public bool Blocking; // should action be blocked from execution if another one is executing already
    }
}
