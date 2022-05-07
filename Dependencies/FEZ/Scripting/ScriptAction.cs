namespace FEZEngine.Structure.Scripting
{
    class ScriptAction
    {
        public Entity Object = new Entity();
        public string Operation = "";
        public string[] Arguments = new string[0];
        public bool Killswitch;
        public bool Blocking;
    }
}
