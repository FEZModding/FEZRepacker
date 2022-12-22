using FEZRepacker.XNB.Attributes;
using System.Numerics;

namespace FezEngine.Structure
{
    [XNBType("FezEngine.Readers.VolumeReader")]
    class Volume
    {
        [XNBProperty(UseConverter = true)]
        public FaceOrientation[] Orientations { get; set; }

        [XNBProperty]
        public Vector3 From { get; set; }

        [XNBProperty]
        public Vector3 To { get; set; }

        [XNBProperty(UseConverter = true)]
        public VolumeActorSettings ActorSettings { get; set; }


        public Volume()
        {
            Orientations = new FaceOrientation[0];
            ActorSettings = new VolumeActorSettings();
        }
    }
}
