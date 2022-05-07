using FEZEngine;
using FEZEngine.Structure;
using FEZEngine.Structure.Input;
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
            new EnumContentType<FaceOrientation>(this),
            new Int32ContentType(this),
            new EnumContentType<LiquidType>(this),
            new DictionaryContentType<int, Volume>(this),
            new VolumeContentType(this),
            new ArrayContentType<FaceOrientation>(this),
            new VolumeActorSettingsContentType(this),
            new ListContentType<DotDialogueLine>(this),
            new DotDialogueLineContentType(this),
            new ArrayContentType<CodeInput>(this),
            new EnumContentType<CodeInput>(this),
        };
        public override string FileFormat => "fezlvl";
    }
}
