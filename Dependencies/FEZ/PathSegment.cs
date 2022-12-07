using System.Numerics;

namespace FEZEngine.Structure
{
    public class PathSegment
    {
        public Vector3 Destination;
        public TimeSpan Duration;
        public TimeSpan WaitTimeOnStart;
        public TimeSpan WaitTimeOnFinish;
        public float Acceleration;
        public float Deceleration;
        public float JitterFactor;
        public Quaternion Orientation;
        public CameraNodeData? CustomData;
    }
}
