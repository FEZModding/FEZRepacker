using System.Globalization;
using System.IO;
using System.Windows;
using FEZRepacker.Core.FileSystem;

namespace FEZRepacker.GUI
{
    public partial class App : Application
    {
        public string CurrentPackageName = "";
        public PakPackage CurrentPackage = new();
        public bool PackageDirty = false;

        public Main MainAppWindow => (Main)MainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");

            base.OnStartup(e);
            CreateEmptyPackage();
        }

        public void CreateEmptyPackage()
        {
            CurrentPackage = new PakPackage();
            CurrentPackageName = "untitled.pak";
            PackageDirty = false;
            RefreshMainWindow();
        }

        public void OpenPackage(string path)
        {
            using var pakStream = File.OpenRead(path);
            CurrentPackage = new PakPackage(pakStream);
            PackageDirty = false;
            RefreshMainWindow();
        }

        void RefreshMainWindow()
        {
            if (MainWindow is Main mainWindow)
            {
                mainWindow.Refresh();
            }
        }
    }
}
