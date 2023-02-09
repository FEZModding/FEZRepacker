using System.Text;

namespace FEZRepacker.Converter.XNB
{
    public struct XnbHeader
    {
        public string FormatIdentifier;
        public XnbPlatformIdentifier PlatformIdentifier;
        public byte Version;
        public XnbFlags Flags;

        public static XnbHeader Default => new XnbHeader
        {
            FormatIdentifier = "XNB",
            PlatformIdentifier = XnbPlatformIdentifier.Windows,
            Version = 5,
            Flags = XnbFlags.HiDefProfile,
        };

        public static bool TryRead(Stream stream, out XnbHeader header)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            header = new XnbHeader();
            header.FormatIdentifier = new string(reader.ReadChars(3));
            if (header.FormatIdentifier != "XNB") return false;
            if (!Enum.TryParse(reader.ReadByte().ToString(), out XnbPlatformIdentifier platformIdentifier))
            {
                return false;
            }
            header.PlatformIdentifier = platformIdentifier;
            header.Version = reader.ReadByte();
            header.Flags = (XnbFlags)reader.ReadByte();

            return true;
        }

        public void Write(Stream stream)
        {
            var formatIdentifierBytes = FormatIdentifier.ToCharArray().Select(c => (byte)c).ToArray();
            stream.Write(formatIdentifierBytes, (int)stream.Position, formatIdentifierBytes.Length);
            stream.WriteByte((byte)PlatformIdentifier);
            stream.WriteByte(Version);
            stream.WriteByte((byte)Flags);
        }
    }
}