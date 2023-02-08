using FEZRepacker.Converter.Helpers;
using FEZRepacker.Converter.XNB.Formats;

namespace FEZRepacker.Converter.XNB
{
    public class XnbDeconverter
    {
        public XnbFormatConverter? FormatConverter;
        public bool Converted { get; private set; }

        public XnbDeconverter(string extension)
        {
            FormatConverter = XnbFormatList.FindByExtension(extension);
            Converted = false;
        }

        public Stream Deconvert(Stream input)
        {
            var output = new MemoryStream();

            if(FormatConverter == null)
            {
                input.CopyTo(output);
                output.Position = 0;
                Converted = false;
                return output;
            }

            XnbHeader.Default.Write(output);

            using var xnbContentWriter = new BinaryWriter(new MemoryStream());

            // write list of XNB types
            xnbContentWriter.Write7BitEncodedInt(FormatConverter.PublicContentTypes.Count);
            foreach (var type in FormatConverter.PublicContentTypes)
            {
                xnbContentWriter.Write(type.Name.GetDisplayName(false)); // name
                xnbContentWriter.Write(0); // version
            }

            // number of shared resources (0 for FEZ assets)
            xnbContentWriter.Write7BitEncodedInt(0);

            // main resource id (in my system, first one is always the primary one)
            xnbContentWriter.Write7BitEncodedInt(1);

            // convert actual data
            using var inputReader = new BinaryReader(input);
            FormatConverter.ToBinary(inputReader, xnbContentWriter);

            // copy length of the file (including header block of 10 bytes) and data into output stream
            new BinaryWriter(output).Write((int)xnbContentWriter.BaseStream.Length + 10);
            xnbContentWriter.BaseStream.Position = 0;
            xnbContentWriter.BaseStream.CopyTo(output);
            Converted = true;
            output.Position = 0;
            return output;
        }
    }
}
