namespace FEZRepacker.Core.FileSystem
{
    public class PakFileRecord
    {
        private readonly string path;
        private byte[] payload;
        private bool containsData;

        public string Path => path;
        public int Length => payload.Length;
        public byte[] Payload => payload;

        public PakFileRecord(string path)
        {
            this.path = path;
            this.payload = [];
            this.containsData = false;
        }

        public Stream Open()
        {
            if (!containsData)
            {
                return new RecordWriterStream(this);
            }
            else
            {
                return new MemoryStream(payload);
            }
        }

        public string FindExtension()
        {
            var extension = "";

            if (payload.Length >= 4)
            {
                if (payload[0] == 'X' && payload[1] == 'N' && payload[2] == 'B')
                {
                    extension = ".xnb";
                }
                else if (payload[0] == 'O' && payload[1] == 'g' && payload[2] == 'g' && payload[3] == 'S')
                {
                    extension = ".ogg";
                }
                else if (payload[0] == 0x01 && payload[1] == 0x09 && payload[2] == 0xFF && payload[3] == 0xFE)
                {
                    extension = ".fxc";
                }
            }

            return extension;
        }

        private class RecordWriterStream : MemoryStream
        {
            private readonly PakFileRecord fileRecord;
            public RecordWriterStream(PakFileRecord record) : base()
            {
                fileRecord = record;
            }
            public override void Close()
            {
                fileRecord.payload = ToArray();
                fileRecord.containsData = true;
                base.Close();
            }
        }
    }
}
