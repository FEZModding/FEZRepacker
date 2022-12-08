using System.Numerics;

namespace FEZEngine.Structure
{
    class ArtObjectInstance
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public ArtObjectActorSettings ActorSettings { get; set; }

        
        public ArtObjectInstance()
        {
            Name = "";
            ActorSettings = new();
        }
    }
}
