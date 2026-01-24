using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Todo;

namespace Todo.View
{
    public partial class Main_empty : Window
    {
        private const string DefaultPhotoFileName = "фото_умолч.jpg";
        private const string ConfigFolderName = "WeddingAgency";
        private const string PhotoPathFileName = "photo_path.txt";

        private string _currentPhotoPath;
        private BitmapImage _userPhoto;

        public BitmapImage UserPhoto
        {
            get { return _userPhoto; }
            private set { _userPhoto = value; }
        }

        public Main_empty()
        {
            InitializeComponent();
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            Loaded += HandleWindowLoaded;
            ExitButton.Click += HandleExit;
            CreateTaskButton.Click += HandleCreateTask;
            ChangePhotoButton.Click += HandleChangePhoto;
        }

        private void HandleWindowLoaded(object sender, RoutedEventArgs e)
        {
            LoadUserPhoto();
        }

        private void HandleExit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void HandleCreateTask(object sender, RoutedEventArgs e)
        {
            OpenTaskCreationWindow();
        }

        private void HandleChangePhoto(object sender, RoutedEventArgs e)
        {
            ChangeUserPhoto();
        }

        private void OpenTaskCreationWindow()
        {

            var taskCreationWindow = new Creating_tasks
            {
                Title = "Создание задачи",
                Width = 800,
                Height = 450,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };


            taskCreationWindow.TaskCreated += HandleTaskCreated;


            taskCreationWindow.Closed += (s, args) =>
            {
                taskCreationWindow.TaskCreated -= HandleTaskCreated;
            };

            taskCreationWindow.Show();
        }


        private void HandleTaskCreated(Todo.View.Main.Task newTask)
        {
            var mainWindow = new Todo.View.Main();

            mainWindow.AddNewTask(newTask);

            if (UserPhoto != null)
            {
                mainWindow.SetUserPhoto(UserPhoto);
            }

            mainWindow.Show();
            Close();
        }

        private void ChangeUserPhoto()
        {
            try
            {
                var openFileDialog = CreateImageFileDialog();
                var dialogResult = openFileDialog.ShowDialog();

                if (dialogResult == true)
                {
                    ProcessSelectedPhoto(openFileDialog.FileName);
                }
                else
                {
                    ShowInformationMessage("Выбор фотографии отменен.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при выборе фотографии: {ex.Message}");
            }
        }

        private OpenFileDialog CreateImageFileDialog()
        {
            return new OpenFileDialog
            {
                Filter = "Image Files (*.jpg; *.jpeg; *.png; *.bmp)|*.jpg; *.jpeg; *.png; *.bmp",
                FilterIndex = 1,
                Title = "Выберите фотографию"
            };
        }

        private void ProcessSelectedPhoto(string selectedImagePath)
        {
            if (!File.Exists(selectedImagePath))
            {
                ShowErrorMessage("Выбранный файл не существует!");
                return;
            }

            if (LoadAndDisplayImage(selectedImagePath))
            {
                SavePhotoPath(selectedImagePath);
                ShowSuccessMessage("Фотография успешно изменена!");
            }
        }

        private bool LoadAndDisplayImage(string imagePath)
        {
            try
            {
                var bitmapImage = CreateBitmapImage(imagePath);

                if (UserProfileImage != null)
                {
                    UserProfileImage.Source = bitmapImage;
                    _currentPhotoPath = imagePath;
                    UserPhoto = bitmapImage;
                    return true;
                }
                else
                {
                    ShowErrorMessage("Элемент для отображения фото не найден!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка загрузки изображения: {ex.Message}");
                return false;
            }
        }

        private BitmapImage CreateBitmapImage(string imagePath)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        private void LoadDefaultPhoto()
        {
            try
            {
                if (File.Exists(DefaultPhotoFileName))
                {
                    LoadAndDisplayImage(DefaultPhotoFileName);
                }
                else
                {
                    ShowInformationMessage("Файл фото_умолч.jpg не найден. Используется стандартное изображение.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки фото по умолчанию: {ex.Message}");
            }
        }

        private void SavePhotoPath(string photoPath)
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var appFolder = Path.Combine(appDataPath, ConfigFolderName);

                Directory.CreateDirectory(appFolder);

                var configFilePath = Path.Combine(appFolder, PhotoPathFileName);
                File.WriteAllText(configFilePath, photoPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения пути: {ex.Message}");
            }
        }

        private void LoadUserPhoto()
        {
            try
            {
                var savedPhotoPath = GetSavedPhotoPath();

                if (!string.IsNullOrEmpty(savedPhotoPath) && File.Exists(savedPhotoPath))
                {
                    LoadAndDisplayImage(savedPhotoPath);
                    return;
                }

                LoadDefaultPhoto();
            }
            catch (Exception ex)
            {
                LoadDefaultPhoto();
                Console.WriteLine($"Ошибка загрузки фото: {ex.Message}");
            }
        }

        private string GetSavedPhotoPath()
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var configFilePath = Path.Combine(appDataPath, ConfigFolderName, PhotoPathFileName);

                if (File.Exists(configFilePath))
                {
                    return File.ReadAllText(configFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения сохраненного пути: {ex.Message}");
            }

            return null;
        }

        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Успех!",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowInformationMessage(string message)
        {
            MessageBox.Show(message, "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class TaskItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }

        public TaskItem(string title, string description, DateTime dueDate)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            IsCompleted = false;
        }
    }
}