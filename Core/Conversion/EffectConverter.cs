
using FEZRepacker.Core.Definitions.Game.XNA;
using FEZRepacker.Core.FileSystem;

namespace FEZRepacker.Core.Conversion
{
    internal class EffectConverter : FormatConverter<Effect>
    {
        public override string FileFormat => ".fxb";

        public override FileBundle ConvertTyped(Effect data)
        {
            var outStream = new MemoryStream();
            var outWriter = new BinaryWriter(outStream);

            outWriter.Write(data.Data.Length);
            outWriter.Write(data.Data);

            outStream.Seek(0, SeekOrigin.Begin);
            return FileBundle.Single(outStream, FileFormat);
        }

        public override Effect DeconvertTyped(FileBundle bundle)
        {
            var inReader = new BinaryReader(bundle.GetData(""));
            return new Effect()
            {
                Data = inReader.ReadBytes(inReader.ReadInt32())
            };
        }
    }
}
