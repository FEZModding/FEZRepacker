namespace FEZRepacker.Converter.FileSystem
{
    public class PakFile
    {
        internal class PakFileStream : MemoryStream
        {
            public PakFile File { get; private set; }
            public PakFileStream(PakFile file) : base()
            {
                File = file;
                if (file.data.Length > 0)
                {
                    Write(file.data, 0, file.data.Length);
                    Position = 0;
                }
            }
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                File.data = ToArray();
            }
        }


        public string Path;

        private byte[] data;
        private PakFileStream? openStream;

        public int Size => data.Length;

        public PakFile()
        {
            Path = "";
            data = new byte[0];
        }

        public Stream Open()
        {
            openStream = new PakFileStream(this);
            return openStream;
        }

        public string GetExtensionFromHeaderOrDefault(string defaultExtension = ".unk")
        {
            var extension = defaultExtension;

            if (data.Length >= 4)
            {
                if (data[0] == 'X' && data[1] == 'N' && data[2] == 'B')
                {
                    extension = ".xnb";
                }
                else if (data[0] == 'O' && data[1] == 'g' && data[2] == 'g' && data[3] == 'S')
                {
                    extension = ".ogg";
                }
                else if (data[0] == 0x01 && data[1] == 0x09 && data[2] == 0xFF && data[3] == 0xFE)
                {
                    extension = ".fxc";
                }
            }

            return extension;
        }

        public static PakFile Read(string path, Stream stream)
        {
            var file = new PakFile
            {
                Path = path
            };
            using var fileStream = file.Open();
            stream.CopyTo(fileStream);
            return file;
        }
    }
}
