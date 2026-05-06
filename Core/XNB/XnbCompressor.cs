namespace FEZRepacker.Core.XNB
{
    public class XnbCompressor
    {
        /// <summary>
        /// Attempts to decompress given stream containing XNB file.
        /// </summary>
        /// <param name="xnbStream">A stream to convert</param>
        /// <returns>
        /// New stream containing decompressed XNB file, with position at its start.
        /// If given stream doesn't have a valid XNB file, returns a copy of input stream.
        /// </returns>
        /// <exception cref="InvalidDataException">Thrown when compressed data is invalid.</exception>
        public static Stream Decompress(Stream xnbStream)
        {
            return new XnbDecompressStream(xnbStream);
        }

        public static Stream Compress(Stream xnbStream)
        {
            throw new NotImplementedException();
        }
    }
}
