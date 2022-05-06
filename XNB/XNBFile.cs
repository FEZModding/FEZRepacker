using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FEZRepacker.XNB;

namespace FEZRepacker
{

    class XNBFile : PAKFile
    {
        private XNBContent _xnbcontent = null;

        public char Platform { get; private set; }
        public byte Version { get; private set; }
        public bool IsCompressed { get; private set; }
        public bool IsForHiDef { get; private set; }

        public bool SaveAsPacked { get; set; }

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

        public XNBContent ReadXNBContent()
        {
            if (_xnbcontent != null) return _xnbcontent;

            if (IsCompressed) Decompress();
            if (!IsValid) return null;

            using var xnbStream = new MemoryStream(_content);
            using var xnbReader = new BinaryReader(xnbStream, Encoding.UTF8, false);

            int readerCount = xnbReader.Read7BitEncodedInt();

            List<TypeAssemblyQualifier> usedTypes = new List<TypeAssemblyQualifier>();

            for (var i = 0; i < readerCount; i++)
            {
                string readerName = xnbReader.ReadString();
                int readerVersion = xnbReader.ReadInt32();

                var qualifier = new TypeAssemblyQualifier(readerName);

                usedTypes.Add(qualifier);

                //Console.WriteLine(qualifier.GetDisplayName());
            }

            // shared resource count + main resource
            int resourceCount = xnbReader.Read7BitEncodedInt() + 1;

            // FEZ XNB files shouldn't have more than 1 resource
            if (resourceCount != 1)
            {
                // TODO: exception here
                return null;
            }

            int resourceTypeID = xnbReader.Read7BitEncodedInt();
            TypeAssemblyQualifier mainType = usedTypes[resourceTypeID - 1];


            Console.WriteLine("====MAIN TYPE:" + mainType + "====");

            var converter = XNBContentConverterList.Get(mainType);

            if (converter == null)
            {
                Console.WriteLine("UNKNOWN DATA FORMAT: " + mainType);
                return null;
                //throw new InvalidDataException();
            }

            var xnbContentData = _content.Skip((int)xnbStream.Position).ToArray();

            using var reader = new BinaryReader(new MemoryStream(xnbContentData), Encoding.UTF8, false);

            _xnbcontent = converter.Read(reader);
            return _xnbcontent;
        }

        public void WriteXNBContent(XNBContent content)
        {

        }

        public void Decompress()
        {
            if (!IsValid || !IsCompressed) return;

            var newData = XNBDecompressor.Decompress(_content);
            _content = newData;
            IsCompressed = false;
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

        public override string GetExtension()
        {
            if (!SaveAsPacked)
            {
                return ReadXNBContent().Converter.FileFormat;
            }
            else
            {
                return base.GetExtension();
            }
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
            if (SaveAsPacked)
            {
                // we're saving packed file - reconstruct the header and write the content
                writer.Write(new char[] { 'X', 'N', 'B'});
                writer.Write(Platform);
                writer.Write(Version);

                byte flags = 0;
                if (IsCompressed) flags |= XNBConstants.FLAG_COMPRESSED;
                if (IsForHiDef) flags |= XNBConstants.FLAG_HIDEF;
                writer.Write(flags);

                writer.Write(_content.Length);
                writer.Write(_content);
            }
            else
            {
                // we're saving unpacked file - write XNB content as our custom file format
                // most of these are going to be YAML based, but some are Ogg sounds or TGA textures
                Decompress();

                if (!IsValid) return;
                
                XNBContent content = ReadXNBContent();

                if (content == null) return;
                
                content.Converter.WriteUnpacked(content, writer);
            }
        }
    }
}
