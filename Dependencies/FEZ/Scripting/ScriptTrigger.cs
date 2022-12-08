namespace FEZEngine.Structure.Scripting
{
    class ScriptTrigger
    {
        public Entity Object { get; set; }
        public string Event { get; set; }


        public ScriptTrigger()
        {
            Object = new();
            Event = "";
        }
    }
}
