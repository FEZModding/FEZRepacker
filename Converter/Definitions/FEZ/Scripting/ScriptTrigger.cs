namespace FEZRepacker.Converter.Definitions.FezEngine.Structure.Scripting
{
    [XnbType("FezEngine.Structure.Scripting.ScriptTrigger")]
    [XnbReaderType("FezEngine.Readers.ScriptTriggerReader")]
    class ScriptTrigger
    {
        [XnbProperty(UseConverter = true)]
        public Entity Object { get; set; }

        [XnbProperty]
        public string Event { get; set; }


        public ScriptTrigger()
        {
            Object = new();
            Event = "";
        }
    }
}
