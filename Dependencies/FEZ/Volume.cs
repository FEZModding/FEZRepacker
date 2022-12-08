using System.Numerics;

namespace FEZEngine.Structure
{
    class Volume
    {
        public FaceOrientation[] Orientations { get; set; }
        public Vector3 From { get; set; }
        public Vector3 To { get; set; }
        public VolumeActorSettings ActorSettings { get; set; }


        public Volume()
        {
            Orientations = new FaceOrientation[0];
            ActorSettings = new VolumeActorSettings();
        }
    }
}
