using System.Globalization;
using System.IO;
using System.Windows;

using FEZRepacker.Core.FileSystem;
using FEZRepacker.GUI.Windows;

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
            var status = new ProgressStatus("Opening package");

            status.Task += () =>
            {
                status.SetStageCount(1);
                status.SetStageName("Unpacking package (1/4)");

                FileBundleList = new();

                using var pakStream = File.OpenRead(path);
                using var pakReader = new PakReader(pakStream);

                int filesProcessed = 0;
                foreach (var pakFile in pakReader.ReadFiles())
                {
                    status.SetStageText($"{pakFile.Path}");
                    status.SetStageCompletion(filesProcessed / (float)pakReader.FileCount);

                    var bundle = FileBundle.Single(new MemoryStream(pakFile.Payload), ".xnb");
                    bundle.BundlePath = pakFile.Path;
                    FileBundleList.Add(bundle);

                    filesProcessed++;
                }

                CurrentPackageName = Path.GetFileName(path);
                BundleDirty = false;
            };

            status.OnComplete += () => (MainWindow as Main)?.Refresh();

            status.Run();
        }
    }
}
