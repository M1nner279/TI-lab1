using Avalonia.Controls;
using lab1.ViewModels;
using Avalonia.Media;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Input;
using Avalonia.Interactivity;

namespace lab1.Views
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
        
        private async void SaveToFileButton_Click(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                var dialog = new SaveFileDialog { };
                var result = await dialog.ShowAsync(this);
                if (result != null && result.Length > 0)
                {
                    viewModel.UploadToFile(result);
                }
            }
            
            
        }

        private async void OpenFileButton_Click(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                var dialog = new OpenFileDialog { AllowMultiple = false };
                var result = await dialog.ShowAsync(this);
                if (result != null && result.Length > 0)
                {
                    viewModel.LoadFromFile(result[0]);
                }
            }
        }
    }
}