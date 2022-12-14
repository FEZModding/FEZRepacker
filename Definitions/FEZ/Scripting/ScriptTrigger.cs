using FEZRepacker.XNB.Attributes;

namespace FEZEngine.Structure.Scripting
{
    [XNBType("FezEngine.Readers.ScriptTriggerReader")]
    class ScriptTrigger
    {
        [XNBProperty(UseConverter = true)]
        public Entity Object { get; set; }

        [XNBProperty]
        public string Event { get; set; }


        public ScriptTrigger()
        {
            Object = new();
            Event = "";
        }
    }
}
