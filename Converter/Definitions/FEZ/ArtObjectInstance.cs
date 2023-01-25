using System.Numerics;

namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Structure.ArtObjectInstance")]
    [XnbReaderType("FezEngine.Readers.ArtObjectInstanceReader")]
    internal class ArtObjectInstance
    {
        [XnbProperty]
        public string Name { get; set; }

        [XnbProperty]
        public Vector3 Position { get; set; }

        [XnbProperty]
        public Quaternion Rotation { get; set; }

        [XnbProperty]
        public Vector3 Scale { get; set; }

        [XnbProperty(UseConverter = true)]
        public ArtObjectActorSettings ActorSettings { get; set; }

        
        public ArtObjectInstance()
        {
            Name = "";
            ActorSettings = new();
        }
    }
}
