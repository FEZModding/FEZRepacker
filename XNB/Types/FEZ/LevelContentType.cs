using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace FEZRepacker.XNB.Types.FEZ
{
    class Level
    {
        public string Name;
        [YamlMember(SerializeAs = typeof(string))]
        public Vector3 Size;
        //public TrileFace StartingFace;
        public string SequenceSamplesPath;
        public bool Flat;
        public bool SkipPostProcess;
        public float BaseDiffuse;
        public float BaseAmbient;
        public string GomezHaloName;
        public bool HaloFiltering;
        public bool BlinkingAlpha;
        public bool Loops;
        public int WaterType;
        public float WaterHeight;
        public string SkyName;
        public string TrileSetName;
    }

    class LevelContentType : XNBContentType<Level>
    {
        public LevelContentType(XNBContentConverter converter) : base(converter){}

        public override TypeAssemblyQualifier Name => "FezEngine.Readers.LevelReader";

        public override object Read(BinaryReader reader)
        {
            Level level = new Level();

            return level;
        }

        public override void Write(object data, BinaryWriter writer)
        {

        }
    }
}
