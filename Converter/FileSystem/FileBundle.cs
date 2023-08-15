namespace FEZRepacker.Converter.FileSystem
{
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
