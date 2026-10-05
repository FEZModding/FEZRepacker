using System.Text;

using XnbCompress;

namespace FEZRepacker.Core.XNB
{
    /// <summary>
    /// Reads a complete XNB with its payload compressed as framed LZX data.
    /// Non-XNB and already compressed input is copied unchanged.
    /// </summary>
    public sealed class XnbCompressStream : Stream
    {
        private readonly Stream _source;
        private readonly MemoryStream _compressionBuffer;

        public override bool CanRead => _compressionBuffer.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _compressionBuffer.Length;
        public override long Position
        {
            get => _compressionBuffer.Position;
            set => throw new NotSupportedException();
        }

        public XnbCompressStream(Stream source)
        {
            _source = source;
            _compressionBuffer = new MemoryStream();
            var sourcePosition = source.Position;

            if (!TryProcessHeader(out var header, out var fileLength) ||
                (header.Flags & XnbHeader.XnbFlags.Compressed) != 0)
            {
                source.Position = sourcePosition;
                source.CopyTo(_compressionBuffer);
                _compressionBuffer.Position = 0;
                source.Position = sourcePosition;
                return;
            }

            var payloadLength = fileLength - XnbHeader.Size;
            header.Flags |= XnbHeader.XnbFlags.Compressed;
            header.Write(_compressionBuffer);

            using var writer = new BinaryWriter(_compressionBuffer, Encoding.UTF8, true);
            writer.Write(0); // Total file length will be filled in after compression
            writer.Write(payloadLength);

            const int outLen = int.MaxValue - XnbHeader.Size - sizeof(int);
            var compressor = new XMemCompressor(windowExponent: 16, partitionSize: 0x80000);
            compressor.Compress(source, payloadLength, _compressionBuffer, outLen);

            // Include both size fields and all LZX framing in the container length
            var compressedLength = checked((int)_compressionBuffer.Length);
            _compressionBuffer.Position = XnbHeader.Size - sizeof(int);
            writer.Write(compressedLength);
            _compressionBuffer.Position = 0;
            source.Position = sourcePosition;
        }

        private bool TryProcessHeader(out XnbHeader header, out int fileLength)
        {
            var sourcePositionPreHeaderRead = _source.Position;
            if (!XnbHeader.TryRead(_source, out header))
            {
                fileLength = 0;
                _source.Position = sourcePositionPreHeaderRead;
                return false;
            }

            using var reader = new BinaryReader(_source, Encoding.UTF8, true);
            fileLength = reader.ReadInt32();
            return true;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _compressionBuffer.Read(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _compressionBuffer.Dispose();
            }

            base.Dispose(disposing);
        }

        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
