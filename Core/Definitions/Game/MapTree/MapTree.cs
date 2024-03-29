﻿namespace FEZRepacker.Core.Definitions.Game.MapTree
{
    [XnbType("FezEngine.Structure.MapTree")]
    [XnbReaderType("FezEngine.Readers.MapTreeReader")]
    public class MapTree
    {
        [XnbProperty(UseConverter = true)]
        public MapNode Root { get; set; } = new();
    }
}
