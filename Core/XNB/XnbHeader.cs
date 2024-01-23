using System.Text;

namespace FEZRepacker.Core.XNB
{
    /// <summary>
    /// Stores information present in XNB file header.
    /// </summary>
    public struct XnbHeader
    {
        [Flags]
        public enum XnbFlags
        {
            None = 0x00,
            HiDefProfile = 0x01,
            Compressed = 0x80,
        }

        public enum XnbPlatformIdentifier
        {
            Windows = 'w',
            Mobile = 'm',
            XBox = 'x',
        }

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

        /// <summary>
        /// Attempts to read XNB header from given stream.
        /// </summary>
        /// <remarks>Stream position can change even if reading fails.</remarks>
        /// <param name="stream">Stream to read header to</param>
        /// <param name="header">When read successfully, stores XNB header.</param>
        /// <returns>True if read header from given stream successfully, false otherwise.</returns>
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

        /// <summary>
        /// Writes XNB header into the stream.
        /// </summary>
        /// <param name="stream">Steam to write XNB header to.</param>
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
