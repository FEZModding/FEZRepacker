namespace FEZRepacker.Converter.FileSystem
{
    public sealed class FileBundle : List<(string Extension, Stream Data)>, IDisposable
    {
        public string BundlePath { get; set; }
        public string MainExtension { get; set; }

        public FileBundle() { }

        public FileBundle(string extension, string path = "")
        {
            MainExtension = extension;
            BundlePath = path;
        }

        public Stream GetData(string extension = "")
        {
            foreach(var record in this)
            {
                if (record.Extension == extension) return record.Data;
            }
            return new MemoryStream();
        }

        public HashSet<string> GetSubExtensions()
        {
            var extSet = new HashSet<string>();
            foreach(var item in this) extSet.Add(item.Extension);
            return extSet;
        }

        public void Dispose()
        {
            foreach (var file in this)
            {
                file.Data.Dispose();
            }
            this.Clear();
        }

        public static FileBundle Single(Stream stream, string extension="", string subextension = "")
        {
            return new FileBundle(extension) { (subextension, stream) };
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
                    matchingBundle.Add((secondaryExtension, file));
                }
                else
                {
                    bundles.Add(new FileBundle(primaryExtension, path)
                    {
                        (secondaryExtension, file)
                    });
                }
            }

            return bundles;
        }

        public static Dictionary<string, Stream> UnbundleFiles(List<FileBundle> fileBundles)
        {
            var files = new Dictionary<string, Stream>();

            foreach(var bundle in fileBundles)
            {
                foreach(var file in bundle)
                {
                    files.Add(bundle.BundlePath + bundle.MainExtension + file.Extension, file.Data);
                }
            }

            return files;
        }
    }
}
