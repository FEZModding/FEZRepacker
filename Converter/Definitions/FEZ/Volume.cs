using System.Numerics;

namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Readers.VolumeReader")]
    internal class Volume
    {
        [XnbProperty(UseConverter = true)]
        public FaceOrientation[] Orientations { get; set; }

        [XnbProperty]
        public Vector3 From { get; set; }

        [XnbProperty]
        public Vector3 To { get; set; }

        [XnbProperty(UseConverter = true)]
        public VolumeActorSettings ActorSettings { get; set; }


        public Volume()
        {
            Orientations = new FaceOrientation[0];
            ActorSettings = new VolumeActorSettings();
        }
    }
}
