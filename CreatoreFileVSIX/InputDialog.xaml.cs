using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using System.Windows;

namespace CreatoreFileVSIX
{
    public partial class InputDialog : DialogWindow
    {
        public string InputText => InputTextBox.Text.Trim();

        public InputDialog(string title, string message)
        {
            InitializeComponent();
            this.Title = title;
            this.MessageText.Text = message;
            
            this.Loaded += (s, e) => { InputTextBox.Focus(); };
            
            MessageText.SetResourceReference(System.Windows.Controls.TextBlock.ForegroundProperty, EnvironmentColors.ToolWindowTextBrushKey);
            InputTextBox.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.TextBoxStyleKey);
            ConfirmButton.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ButtonStyleKey);
            CancelButton.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ButtonStyleKey);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
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
