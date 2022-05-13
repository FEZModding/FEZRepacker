using System.Text;

namespace FEZRepacker
{

    class XNBFile : PAKFile
    {
        public char Platform { get; private set; }
        public byte Version { get; private set; }
        public bool IsCompressed { get; private set; }
        public bool IsForHiDef { get; private set; }

        public XNBFile()
        {
            // set default header info
            Identifier = XNBConstants.IDENTIFIER;
            Platform = XNBConstants.PLATFORM_WINDOWS;
            Version = XNBConstants.VERSION;
            IsCompressed = false;
            IsForHiDef = false;

            // empty file, no data until it's passed by external stuff
            Validate();
        }

        public int GetXNBContentSize(bool uncompressed = false)
        {
            // uncompressed data size
            if (!uncompressed || !IsCompressed)
            {
                return _content.Length;
            }

            // compressed data size - it should start with it
            if (_content.Length >= 4)
            {
                return BitConverter.ToInt32(_content, 0);
            }
            else return 0;
        }

        public XNBFile Decompressed()
        {
            XNBFile decomp = (XNBFile)MemberwiseClone();
            if (!decomp.IsValid || !decomp.IsCompressed) return decomp;

            var newData = XNBDecompressor.Decompress(_content);
            decomp._content = newData;
            decomp.IsCompressed = false;
            return decomp;
        }

        // validates XNB file (specifically ones present in FEZ)
        public override void Validate()
        {
            IsValid = false;

            if (Identifier != XNBConstants.IDENTIFIER) return;
            if (Platform != XNBConstants.PLATFORM_WINDOWS) return;
            if (Version != XNBConstants.VERSION) return;
            if (_content.Length == 0) return;

            IsValid = true;
        }

        public override string GetInfo()
        {
            var validStr = (IsValid ? "" : "invalid ");
            var compSize = GetXNBContentSize();
            var uncompSize = GetXNBContentSize(true);

            var compStr = $"{validStr}XNB file; size: {compSize}B" + (IsCompressed ? $" compressed, {uncompSize}B uncompressed" : "");

            return compStr;
        }

        public override int GetSize()
        {
            return _content.Length + XNBConstants.HEADER_SIZE;
        }

        new public static XNBFile FromData(byte[] xnbData)
        {
            var xnb = new XNBFile();

            using var xnbStream = new MemoryStream(xnbData);
            using var xnbReader = new BinaryReader(xnbStream, Encoding.UTF8, false);

            xnb.Identifier = new string(xnbReader.ReadChars(3));
            xnb.Platform = xnbReader.ReadChar();
            xnb.Version = xnbReader.ReadByte();
            var flags = xnbReader.ReadByte();
            xnb.IsCompressed = (flags & XNBConstants.FLAG_COMPRESSED) > 0;
            xnb.IsForHiDef = (flags & XNBConstants.FLAG_HIDEF) > 0;

            var xnbFileSize = xnbReader.ReadUInt32();
            var fileContentSize = (int)xnbFileSize - XNBConstants.HEADER_SIZE;

            if(fileContentSize > 0)
            {
                var data = xnbReader.ReadBytes(fileContentSize);
                xnb.SetData(data);
            }

            return xnb;
        }
        public override void Write(BinaryWriter writer)
        {
            // we're saving packed file - reconstruct the header and write the content
            writer.Write(new char[] { 'X', 'N', 'B'});
            writer.Write(Platform);
            writer.Write(Version);

            byte flags = 0;
            if (IsCompressed) flags |= XNBConstants.FLAG_COMPRESSED;
            if (IsForHiDef) flags |= XNBConstants.FLAG_HIDEF;
            writer.Write(flags);

            writer.Write(GetSize());
            writer.Write(_content);
        }
    }
}
