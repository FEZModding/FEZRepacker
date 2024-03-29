﻿using System.Text.Json.Serialization;

using FEZRepacker.Core.Definitions.Game.Common;
using FEZRepacker.Core.Definitions.Game.Graphics;
using FEZRepacker.Core.Definitions.Game.XNA;


namespace FEZRepacker.Core.Definitions.Game.ArtObject
{
    [XnbType("FezEngine.Structure.ArtObject")]
    [XnbReaderType("FezEngine.Readers.ArtObjectReader")]
    public class ArtObject
    {
        [XnbProperty]
        public string Name { get; set; } = "";

        [JsonIgnore]
        [XnbProperty(UseConverter = true)]
        public Texture2D Cubemap { get; set; } = new();

        [XnbProperty]
        public Vector3 Size { get; set; }

        [JsonIgnore]
        [XnbProperty(UseConverter = true)]
        public IndexedPrimitives<VertexInstance, Matrix> Geometry { get; set; } = new();

        [XnbProperty(UseConverter = true)]
        public ActorType ActorType { get; set; }

        [XnbProperty]
        public bool NoSihouette { get; set; }
    }
}
