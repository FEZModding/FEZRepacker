namespace FEZRepacker.Converter.PAK
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
    }
}
