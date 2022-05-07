using FEZEngine;
using FEZEngine.Structure;
using FEZRepacker.XNB.Types;
using FEZRepacker.XNB.Types.FEZ;
using FEZRepacker.XNB.Types.System;

namespace FEZRepacker.XNB.Converters.Files
{
    class LevelConverter : YamlStorageConverter<Level>
    {
        public override XNBContentType[] Types => new XNBContentType[]
        {
            new LevelContentType(this),
            new StringContentType(this),
            new TrileFaceContentType(this),
            new TrileEmplacementContentType(this),
            new EnumContentType<FaceOrientation>(this)
        };
        public override string FileFormat => "fezlvl";
    }
}
