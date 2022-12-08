using System.Numerics;

namespace FEZEngine.Structure
{
    public class PathSegment
    {
        public Vector3 Destination { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan WaitTimeOnStart { get; set; }
        public TimeSpan WaitTimeOnFinish { get; set; }
        public float Acceleration { get; set; }
        public float Deceleration { get; set; }
        public float JitterFactor { get; set; }
        public Quaternion Orientation { get; set; }
        public CameraNodeData? CustomData { get; set; }


        public PathSegment()
        {
            CustomData = null;
            Duration = TimeSpan.FromSeconds(1.0f);
            Orientation = Quaternion.Identity;
        }
    }
}
