using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.Windows;

namespace CreatoreFileVSIX
{
    public class ColumnItem
    {
        public bool IsSelected { get; set; } = true;
        public string OriginalName { get; set; }
        public string Type { get; set; }
        public string NewName { get; set; }
    }

    public partial class ColumnSelector : DialogWindow
    {
        public List<ColumnItem> Columns { get; private set; }

        public ColumnSelector() : this(new List<ColumnItem>
        {
            // Fallback for designer or if called empty
            new ColumnItem { IsSelected = true, OriginalName = "Id", Type = "int", NewName = "Id" },
            new ColumnItem { IsSelected = true, OriginalName = "Name", Type = "string", NewName = "Name" },
            new ColumnItem { IsSelected = false, OriginalName = "SecretPassword", Type = "string", NewName = "SecretPassword" },
             new ColumnItem { IsSelected = true, OriginalName = "Id", Type = "int", NewName = "Id" },
            new ColumnItem { IsSelected = true, OriginalName = "Name", Type = "string", NewName = "Name" },
            new ColumnItem { IsSelected = false, OriginalName = "SecretPassword", Type = "string", NewName = "SecretPassword" },
             new ColumnItem { IsSelected = true, OriginalName = "Id", Type = "int", NewName = "Id" },
            new ColumnItem { IsSelected = true, OriginalName = "Name", Type = "string", NewName = "Name" },
            new ColumnItem { IsSelected = false, OriginalName = "SecretPassword", Type = "string", NewName = "SecretPassword" },
             new ColumnItem { IsSelected = true, OriginalName = "Id", Type = "int", NewName = "Id" },
            new ColumnItem { IsSelected = true, OriginalName = "Name", Type = "string", NewName = "Name" },
            new ColumnItem { IsSelected = false, OriginalName = "SecretPassword", Type = "string", NewName = "SecretPassword" },
             new ColumnItem { IsSelected = true, OriginalName = "Id", Type = "int", NewName = "Id" },
            new ColumnItem { IsSelected = true, OriginalName = "Name", Type = "string", NewName = "Name" },
            new ColumnItem { IsSelected = false, OriginalName = "SecretPassword", Type = "string", NewName = "SecretPassword" },
             new ColumnItem { IsSelected = true, OriginalName = "Id", Type = "int", NewName = "Id" },
            new ColumnItem { IsSelected = true, OriginalName = "Name", Type = "string", NewName = "Name" },
            new ColumnItem { IsSelected = false, OriginalName = "SecretPassword", Type = "string", NewName = "SecretPassword" },
             new ColumnItem { IsSelected = true, OriginalName = "Id", Type = "int", NewName = "Id" },
            new ColumnItem { IsSelected = true, OriginalName = "Name", Type = "string", NewName = "Name" },
            new ColumnItem { IsSelected = false, OriginalName = "SecretPassword", Type = "string", NewName = "SecretPassword" },
             new ColumnItem { IsSelected = true, OriginalName = "Id", Type = "int", NewName = "Id" },
            new ColumnItem { IsSelected = true, OriginalName = "Name", Type = "string", NewName = "Name" },
            new ColumnItem { IsSelected = false, OriginalName = "SecretPassword", Type = "string", NewName = "SecretPassword" },
             new ColumnItem { IsSelected = true, OriginalName = "Id", Type = "int", NewName = "Id" },
            new ColumnItem { IsSelected = true, OriginalName = "Name", Type = "string", NewName = "Name" },
            new ColumnItem { IsSelected = false, OriginalName = "SecretPassword", Type = "string", NewName = "SecretPassword" },
             new ColumnItem { IsSelected = true, OriginalName = "Id", Type = "int", NewName = "Id" },
            new ColumnItem { IsSelected = true, OriginalName = "Name", Type = "string", NewName = "Name" },
            new ColumnItem { IsSelected = false, OriginalName = "SecretPassword", Type = "string", NewName = "SecretPassword" }

        })
        {
        }

        public ColumnSelector(List<ColumnItem> columns)
        {
            InitializeComponent();

            Columns = columns ?? new List<ColumnItem>();
            ColumnsDataGrid.ItemsSource = Columns;

            NextButton.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ThemedDialogButtonStyleKey);
            CancelButton.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ThemedDialogButtonStyleKey);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
