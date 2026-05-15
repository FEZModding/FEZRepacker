using System.Text;

using Microsoft.Xna.Framework.Content;

namespace FEZRepacker.Core.XNB
{
    /// <summary>
    /// A stream for reading compressed XNB asset, including header. Data is decompressed progressively as it's needed.
    /// If an underlying stream does not contain compressed XNB asset, its content is simply copied over.
    /// </summary>
    public sealed class XnbDecompressStream : Stream
    {
        private readonly Stream _source;
        private readonly bool _copyOriginalSource;
        private readonly MemoryStream _decompressionBuffer;
        private readonly LzxDecoder? _decoder;
        private readonly long _sourceEndPosition;
        private readonly int _decompressedSize;
        private long _readPosition;
        private long _sourcePosition;
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
            
            _decoder = new LzxDecoder(16);
            _sourcePosition = source.Position;
            _sourceEndPosition = _sourcePosition + compressedSize;
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
            compressedSize = reader.ReadInt32();
            decompressedSize = reader.ReadInt32() + XnbHeader.Size;
            
            header.Flags &= ~XnbHeader.XnbFlags.Compressed;
            header.Write(_decompressionBuffer);
            new BinaryWriter(_decompressionBuffer).Write(decompressedSize);

            return true;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_copyOriginalSource)
            {
                return _source.Read(buffer, offset, count);
            }

            int bytesRead = 0;

            while (_decompressionBuffer.Length - _readPosition < count && !_finished)
            {
                DecompressNextBlock();
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

        private void DecompressNextBlock()
        {
            if (_sourcePosition >= _sourceEndPosition)
            {
                MarkFinished();
                return;
            }

            _source.Position = _sourcePosition;

            // all of these shorts are big-endian
            int flag = _source.ReadByte();
            int frameSize, blockSize;
            if (flag == 0xFF)
            {
                frameSize = (_source.ReadByte() << 8) | _source.ReadByte();
                blockSize = (_source.ReadByte() << 8) | _source.ReadByte();
                _sourcePosition += 5;
            }
            else
            {
                frameSize = 0x8000;
                blockSize = (flag << 8) | _source.ReadByte();
                _sourcePosition += 2;
            }

            if (blockSize == 0 || frameSize == 0)
            {
                MarkFinished();
                return;
            }

            _decompressionBuffer.Position = _decompressionBuffer.Length;
            _decoder!.Decompress(_source, blockSize, _decompressionBuffer, frameSize);
            _sourcePosition += blockSize;
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
