using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using FEZRepacker.Core.FileSystem;

namespace FEZRepacker.GUI.Views
{
    internal class PakFileTreeView
    {
        private class TreeNode
        {
            public readonly TreeNode? Parent;
            public readonly string Name = "";
            public readonly int Depth;

            public readonly List<TreeNode> Entries = new();

            public WeakReference<PakFileRecord>? LinkedFile;
            public bool Expanded = false;

            public TreeNode(TreeNode? parent, string name)
            {
                Parent = parent;
                Depth = parent?.Depth + 1 ?? 0;
                Name = name;
            }

            public TreeNode GetOrCreateEntry(string name)
            {
                var node = Entries.Find(n => n.Name == name);
                if (node == null)
                {
                    node = new TreeNode(this, name);
                    Entries.Add(node);
                }
                return node;
            }
        }

        private struct TreeNodeView
        {
            public TreeNode Model;

            public StackPanel MainView;
            public Button Button;
            public StackPanel EntriesGroup;
        }

        private readonly StackPanel FileListPanel;

        private readonly Style baseFileButtonStyle;
        private readonly Style openFileButtonStyle;
        private readonly Style closedFileButtonStyle;

        private TreeNode rootNode = new(null, "");
        private TreeNode? selectedNode = null;

        private readonly Dictionary<TreeNode, TreeNodeView> nodeViews = new();

        public PakFileTreeView(StackPanel fileListPanel)
        {
            FileListPanel = fileListPanel;

            baseFileButtonStyle = (Style)FileListPanel.FindResource("FileButton");
            openFileButtonStyle = (Style)FileListPanel.FindResource("OpenDirectoryFileButton");
            closedFileButtonStyle = (Style)FileListPanel.FindResource("DirectoryFileButton");
        }

        public void Refresh(PakPackage pakPackage, string pakName)
        {
            RecreateTree(pakPackage, pakName);
            RefreshTreeView();
        }

        private void RecreateTree(PakPackage pakPackage, string pakName)
        {
            rootNode = new TreeNode(null, pakName);
            if(pakPackage.Entries.Count > 0)
            {
                rootNode.Expanded = true;
            }

            foreach (var entry in pakPackage.Entries)
            {
                CreateBranchForFile(entry);
            }
        }

        private void CreateBranchForFile(PakFileRecord record)
        {
            var dirs = record.Path.Split("\\");
            var currentNode = rootNode;
            foreach (var dir in dirs)
            {
                currentNode = currentNode.GetOrCreateEntry(dir);
            }
            currentNode.LinkedFile = new(record);
        }

        private void RefreshTreeView()
        {
            FileListPanel.Children.Clear();
            RefreshNodeBranch(rootNode);
        }

        private void RefreshNodeBranch(TreeNode node)
        {
            if (!nodeViews.ContainsKey(node))
            {
                CreateNodeView(node);
            }

            UpdateNodeView(node);

            if (node.Expanded)
            {
                foreach (var childNode in node.Entries)
                {
                    RefreshNodeBranch(childNode);
                }
            }
        }

        private void CreateNodeView(TreeNode node)
        {
            var button = new Button();
            button.Content = node.Name;
            button.Click += (sender, e) => OnFileViewClicked(node);

            var nodeView = new StackPanel();
            var entriesGroup = new StackPanel();

            nodeView.Children.Add(button);
            nodeView.Children.Add(entriesGroup);

            if(node.Parent == null)
            {
                FileListPanel.Children.Add(nodeView);
            }
            else
            {
                var parentEntriesGroup = nodeViews[node.Parent].EntriesGroup;
                var insertIndex = node.Parent.Entries.IndexOf(node);
                parentEntriesGroup.Children.Insert(insertIndex, nodeView);
            }

            nodeViews.Add(node, new()
            {
                Model = node,
                MainView = nodeView,
                Button = button,
                EntriesGroup = entriesGroup,
            });
        }

        private void UpdateNodeView(TreeNode node)
        {
            if (!nodeViews.TryGetValue(node, out var view))
            {
                return;
            }

            view.Button.Padding = new Thickness(node.Depth * 10, 0, 0, 0);

            view.Button.Style = node switch
            {
                { LinkedFile: not null } => baseFileButtonStyle,
                { Expanded: true } => openFileButtonStyle,
                { Expanded: false } => closedFileButtonStyle,
            };

            if (node == selectedNode)
            {
                view.Button.Background = new SolidColorBrush(new Color() { R = 255, G = 255, B = 255, A = 64 });
            }
            else
            {
                view.Button.ClearValue(Button.BackgroundProperty);
            }

            view.EntriesGroup.Visibility = node.Expanded switch
            {
                true => Visibility.Visible,
                false => Visibility.Collapsed
            };
        }

        void OnFileViewClicked(TreeNode node)
        {
            node.Expanded = !node.Expanded;
            var oldSelectedNode = selectedNode;
            selectedNode = node;
            if(oldSelectedNode != null && oldSelectedNode != node)
            {
                UpdateNodeView(oldSelectedNode);
            }

            RefreshNodeBranch(node);
        }
    }
}
