﻿using FEZRepacker.Core.Definitions.Game.XNA;

namespace FEZRepacker.Core.Definitions.Game.Level
{
    [XnbType("FezEngine.Structure.PathSegment")]
    [XnbReaderType("FezEngine.Readers.PathSegmentReader")]
    public class PathSegment
    {
        [XnbProperty]
        public Vector3 Destination { get; set; }

        [XnbProperty(UseConverter = true)]
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(1.0f);

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
        public Quaternion Orientation { get; set; } = Quaternion.Identity;

        [XnbProperty(UseConverter = true, Optional = true)]
        public CameraNodeData? CustomData { get; set; } = null;
    }
}
