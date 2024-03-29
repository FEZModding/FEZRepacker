﻿namespace FEZRepacker.Core.Definitions.Game.Level.Scripting
{
    [XnbType("FezEngine.Structure.Scripting.ScriptTrigger")]
    [XnbReaderType("FezEngine.Readers.ScriptTriggerReader")]
    public class ScriptTrigger
    {
        [XnbProperty(UseConverter = true)]
        public Entity Object { get; set; } = new();

        [XnbProperty]
        public string Event { get; set; } = "";
    }
}
