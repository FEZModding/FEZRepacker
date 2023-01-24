using System.Numerics;

namespace FEZRepacker.Converter.Definitions.FezEngine.Structure
{
    [XnbType("FezEngine.Readers.PathSegmentReader")]
    internal class PathSegment
    {
        [XnbProperty]
        public Vector3 Destination { get; set; }

        [XnbProperty(UseConverter = true)]
        public TimeSpan Duration { get; set; }

        [XnbProperty(UseConverter = true)]
        public TimeSpan WaitTimeOnStart { get; set; }

        [XnbProperty(UseConverter = true)]
        public TimeSpan WaitTimeOnFinish { get; set; }

        [XnbProperty]
        public float Acceleration { get; set; }

        [XnbProperty]
        public float Deceleration { get; set; }

        [XnbProperty]
        public float JitterFactor { get; set; }

        [XnbProperty]
        public Quaternion Orientation { get; set; }

        [XnbProperty(UseConverter = true, Optional = true)]
        public CameraNodeData? CustomData { get; set; }


        public PathSegment()
        {
            CustomData = null;
            Duration = TimeSpan.FromSeconds(1.0f);
            Orientation = Quaternion.Identity;
        }
    }
}
