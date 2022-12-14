using FEZRepacker.XNB.Attributes;
using System.Numerics;

namespace FEZEngine.Structure
{
    [XNBType("FezEngine.Readers.ArtObjectInstanceReader")]
    class ArtObjectInstance
    {
        [XNBProperty]
        public string Name { get; set; }

        [XNBProperty]
        public Vector3 Position { get; set; }

        [XNBProperty]
        public Quaternion Rotation { get; set; }

        [XNBProperty]
        public Vector3 Scale { get; set; }

        [XNBProperty(UseConverter = true)]
        public ArtObjectActorSettings ActorSettings { get; set; }

        
        public ArtObjectInstance()
        {
            Name = "";
            ActorSettings = new();
        }
    }
}
