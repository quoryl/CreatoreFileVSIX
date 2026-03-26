using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CreatoreFileVSIX
{
    public partial class TableSelector : DialogWindow
    {
        public string SelectedTableName { get; private set; }
        public string ConnectionString => ConnectionStringTextBox.Text.Trim();
        private bool IsManualMode = false;
        public TableSelector()
        {
            InitializeComponent();

            this.Loaded += (s, e) => { ConnectionStringTextBox.Focus(); };

            ConnectionStringTextBox.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ThemedDialogTextBoxStyleKey);
            ManualTableNameTextBox.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ThemedDialogTextBoxStyleKey);
            TablesComboBox.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ThemedDialogComboBoxStyleKey);
            
            ConnectButton.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ThemedDialogButtonStyleKey);
            ModeToggleButton.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ThemedDialogButtonStyleKey);
            NextButton.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ThemedDialogButtonStyleKey);
            CancelButton.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ThemedDialogButtonStyleKey);

            UpdateNextButtonState();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder: qui andrà la logica per connettersi al db e popolare la combobox
            var dummyTables = new List<string> { "Users", "Products", "Orders" };
            TablesComboBox.ItemsSource = dummyTables;
            if (dummyTables.Count > 0)
                TablesComboBox.SelectedIndex = 0;
        }

        private void ModeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            IsManualMode = !IsManualMode;
            if (IsManualMode)
            {
                ModeToggleButton.Content = "Switch to Selection mode";
                ManualPanel.Visibility = Visibility.Visible;
                SelectionPanel.Visibility = Visibility.Collapsed;
                SelectedTableName = ManualTableNameTextBox.Text.Trim();
            }
            else
            {
                ModeToggleButton.Content = "Switch to Manual entry";
                ManualPanel.Visibility = Visibility.Collapsed;
                SelectionPanel.Visibility = Visibility.Visible;
                SelectedTableName = TablesComboBox.SelectedItem as string;
            }
            UpdateNextButtonState();
        }

        private void TableName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsManualMode)
            {
                SelectedTableName = ManualTableNameTextBox.Text.Trim();
                UpdateNextButtonState();
            }
        }

        private void TablesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsManualMode)
            {
                SelectedTableName = TablesComboBox.SelectedItem as string;
                UpdateNextButtonState();
            }
        }

        private void UpdateNextButtonState()
        {
            NextButton.IsEnabled = !string.IsNullOrWhiteSpace(SelectedTableName);
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
