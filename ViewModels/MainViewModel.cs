using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using lab1.Services;

namespace lab1.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _inputText = "";
        private string _resultText;
        private string _key1 = "";
        private bool _isKeyVisible;
        private bool _isTurningMethod = true;
        private static List<string> _algorithms = ["Метод поворачивающейся решетки", "Шифр Виженера"];
        private string _selectedAlgorithm = _algorithms.First();
        public event PropertyChangedEventHandler? PropertyChanged;
        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value; 
                OnPropertyChanged(nameof(InputText));
                UpdateCommandState();
            }
        }
        public string ResultText
        {
            get => _resultText;
            set { _resultText = value; OnPropertyChanged(nameof(ResultText)); }
        }
        public string Key1
        {
            get => _key1;
            set { 
                _key1 = value; 
                OnPropertyChanged(nameof(Key1)); 
                UpdateCommandState();
            }
        }
        public bool IsKeyVisible
        {
            get => _isKeyVisible;
            private set
            {
                _isKeyVisible = value;
                OnPropertyChanged(nameof(IsKeyVisible));
                UpdateCommandState();
            }
        }
        private void UpdateCommandState()
        {
            ((RelayCommand)EncryptCommand).RaiseCanExecuteChanged();
            ((RelayCommand)DecryptCommand).RaiseCanExecuteChanged();
        }
        public bool IsTurningMethod
        {
            get => _isTurningMethod;
            set { _isTurningMethod = value; OnPropertyChanged(nameof(IsTurningMethod)); }
        }
        public List<string> Algorithms
        {
            get => _algorithms;
            set { _algorithms = value; OnPropertyChanged(nameof(Algorithms)); }
        }
        public string SelectedAlgorithm
        {
            get => _selectedAlgorithm;
            set
            {
                _selectedAlgorithm = value;
                OnPropertyChanged(nameof(SelectedAlgorithm));
                OnPropertyChanged(nameof(SelectedAlgorithmInfo));
                IsTurningMethod = _selectedAlgorithm == _algorithms[0];
                IsKeyVisible = !IsTurningMethod;
            }
        }
        public string SelectedAlgorithmInfo => $"Выбран алгоритм: {SelectedAlgorithm}";
        public ICommand EncryptCommand { get; }
        public ICommand DecryptCommand { get; }

        public MainViewModel()
        {
            EncryptCommand = new RelayCommand(_ => Encrypt(), _ => CanEncryptDecrypt());
            DecryptCommand = new RelayCommand(_ => Decrypt(), _ => CanEncryptDecrypt());
        }
        public ICommand SaveFileCommand => new RelayCommand(_ => SaveToFile());
        public ICommand OpenFileCommand => new RelayCommand(_ => OpenFile());
        private bool CanEncryptDecrypt()
        {
            if (!IsKeyVisible)
                return !string.IsNullOrWhiteSpace(InputText);
            return !string.IsNullOrWhiteSpace(Key1) && !string.IsNullOrWhiteSpace(InputText);
            // сюда добавить проверки на правильный ввод
        }
        public void Encrypt()
        {
            if (IsTurningMethod)
            {
                ResultText = TurnCipher.Encrypt(InputText);
            }
            else
            {
                StringBuilder newkey = new StringBuilder();
                foreach (char c in Key1)
                {
                    int index = VigenereCipher.alphabet.IndexOf(char.ToUpper(c));
                    if (index == -1) continue; // Пропускаем неалфавитные символы
                    newkey.Append(char.ToUpper(c));
                }
                Key1 = newkey.ToString(); 
                newkey = new StringBuilder();
                foreach (char c in InputText)
                {
                    int index = VigenereCipher.alphabet.IndexOf(char.ToUpper(c));
                    if (index == -1) continue; // Пропускаем неалфавитные символы
                    newkey.Append(char.ToUpper(c));
                }
                ResultText = VigenereCipher.Encrypt(newkey.ToString(), Key1);
            }
        }
        public void Decrypt()
        {
            if (IsTurningMethod)
            {
                // ResultText = ColumnCipher.Decrypt(InputText, Key1, Key2);
                ResultText = TurnCipher.Decrypt(InputText);
            }
            else
            {
                StringBuilder newkey = new StringBuilder();
                foreach (char c in Key1)
                {
                    int index = VigenereCipher.alphabet.IndexOf(char.ToUpper(c));
                    if (index == -1) continue; // Пропускаем неалфавитные символы
                    newkey.Append(char.ToUpper(c));
                }
                Key1 = newkey.ToString(); 
                newkey = new StringBuilder();
                foreach (char c in InputText)
                {
                    int index = VigenereCipher.alphabet.IndexOf(char.ToUpper(c));
                    if (index == -1) continue; // Пропускаем неалфавитные символы
                    newkey.Append(char.ToUpper(c));
                }
                ResultText = VigenereCipher.Decrypt(newkey.ToString(), Key1);
            }
        }
        public async void SaveToFile()
        {
            if (string.IsNullOrWhiteSpace(ResultText))
            {
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Text Files", Extensions = new List<string> { "txt" } },
                    new FileDialogFilter { Name = "All Files", Extensions = new List<string> { "*" } }
                },
                DefaultExtension = "txt",
                InitialFileName = "result.txt"
            };

            var result = await dialog.ShowAsync(new Window());
            if (result != null)
            {
                UploadToFile(result);
            }
        }

        public void UploadToFile(string filePath)
        {
            try
            {
                File.WriteAllText(filePath, ResultText, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                // Обработка ошибок (например, вывод в лог или сообщение пользователю)
                Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
            }
        }

        public void LoadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                InputText = File.ReadAllText(filePath, Encoding.UTF8);
            }
        }

        private async void OpenFile()
        {
            var dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Text Files", Extensions = new List<string> { "txt" } },
                    new FileDialogFilter { Name = "All Files", Extensions = new List<string> { "*" } }
                }
            };

            var result = await dialog.ShowAsync(new Window());
            if (result != null && result.Length > 0)
            {
                LoadFromFile(result[0]);
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == nameof(InputText) || propertyName == nameof(Key1))
            {
                ((RelayCommand)EncryptCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DecryptCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}