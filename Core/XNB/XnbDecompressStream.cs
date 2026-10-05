using System.Text;

using XnbCompress;

namespace FEZRepacker.Core.XNB
{
    /// <summary>
    /// A stream for reading compressed XNB assets, including the header.
    /// The framed payload is decompressed once when reading reaches it.
    /// If an underlying stream does not contain compressed XNB asset, its content is simply copied over.
    /// </summary>
    public sealed class XnbDecompressStream : Stream
    {
        private readonly Stream _source;
        private readonly bool _copyOriginalSource;
        private readonly MemoryStream _decompressionBuffer;
        private readonly XMemDecompressor? _decoder;
        private readonly int _compressedSize;
        private readonly int _decompressedSize;
        private long _readPosition;
        private readonly long _sourcePosition;
        private bool _finished;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _copyOriginalSource ? _source.Length : _decompressedSize;
        public override long Position
        {
            get => _copyOriginalSource ? _source.Position : _readPosition;
            set => throw new NotSupportedException();
        }

        public XnbDecompressStream(Stream source)
        {
            _source = source;
            _decompressionBuffer = new MemoryStream();

            if (!TryProcessHeader(out var compressedSize, out var decompressedSize))
            {
                _copyOriginalSource = true;
                return;
            }
            
            _decoder = new XMemDecompressor(16);
            _sourcePosition = source.Position;
            _compressedSize = compressedSize;
            _decompressedSize = decompressedSize;
        }

        private bool TryProcessHeader(out int compressedSize, out int decompressedSize)
        {
            var sourcePositionPreHeaderRead = _source.Position;
            if (!XnbHeader.TryRead(_source, out var header) || (header.Flags & XnbHeader.XnbFlags.Compressed) == 0)
            {
                compressedSize = 0;
                decompressedSize = 0;
                _source.Position = sourcePositionPreHeaderRead;
                return false;
            }
            
            using var reader = new BinaryReader(_source, Encoding.UTF8, true);
            var fileSize = reader.ReadInt32();
            var payloadSize = reader.ReadInt32();
            compressedSize = fileSize - XnbHeader.Size - sizeof(int);
            if (compressedSize < 0 || payloadSize < 0 || payloadSize > int.MaxValue - XnbHeader.Size)
            {
                throw new XnbSerializationException("Invalid compressed XNB lengths.");
            }

            decompressedSize = payloadSize + XnbHeader.Size;
            header.Flags &= ~XnbHeader.XnbFlags.Compressed;
            header.Write(_decompressionBuffer);
            using var writer = new BinaryWriter(_decompressionBuffer, Encoding.UTF8, true);
            writer.Write(decompressedSize);

            return true;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_copyOriginalSource)
            {
                return _source.Read(buffer, offset, count);
            }

            int bytesRead = 0;
            if (_decompressionBuffer.Length - _readPosition < count && !_finished)
            {
                DecompressPayload();
            }

            int decompressedBytesToReadCount = (int)Math.Min(count, _decompressionBuffer.Length - _readPosition);
            if (decompressedBytesToReadCount > 0)
            {
                _decompressionBuffer.Position = _readPosition;
                int read = _decompressionBuffer.Read(buffer, offset, decompressedBytesToReadCount);
                _readPosition += read;
                bytesRead += read;
            }

            return bytesRead;
        }

        private void DecompressPayload()
        {
            _source.Position = _sourcePosition;
            _decompressionBuffer.Position = _decompressionBuffer.Length;

            var status = _decoder!.Decompress(_source, _compressedSize,
                _decompressionBuffer, _decompressedSize - XnbHeader.Size);

            if (status != 0)
            {
                throw new XnbSerializationException($"XNB decompression failed with status {status}.");
            }

            MarkFinished();
        }

        private void MarkFinished()
        {
            _finished = true;
            var finalSize = _decompressionBuffer.Length;
            if (finalSize != _decompressedSize)
            {
                throw new XnbSerializationException(
                    $"XNB decompression data size mismatch - expected {_decompressedSize}, got {finalSize}"
                );
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _decompressionBuffer.Dispose();
            }
            
            base.Dispose(disposing);
        }
        
        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
