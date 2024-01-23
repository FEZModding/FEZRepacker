﻿using FEZRepacker.Core.Definitions.Game.Common;

namespace FEZRepacker.Core.Definitions.Game.MapTree
{
    [XnbType("FezEngine.Structure.MapNode+Connection")]
    [XnbReaderType("FezEngine.Readers.MapNodeConnectionReader")]
    public class MapNodeConnection
    {
        [XnbProperty(UseConverter = true)]
        public FaceOrientation Face { get; set; }

        [XnbProperty(UseConverter = true)]
        public MapNode Node { get; set; }

        [XnbProperty]
        public float BranchOversize { get;set; }


        public MapNodeConnection()
        {
            Node = new MapNode();
        }
    }
}
