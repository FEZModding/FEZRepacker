using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows;

using FEZRepacker.Converter.FileSystem;

namespace FEZRepacker.GUI
{
    public partial class App : Application
    {
        public string CurrentPackageName = "";
        public List<FileBundle> FileBundleList = new();
        public bool BundleDirty = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");

            base.OnStartup(e);
            CreateEmptyPackage();
        }


        public void CreateEmptyPackage()
        {
            FileBundleList = new();
            CurrentPackageName = "untitled.pak";
            BundleDirty = false;

            (MainWindow as Main)?.Refresh();
        }

        public void OpenPackage(string path)
        {
            FileBundleList = new();

            using var pakStream = File.OpenRead(path);
            using var pakReader = new PakReader(pakStream);

            foreach (var pakFile in pakReader.ReadFiles())
            {
                var bundle = FileBundle.Single(pakFile.Data, ".xnb");
                bundle.BundlePath = pakFile.Path;
                FileBundleList.Add(bundle);
            }

            CurrentPackageName = Path.GetFileName(path);
            BundleDirty = false;

            (MainWindow as Main)?.Refresh();
        }
    }
}
