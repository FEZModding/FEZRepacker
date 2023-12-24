using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using FEZRepacker.Converter.FileSystem;

namespace FEZRepacker.GUI
{
    public partial class Main : Window
    {
        private class DirectoryRecord
        {
            public Dictionary<string, DirectoryRecord> ContainedDirectories = new();
            public FileBundle? LinkedBundle = null;
            public bool Opened = false;
        }

        private readonly string initialTitle;
        private readonly App app;

        private DirectoryRecord mainDirectory = new();

        public Main()
        {
            InitializeComponent();
            app = (Application.Current as App)!;
            initialTitle = Title;

            Refresh();
        }

        public void Refresh()
        {
            RefreshTitle();
            RefreshList();
            RefreshFileListDisplay();
        }

        private void RefreshTitle()
        {
            Title = app.CurrentPackageName;
            if (app.BundleDirty) Title += "*";
            Title += " - " + initialTitle;
        }

        private void RefreshList()
        {
            var fileBundles = app.FileBundleList;

            mainDirectory = new();
            foreach (var bundle in fileBundles)
            {
                var dirs = bundle.BundlePath.Split("\\");
                var currentDirRecord = mainDirectory;
                foreach(var dir in dirs)
                {
                    if (!currentDirRecord.ContainedDirectories.ContainsKey(dir))
                    {
                        currentDirRecord.ContainedDirectories.Add(dir, new());
                    }
                    currentDirRecord = currentDirRecord.ContainedDirectories[dir];
                }
                currentDirRecord.LinkedBundle = bundle;
            }
        }

        private void RefreshFileListDisplay()
        {
            fileList.Children.Clear();
            AddRecordToList(app.CurrentPackageName, mainDirectory);
        }

        private void AddRecordToList(string name, DirectoryRecord record, int depth = 0)
        {
            Button recordButton = new Button();
            recordButton.Content = name;

            var styleName = "FileButton";
            if(record.LinkedBundle == null)
            {
                styleName = record.Opened ? "OpenDirectoryFileButton" : "DirectoryFileButton";
            }
            recordButton.Style = (Style)fileList.FindResource(styleName);

            recordButton.Padding = new Thickness(depth * 10, 0, 0, 0);

            recordButton.Click += (sender, e) =>
            {
                record.Opened = !record.Opened;
                RefreshFileListDisplay();
            };

            fileList.Children.Add(recordButton);

            if (record.Opened)
            {
                foreach(var dir in record.ContainedDirectories)
                {
                    AddRecordToList(dir.Key, dir.Value, depth + 1);
                }
            }
        }


        /* Commands callbacks */

        private void NewPackage(object sender, ExecutedRoutedEventArgs e)
        {
            app.CreateEmptyPackage();
        }

        private void OpenPackage(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog()
            {
                FileName = "Package",
                DefaultExt = ".pak",
                Filter = "Package files (.pak)|*.pak",
                Multiselect = false
            };

            bool? result = dialog.ShowDialog();

            if (result == false) return;

            string filename = dialog.FileName;
            app.OpenPackage(filename);
        }

        private void ImportDirectory(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void SavePackage(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void SavePackageAs(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Export(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Exit(object sender, ExecutedRoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void AddFiles(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void AddDirectory(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Undo(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Redo(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void SelectAll(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void DeleteSelected(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void Preferences(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ReportIssue(object sender, ExecutedRoutedEventArgs e)
        {
            OpenURL("https://github.com/FEZModding/FEZRepacker/issues");
        }

        private void CheckUpdates(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void OpenDocs(object sender, ExecutedRoutedEventArgs e)
        {
            OpenURL("https://github.com/FEZModding/FEZRepacker/wiki");

        }

        private void JoinDiscord(object sender, ExecutedRoutedEventArgs e)
        {
            OpenURL("https://discord.gg/wwVB86HhJz");
        }

        private void ShowAbout(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void OpenURL(string url)
        {
            var sInfo = new System.Diagnostics.ProcessStartInfo(url)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
        }
    }

    public static class Commands
    {
        // File Menu

        public static readonly RoutedUICommand CreateEmpty = new(
            "_Create Empty", "CreateEmpty", typeof(Commands),
            new() { new KeyGesture(Key.N, ModifierKeys.Control) }
        );

        public static readonly RoutedUICommand Open = new(
            "_Open Package", "Open", typeof(Commands),
            new() { new KeyGesture(Key.O, ModifierKeys.Control) }
        );

        public static readonly RoutedUICommand ImportDirectory = new(
            "_Import Directory", "ImportDirectory", typeof(Commands),
            new() { new KeyGesture(Key.I, ModifierKeys.Control) }
        );

        public static readonly RoutedUICommand Save = new(
            "_Save", "Save", typeof(Commands),
            new() { new KeyGesture(Key.S, ModifierKeys.Control) }
        );

        public static readonly RoutedUICommand SaveAs = new(
            "Save _As...", "SaveAs", typeof(Commands),
            new() { new KeyGesture(Key.S, ModifierKeys.Alt) }
        );

        public static readonly RoutedUICommand Export = new(
            "_Export Content", "Export", typeof(Commands),
            new() { new KeyGesture(Key.E, ModifierKeys.Control) }
        );

        public static readonly RoutedUICommand Exit = new(
            "_Exit", "Exit", typeof(Commands),
            new() { new KeyGesture(Key.F4, ModifierKeys.Alt) }
        );

        // Edit Menu

        public static readonly RoutedUICommand AddFiles = new(
            "Add _Files", "AddFiles", typeof(Commands)
        );

        public static readonly RoutedUICommand AddDirectory = new(
            "Add _Directory", "AddDirectory", typeof(Commands)
        );

        public static readonly RoutedUICommand Undo = new(
            "_Undo", "Undo", typeof(Commands),
            new() { new KeyGesture(Key.Z, ModifierKeys.Control) }
        );

        public static readonly RoutedUICommand Redo = new(
            "_Redo", "Redo", typeof(Commands),
            new() { new KeyGesture(Key.Y, ModifierKeys.Control) }
        );

        public static readonly RoutedUICommand SelectAll = new(
            "_Select All", "SelectAll", typeof(Commands),
            new() { new KeyGesture(Key.A, ModifierKeys.Control) }
        );

        public static readonly RoutedUICommand DeleteSelected = new(
            "_Delete Selected", "DeleteSelected", typeof(Commands),
            new() { new KeyGesture(Key.Delete) }
        );

        public static readonly RoutedUICommand Preferences = new(
            "_Preferences", "Preferences", typeof(Commands)
        );

        // Help Menu

        public static readonly RoutedUICommand ReportIssue = new(
            "_Report An Issue", "ReportIssue", typeof(Commands)
        );

        public static readonly RoutedUICommand CheckUpdates = new(
            "_Check For Updates", "CheckUpdates", typeof(Commands)
        );

        public static readonly RoutedUICommand OpenDocs = new(
            "_Open Documentation", "OpenDocs", typeof(Commands)
        );

        public static readonly RoutedUICommand JoinDiscord = new(
            "_Join FEZ: Community Projects Discord", "JoinDiscord", typeof(Commands)
        );

        public static readonly RoutedUICommand ShowAbout = new(
            "_About FEZ Repacker", "ShowAbout", typeof(Commands),
            new() { new KeyGesture(Key.F1) }
        );
    }
}