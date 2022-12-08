using FEZEngine;
using FEZEngine.Structure;
using FEZEngine.Structure.Scripting;
using System.Numerics;

namespace FEZRepacker.XNB.Types.FEZ
{
    class LevelContentType : XNBContentType<Level>
    {
        public LevelContentType(XNBContentConverter converter) : base(converter) { }

        public override FEZAssemblyQualifier Name => "FezEngine.Readers.LevelReader";

        public override object Read(BinaryReader reader)
        {
            Level level = new Level();

            level.Name = Converter.ReadType<string>(reader) ?? "";
            level.Size = new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
            );
            level.StartingFace = Converter.ReadType<TrileFace>(reader) ?? level.StartingFace;
            level.SequenceSamplesPath = Converter.ReadType<string>(reader) ?? level.SequenceSamplesPath;
            level.Flat = reader.ReadBoolean();
            level.SkipPostProcess = reader.ReadBoolean();
            level.BaseDiffuse = reader.ReadSingle();
            level.BaseAmbient = reader.ReadSingle();
            level.GomezHaloName = Converter.ReadType<string>(reader) ?? level.GomezHaloName;
            level.HaloFiltering = reader.ReadBoolean();
            level.BlinkingAlpha = reader.ReadBoolean();
            level.Loops = reader.ReadBoolean();
            level.WaterType = Converter.ReadType<LiquidType>(reader);
            level.WaterHeight = reader.ReadSingle();
            level.SkyName = reader.ReadString();
            level.TrileSetName = Converter.ReadType<string>(reader) ?? level.TrileSetName;
            level.Volumes = Converter.ReadType<Dictionary<int, Volume>>(reader) ?? level.Volumes;
            level.Scripts = Converter.ReadType<Dictionary<int, Script>>(reader) ?? level.Scripts;
            level.SongName = Converter.ReadType<string>(reader) ?? level.SongName;
            level.FarAwayPlaceFadeOutStart = reader.ReadInt32();
            level.FarAwayPlaceFadeOutLength = reader.ReadInt32();

            level.Triles = Converter.ReadType<Dictionary<TrileEmplacement, TrileInstance>>(reader) ?? level.Triles;
            level.ArtObjects = Converter.ReadType<Dictionary<int, ArtObjectInstance>>(reader) ?? level.ArtObjects;
            level.BackgroundPlanes = Converter.ReadType<Dictionary<int, BackgroundPlane>>(reader) ?? level.BackgroundPlanes;
            level.Groups = Converter.ReadType<Dictionary<int, TrileGroup>>(reader) ?? level.Groups;
            level.NonPlayerCharacters = Converter.ReadType<Dictionary<int, NpcInstance>>(reader) ?? level.NonPlayerCharacters;
            level.Paths = Converter.ReadType<Dictionary<int, MovementPath>>(reader) ?? level.Paths;

            level.Descending = reader.ReadBoolean();
            level.Rainy = reader.ReadBoolean();
            level.LowPass = reader.ReadBoolean();
            level.MutedLoops = Converter.ReadType<List<string>>(reader) ?? level.MutedLoops;
            level.AmbienceTracks = Converter.ReadType<List<AmbienceTrack>>(reader) ?? level.AmbienceTracks;
            level.NodeType = Converter.ReadType<LevelNodeType>(reader);
            level.Quantum = reader.ReadBoolean();

            return level;
        }

        public override void Write(object data, BinaryWriter writer)
        {

        }
    }
}
