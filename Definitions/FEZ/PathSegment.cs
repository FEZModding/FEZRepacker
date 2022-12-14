using FEZRepacker.XNB.Attributes;
using System.Numerics;

namespace FEZEngine.Structure
{
    [XNBType("FezEngine.Readers.PathSegmentReader")]
    public class PathSegment
    {
        [XNBProperty]
        public Vector3 Destination { get; set; }

        [XNBProperty(UseConverter = true)]
        public TimeSpan Duration { get; set; }

        [XNBProperty(UseConverter = true)]
        public TimeSpan WaitTimeOnStart { get; set; }

        [XNBProperty(UseConverter = true)]
        public TimeSpan WaitTimeOnFinish { get; set; }

        [XNBProperty]
        public float Acceleration { get; set; }

        [XNBProperty]
        public float Deceleration { get; set; }

        [XNBProperty]
        public float JitterFactor { get; set; }

        [XNBProperty]
        public Quaternion Orientation { get; set; }

        [XNBProperty(UseConverter = true, Optional = true)]
        public CameraNodeData? CustomData { get; set; }


        public PathSegment()
        {
            CustomData = null;
            Duration = TimeSpan.FromSeconds(1.0f);
            Orientation = Quaternion.Identity;
        }
    }
}
