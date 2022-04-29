using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEZRepacker
{
    static class XNBConstants
    {
        public const string IDENTIFIER = "XNB";
        public const byte VERSION = 5;

        public const byte FLAG_HIDEF = 0x01;
        public const byte FLAG_COMPRESSED = 0x80;

        public const char PLATFORM_WINDOWS = 'w';
        public const char PLATFORM_MOBILE = 'm';
        public const char PLATFORM_XBOX = 'x';

        public const int HEADER_SIZE = 6;
    }

    class XNBFile
    {
        public string Identifier { get; private set; }
        public char Platform { get; private set; }
        public byte Version { get; private set; }
        public bool IsCompressed { get; private set; }
        public bool IsForHiDef { get; private set; }

        public bool IsValid { get; private set; }


        private byte[] _fileContents;
        public byte[] FileContents => _fileContents;



        
        public XNBFile()
        {
            // set default header info
            Identifier = XNBConstants.IDENTIFIER;
            Platform = XNBConstants.PLATFORM_WINDOWS;
            Version = XNBConstants.VERSION;
            IsCompressed = false;
            IsForHiDef = false;

            // empty file, no data until it's passed by external stuff
            _fileContents = new byte[0];
            Validate();
        }

        public void SetData(byte[] data, bool compressed = false)
        {
            _fileContents = data;
            IsCompressed = compressed;

            Validate();
        }

        public void Decompress()
        {
            if (!IsValid || !IsCompressed) return;

            var newData = XNBDecompressor.Decompress(_fileContents);
            _fileContents = newData;
            IsCompressed = false;
        }

        // validates XNB file (specifically ones present in FEZ)
        public void Validate()
        {
            IsValid = false;

            if (Identifier != XNBConstants.IDENTIFIER) return;
            if (Platform != XNBConstants.PLATFORM_WINDOWS) return;
            if (Version != XNBConstants.VERSION) return;
            if (_fileContents.Length == 0) return;

            IsValid = true;
        }

        public int GetContentsSize(bool uncompressed = false)
        {
            // uncompressed data size
            if (!uncompressed || !IsCompressed)
            {
                return _fileContents.Length;
            }

            // compressed data size - it should start with it
            if(_fileContents.Length >= 4)
            {
                return BitConverter.ToInt32(_fileContents, 0);
            }
            else return 0;
        }

        public static XNBFile FromStream(Stream xnbStream)
        {
            var xnb = new XNBFile();
            
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
                xnb.SetData(data, xnb.IsCompressed);
            }

            return xnb;
        }
    }
}
