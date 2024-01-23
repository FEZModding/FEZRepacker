﻿namespace FEZRepacker.Core.Definitions.Game.XNA
{
    [XnbType("Microsoft.Xna.Framework.Vector3")]
    [XnbReaderType("Microsoft.Xna.Framework.Content.Vector3Reader")]
    public class Vector3
    {
        [XnbProperty]
        public float X { get; set; }

        [XnbProperty]
        public float Y { get; set; }

        [XnbProperty]
        public float Z { get; set; }

        public static Vector3 Zero => new Vector3(0.0f, 0.0f, 0.0f);
        public static Vector3 One => new Vector3(1.0f, 1.0f, 1.0f);

        public Vector3() { }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float Length() => (float)Math.Sqrt(Dot(this,this));
        public static float Dot(Vector3 v, Vector3 w) => v.X * w.X + v.Y * w.Y + v.Z * w.Z;

        public static Vector3 operator +(Vector3 v) => v;
        public static Vector3 operator -(Vector3 v) => new Vector3(-v.X, -v.Y, -v.Z);

        public static Vector3 operator *(Vector3 v, float f) => new Vector3(v.X * f, v.Y * f, v.Z * f);
        public static Vector3 operator /(Vector3 v, float f) => new Vector3(v.X / f, v.Y / f, v.Z / f);

        public static Vector3 operator +(Vector3 v, Vector3 w) => new Vector3(v.X + w.X, v.Y + w.Y, v.Z + w.Y);
        public static Vector3 operator -(Vector3 v, Vector3 w) => new Vector3(v.X - w.X, v.Y - w.Y, v.Z - w.Z);
        public static Vector3 operator *(Vector3 v, Vector3 w) => new Vector3(v.X * w.X, v.Y * w.Y, v.Z * w.Y);
        public static Vector3 operator /(Vector3 v, Vector3 w) => new Vector3(v.X / w.X, v.Y / w.Y, v.Z / w.Z);
    }
}
