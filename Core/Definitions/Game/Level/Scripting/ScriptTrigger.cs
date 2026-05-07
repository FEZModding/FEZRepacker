namespace FEZRepacker.Core.Definitions.Game.Level.Scripting
{
    [XnbType("FezEngine.Structure.Scripting.ScriptTrigger, FezEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    [XnbReaderType("FezEngine.Readers.ScriptTriggerReader, FezEngine")]
    public class ScriptTrigger
    {
        [XnbProperty(UseConverter = true)]
        public Entity Object { get; set; } = new();

        [XnbProperty]
        public string Event { get; set; } = "";
    }
}
