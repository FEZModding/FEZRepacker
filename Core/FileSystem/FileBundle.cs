namespace FEZRepacker.Converter.FileSystem
{

    /// <summary>
    /// Defines a set of file streams which can be collectively created from
    /// or converted into a single XNB asset.
    /// Files forming a bundle have two extensions, one defining the bundle extension
    /// and the other defining the individual file extension:
    /// <code>[filename].[bundle-extension].[file-extension]</code>
    /// </summary>
    public sealed class FileBundle : IDisposable
    {
        public struct FileRecord
        {
            public string Extension;
            public Stream Data;
        }

        public string BundlePath { get; set; }
        public string MainExtension { get; set; }
        public List<FileRecord> Files { get; private set; }

        public FileBundle() {
            Files = new();
        }

        public FileBundle(string extension, string path = "")
        {
            Files = new();
            MainExtension = extension;
            BundlePath = path;
        }

        public Stream GetData(params string[] validExtensions)
        {
            foreach (var extension in validExtensions) {
                foreach (var record in Files)
                {
                    if (record.Extension == extension) return record.Data;
                }
            }
            return new MemoryStream();
        }

        public HashSet<string> GetSubExtensions()
        {
            var extSet = new HashSet<string>();
            foreach (var item in Files) extSet.Add(item.Extension);
            return extSet;
        }

        public void Dispose()
        {
            foreach (var file in Files)
            {
                file.Data.Dispose();
            }
            this.Files.Clear();
        }

        public void AddFile(Stream stream, string subextension = "")
        {
            var fileRecord = new FileRecord()
            {
                Data = stream,
                Extension = subextension
            };
            Files.Add(fileRecord);
        }

        public static FileBundle Single(Stream stream, string extension="", string subextension = "")
        {
            var bundle = new FileBundle(extension);
            bundle.AddFile(stream, subextension);
            return bundle;
        }

        private static string GetRelativePath(string path, string relativeRootPath)
        {
            var fullPath = Path.GetFullPath(path);
            var fullRelativeRootPath = Path.GetFullPath(relativeRootPath + "\\");
            if (fullPath.StartsWith(fullRelativeRootPath))
            {
                return fullPath.Substring(fullRelativeRootPath.Length);
            }
            return path;
        }

        /// <summary>
        /// Bundles given list of files based on their extensions.
        /// </summary>
        /// <param name="files">Dictionary containing file paths paired with corresponding file streams.</param>
        /// <returns>List of organized file bundles.</returns>
        public static List<FileBundle> BundleFiles(Dictionary<string, Stream> files)
        {
            var bundles = new List<FileBundle>();

            foreach(var fileRecord in files)
            {
                var path = fileRecord.Key;
                var file = fileRecord.Value;

                var secondaryExtension = Path.GetExtension(path);
                path = path.Substring(0, path.Length - secondaryExtension.Length);
                var primaryExtension = Path.GetExtension(path);
                path = path.Substring(0, path.Length - primaryExtension.Length);
                if (primaryExtension.Length == 0)
                {
                    primaryExtension = secondaryExtension;
                    secondaryExtension = "";
                }

                var matchingBundle = bundles.Find(bundle => bundle.BundlePath == path && bundle.MainExtension == primaryExtension);
                if(matchingBundle != null)
                {
                    matchingBundle.AddFile(file, secondaryExtension);
                }
                else
                {
                    var newBundle = new FileBundle(primaryExtension, path);
                    newBundle.AddFile(file, secondaryExtension);
                    bundles.Add(newBundle);
                }
            }

            return bundles;
        }


        /// <summary>
        /// Opens listed files and bundles them based on their extensions.
        /// </summary>
        /// <param name="filePaths">List of file paths to open and bundle.</param>
        /// <param name="mainDirectory">
        /// Optional path to main directory. 
        /// If specified, file bundle paths will be relative to given directory.
        /// </param>
        /// <returns>List of organized file bundles.</returns>
        public static List<FileBundle> BundleFiles(IEnumerable<string> filePaths, string mainDirectory = "")
        {
            var fileList = new Dictionary<string, Stream>();
            foreach (var filePath in filePaths)
            {
                var relativePath = GetRelativePath(filePath, mainDirectory).Replace("/", "\\");
                fileList[relativePath] = File.OpenRead(filePath);
            }
            return FileBundle.BundleFiles(fileList);
        }

        /// <summary>
        /// Loads all files at given path and then bundles them into file bundles.
        /// </summary>
        /// <remarks>
        /// If the path is a directory, all files in that directory will be bundled recursively.
        /// If the path points to a single file instead, it will be bundled into a single file bundle
        /// with all of the files in the directory which belong to the same bundle.
        /// </remarks>
        /// <param name="path">Path of a file or a directory to bundle its content from.</param>
        /// <returns>List of file bundles found at given path.</returns>
        public static List<FileBundle> BundleFilesAtPath(string path)
        {
            if (Directory.Exists(path))
            {
                string[] filePaths = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                return BundleFiles(filePaths, path);
            }
            else if (File.Exists(path))
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                path = Path.GetDirectoryName(path) ?? "";
                string[] filePaths = Directory.GetFiles(path, $"{fileName}.*");
                return BundleFiles(filePaths, path);
            }
            else
            {
                throw new FileNotFoundException("Specified path does not lead to any file or a directory");
            }
        }

        /// <summary>
        /// Unpacks all of the bundle files into a list.
        /// </summary>
        /// <param name="fileBundles">File bundles to unpack</param>
        /// <returns>Dictionary containing file names paired with corresponding file streams.</returns>
        public static Dictionary<string, Stream> UnbundleFiles(List<FileBundle> fileBundles)
        {
            var files = new Dictionary<string, Stream>();

            foreach(var bundle in fileBundles)
            {
                foreach(var file in bundle.Files)
                {
                    files.Add(bundle.BundlePath + bundle.MainExtension + file.Extension, file.Data);
                }
            }

            return files;
        }
    }
}
