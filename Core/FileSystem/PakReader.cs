using System.Text;

namespace FEZRepacker.Core.FileSystem
{
    /// <summary>
    /// Allows accessing individual files in a FEZ PAK package contained in the given stream.
    /// Use this for a memory-efficient access to files in a PAK package.
    /// </summary>
    public class PakReader : IDisposable
    {
        private readonly BinaryReader reader;
        public uint FileCount { get; private set; }

        /// <summary>
        /// Initializes PAK package reader.
        /// </summary>
        /// <param name="stream">A stream to read a package from.</param>
        public PakReader(Stream stream)
        {
            reader = new BinaryReader(stream, Encoding.UTF8, false);

            FileCount = reader.ReadUInt32();
        }

        /// <summary>
        /// Enumerates files from the package.
        /// </summary>
        /// <remarks>Enumeration copies the content of the file into a buffer.</remarks>
        /// <returns>A list of files contained within the package.</returns>
        public IEnumerable<PakFileRecord> ReadFiles()
        {
            for (var i = 0; i < FileCount; i++)
            {
                string name = reader.ReadString();
                uint fileSize = reader.ReadUInt32();
                var data = reader.ReadBytes((int)fileSize);

                yield return new PakFileRecord(name, data);
            }
        }

        /// <summary>
        /// Loads a file from path and uses it to create PAK archive reader.
        /// </summary>
        /// <param name="filepath">A path of a file to load a stream from.</param>
        /// <returns>PAK archive reader assigned to a stream of a loaded file.</returns>
        /// <exception cref="FormatException">Thrown when given file path does not end with .pak extension</exception>
        public static PakReader FromFile(string filepath)
        {
            if (Path.GetExtension(filepath) != ".pak")
            {
                throw new FormatException("PAK package path must be a .PAK file.");
            }

            var fileStream = File.Open(filepath, FileMode.Open);
            return new PakReader(fileStream);
        }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}