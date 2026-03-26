using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System;
using System.Runtime.InteropServices;

namespace CreatoreFileVSIX
{
    public class RelayCommand : ICommand
    {
        private Action<object> _execute;

        public RelayCommand(Action<object> execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute(parameter);
    }

    public class TemplateConfig : INotifyPropertyChanged
    {
        private string _selectedTemplate;
        public string SelectedTemplate
        {
            get => _selectedTemplate;
            set { _selectedTemplate = value; OnPropertyChanged(nameof(SelectedTemplate)); }
        }

        private string _targetPath;
        public string TargetPath
        {
            get => _targetPath;
            set { _targetPath = value; OnPropertyChanged(nameof(TargetPath)); }
        }

        private bool _createSubfolder;
        public bool CreateSubfolder
        {
            get => _createSubfolder;
            set { _createSubfolder = value; OnPropertyChanged(nameof(CreateSubfolder)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public partial class TemplateSelector : DialogWindow
    {
        public ObservableCollection<TemplateConfig> Cards { get; set; } = new ObservableCollection<TemplateConfig>();
        public List<string> AvailableTemplates { get; set; } = new List<string> { "Controller", "Service", "Repository", "Entity", "Model", "View" };

        public ICommand RemoveCardCommand { get; }
        public ICommand BrowsePathCommand { get; }

        public TemplateSelector()
        {
            InitializeComponent();
            
            RemoveCardCommand = new RelayCommand(param => {
                if (param is TemplateConfig card)
                    Cards.Remove(card);
            });

            BrowsePathCommand = new RelayCommand(param => {
                if (param is TemplateConfig card)
                {
                    ThreadHelper.ThrowIfNotOnUIThread();

                    var dialog = new ProjectBrowser();
                    if (dialog.ShowModal() == true)
                    {
                        card.TargetPath = dialog.SelectedPath;
                    }
                }
            });

            CardsContainer.ItemsSource = Cards;
            this.DataContext = this;

            AddButton.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ThemedDialogButtonStyleKey);
            ResetButton.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ThemedDialogButtonStyleKey);
            LoadDefaultButton.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ThemedDialogButtonStyleKey);
            NextButton.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ThemedDialogButtonStyleKey);
            CancelButton.SetResourceReference(FrameworkElement.StyleProperty, VsResourceKeys.ThemedDialogButtonStyleKey);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Cards.Add(new TemplateConfig { CreateSubfolder = true });
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Cards.Clear();
        }

        private void LoadDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            Cards.Clear();
            if (AvailableTemplates.Count > 0)
            {
                Cards.Add(new TemplateConfig { SelectedTemplate = "Controller", CreateSubfolder = true, TargetPath = "Progetto.Controllers" });
                Cards.Add(new TemplateConfig { SelectedTemplate = "Service", CreateSubfolder = true, TargetPath = "Progetto.Services" });
                Cards.Add(new TemplateConfig { SelectedTemplate = "Repository", CreateSubfolder = true, TargetPath = "Progetto.Proxies" });
            }
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
