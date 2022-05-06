using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace FEZEngine
{
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z) 
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static explicit operator Vector3(string s)
        {
            Vector3 vec = new Vector3();
            float[] coordinates = Array.ConvertAll(s.Split(" "), float.Parse);
            if (coordinates.Length > 0) vec.x = coordinates[0];
            if (coordinates.Length > 1) vec.y = coordinates[1];
            if (coordinates.Length > 2) vec.z = coordinates[2];

            return vec;
        }

        public override string ToString()
        {
            return $"{x} {y} {z}";
        }
    }

    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static explicit operator Vector2(string s)
        {
            Vector2 vec = new Vector2();
            float[] coordinates = Array.ConvertAll(s.Split(" "), float.Parse);
            if (coordinates.Length > 0) vec.x = coordinates[0];
            if (coordinates.Length > 1) vec.y = coordinates[1];

            return vec;
        }

        public override string ToString()
        {
            return $"{x} {y}";
        }
    }

    public enum FaceOrientation
    {
        Left,
        Down,
        Back,
        Right,
        Top,
        Front,
    }

    struct TrileEmplacement
    {
        public int X;
        public int Y;
        public int Z;
        public TrileEmplacement(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static explicit operator TrileEmplacement(string s)
        {
            TrileEmplacement pos = new TrileEmplacement();
            int[] coordinates = Array.ConvertAll(s.Split(" "), int.Parse);
            if (coordinates.Length > 0) pos.X = coordinates[0];
            if (coordinates.Length > 1) pos.Y = coordinates[1];
            if (coordinates.Length > 2) pos.Z = coordinates[2];

            return pos;
        }

        public override string ToString()
        {
            return $"{X} {Y} {Z}";
        }
    }

    struct TrileFace
    {
        [YamlMember(SerializeAs = typeof(string))]
        public TrileEmplacement Position;
        public FaceOrientation Orientation;

        public TrileFace(int x, int y, int z, FaceOrientation orientation)
        {
            Position = new TrileEmplacement(x,y,z);
            Orientation = orientation;
        }
    }

    struct DotDialogueLine
    {
        public string Text;
        public bool Grouped;
    }

    public enum CodeInput
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
        SpinLeft = 16,
        SpinRight = 32,
        Jump = 64,
    }

    struct VolumeActorSettings
    {
        public Vector2 FarawayPlaneOffset;
        public bool IsPointOfIntersect;
        public List<DotDialogueLine> DotDialogue;
        public bool WaterLocked;
        public CodeInput[] CodePattern;
        public bool IsBlackHole;
        public bool NeedsTrigger;
        public bool IsSecretPassage;
    }

    struct Volume
    {
        public FaceOrientation[] Orientations;
        public Vector3 From;
        public Vector3 To;

        public VolumeActorSettings ActorSettings;
    }

    struct Entity
    {
        public string Type;
        public int Identifier;
    }

    struct ScriptTrigger
    {
        public Entity Object;
        public string Event;
    }

    public enum ComparisonOperator
    {
        None = -1,
        Equal = 0,
        Greater = 1,
        GreaterEqual = 2,
        Less = 3,
        LessEqual = 4,
        NotEqual = 5,
    }

    struct ScriptCondition
    {
        public Entity Object;
        public ComparisonOperator Operator;
        public string Property;
        public string Value;
    }

    struct ScriptAction
    {
        public Entity Object;
        public string Operation;
        public string[] Arguments;
        public bool Killswitch;
        public bool Blocking;
    }

    struct Script
    {
        public string Name;
        public TimeSpan Timeout;
        public List<ScriptTrigger> Triggers;
        public List<ScriptCondition> Conditions;
        public List<ScriptAction> Actions;
        public bool OneTime;
        public bool Triggerless;
        public bool IgnoreEndTriggers;
        public bool LevelWideOneTime;
        public bool Disabled;
        public bool IsWinCondition;
    }
}
