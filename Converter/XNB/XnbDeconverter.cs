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

        public void Deconvert(Stream input, Stream output)
        {
            if(FormatConverter == null)
            {
                input.CopyTo(output);
                Converted = false;
                return;
            }

            using var inputReader = new BinaryReader(input);
            using var outputWriter = new BinaryWriter(output);

            using var xnbContentWriter = new BinaryWriter(new MemoryStream());

            // write list of XNB types
            xnbContentWriter.Write7BitEncodedInt(FormatConverter.ContentTypes.Length);
            foreach (var type in FormatConverter.ContentTypes)
            {
                xnbContentWriter.Write(type.Name.GetDisplayName(false)); // name
                xnbContentWriter.Write(0); // version
            }

            // number of shared resources (0 for FEZ assets)
            xnbContentWriter.Write7BitEncodedInt(0);

            // main resource id (in my system, first one is always the primary one)
            xnbContentWriter.Write7BitEncodedInt(1);

            // convert actual data
            FormatConverter.ToBinary(inputReader, xnbContentWriter);

            // copy length and data into output stream
            outputWriter.Write(xnbContentWriter.BaseStream.Length);
            xnbContentWriter.BaseStream.CopyTo(output);
            Converted = true;
        }
    }
}
