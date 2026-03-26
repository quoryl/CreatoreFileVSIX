using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using System.Collections.ObjectModel;
using System.Windows;

namespace CreatoreFileVSIX
{
    public class ProjectNode
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }
        public ObservableCollection<ProjectNode> Children { get; set; } = new ObservableCollection<ProjectNode>();
    }

    public partial class ProjectBrowser : DialogWindow
    {
        public ObservableCollection<ProjectNode> SolutionNodes { get; set; } = new ObservableCollection<ProjectNode>();
        public string SelectedPath { get; private set; }

        public ProjectBrowser()
        {
            InitializeComponent();
            this.DataContext = this;

            LoadSolution();
            
            ConfirmButton.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ThemedDialogButtonStyleKey);
            CancelSelectionButton.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ThemedDialogButtonStyleKey);
        }

        private void LoadSolution()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            DTE2 dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            if (dte == null || dte.Solution == null || !dte.Solution.IsOpen) return;

            foreach (Project proj in dte.Solution.Projects)
            {
                var node = CreateNode(proj);
                if (node != null)
                {
                    node.IsExpanded = true;
                    SolutionNodes.Add(node);
                }
            }
        }

        private ProjectNode CreateNode(Project proj)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (proj == null) return null;

            var node = new ProjectNode { Name = proj.Name, Path = proj.Name, IsExpanded = true };
            
            // Solution Folder
            if (proj.Kind == "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}")
            {
                if (proj.ProjectItems != null)
                {
                    foreach (ProjectItem item in proj.ProjectItems)
                    {
                        if (item.SubProject != null)
                        {
                            var child = CreateNode(item.SubProject);
                            if (child != null) node.Children.Add(child);
                        }
                    }
                }
            }
            else // Regular project
            {
                if (proj.ProjectItems != null)
                {
                    foreach (ProjectItem item in proj.ProjectItems)
                    {
                        var child = CreateFolderNode(item, proj.Name);
                        if (child != null) node.Children.Add(child);
                    }
                }
            }
            return node;
        }

        private ProjectNode CreateFolderNode(ProjectItem item, string parentPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (item == null) return null;

            // Physical Folder
            if (item.Kind == Constants.vsProjectItemKindPhysicalFolder)
            {
                var currentPath = parentPath + "\\" + item.Name;
                var node = new ProjectNode { Name = item.Name, Path = currentPath, IsExpanded = false };
                if (item.ProjectItems != null)
                {
                    foreach (ProjectItem childItem in item.ProjectItems)
                    {
                        var child = CreateFolderNode(childItem, currentPath);
                        if (child != null) node.Children.Add(child);
                    }
                }
                return node;
            }
            return null;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedNode = GetSelectedNode(SolutionNodes);
            if (selectedNode != null)
            {
                SelectedPath = selectedNode.Path;
                this.DialogResult = true;
            }
            else
            {
                this.DialogResult = false;
            }
            this.Close();
        }

        private ProjectNode GetSelectedNode(ObservableCollection<ProjectNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.IsSelected) return node;
                var childSelected = GetSelectedNode(node.Children);
                if (childSelected != null) return childSelected;
            }
            return null;
        }

        private void CancelSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
