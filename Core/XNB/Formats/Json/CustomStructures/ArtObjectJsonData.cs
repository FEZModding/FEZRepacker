using System.Numerics;

using FEZRepacker.Converter.Definitions.FezEngine.Structure;

namespace FEZRepacker.Converter.XNB.Formats.Json.CustomStructures
{
    internal class ArtObjectJsonData
    {
        public string Name { get; set; }
        public Vector3 Size { get; set; }
        public ActorType ActorType { get; set; }
        public bool NoSihouette { get; set; }

        public static ArtObjectJsonData FromArtObject(ArtObject ao)
        {
            var data = new ArtObjectJsonData();
            data.Name = ao.Name;
            data.Size = ao.Size;
            data.ActorType = ao.ActorType;
            data.NoSihouette = ao.NoSihouette;
            return data;
        }

        public ArtObject ToArtObject()
        {
            var ao = new ArtObject();
            ao.Name = Name;
            ao.Size = Size;
            ao.ActorType = ActorType;
            ao.NoSihouette = NoSihouette;
            return ao;
        }
    }
}
