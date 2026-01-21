using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Todo
{
    /// <summary>
    /// Класс для представления задачи
    /// </summary>
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

    /// <summary>
    /// Логика взаимодействия для Main_empty.xaml
    /// </summary>
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

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            Loaded += OnWindowLoaded;
            ExitButton.Click += OnExitButtonClick;
            CreateTaskButton.Click += OnCreateTaskButtonClick;
            ChangePhotoButton.Click += OnChangePhotoButtonClick;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            LoadUserPhoto();
        }

        private void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnCreateTaskButtonClick(object sender, RoutedEventArgs e)
        {
            CreateAndOpenTaskWindow();
        }

        private void OnChangePhotoButtonClick(object sender, RoutedEventArgs e)
        {
            ChangeUserPhoto();
        }

        private void CreateAndOpenTaskWindow()
        {
            Creating_tasks taskCreationWindow = new Creating_tasks();

           
            taskCreationWindow.TaskCreated += OnTaskCreated;
            taskCreationWindow.Show();
        }

      
        private void OnTaskCreated(Todo.Main.Task newTask)
        {
           
            Main mainWindow = new Main();


            if (mainWindow is Main main)
            {
 
                main.AddNewTask(newTask);

                if (UserPhoto != null)
                {
                    main.SetUserPhoto(UserPhoto);
                }

                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка создания главного окна", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
        private void ChangeUserPhoto()
        {
            try
            {
                OpenFileDialog openFileDialog = CreateImageFileDialog();
                bool? dialogResult = openFileDialog.ShowDialog();

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
            if (!System.IO.File.Exists(selectedImagePath))
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
                BitmapImage bitmapImage = CreateBitmapImage(imagePath);

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
            BitmapImage bitmap = new BitmapImage();

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
                if (System.IO.File.Exists(DefaultPhotoFileName))
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
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки фото по умолчанию: {ex.Message}");
            }
        }

        private void SavePhotoPath(string photoPath)
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string appFolder = System.IO.Path.Combine(appDataPath, ConfigFolderName);

                System.IO.Directory.CreateDirectory(appFolder);

                string configFilePath = System.IO.Path.Combine(appFolder, PhotoPathFileName);
                System.IO.File.WriteAllText(configFilePath, photoPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения пути: {ex.Message}");
            }
        }

        private void LoadUserPhoto()
        {
            try
            {
                string savedPhotoPath = GetSavedPhotoPath();

                if (!string.IsNullOrEmpty(savedPhotoPath) && System.IO.File.Exists(savedPhotoPath))
                {
                    LoadAndDisplayImage(savedPhotoPath);
                    return;
                }

                LoadDefaultPhoto();
            }
            catch (Exception ex)
            {
                LoadDefaultPhoto();
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки фото: {ex.Message}");
            }
        }

        private string GetSavedPhotoPath()
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string configFilePath = System.IO.Path.Combine(appDataPath, ConfigFolderName, PhotoPathFileName);

                if (System.IO.File.Exists(configFilePath))
                {
                    return System.IO.File.ReadAllText(configFilePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка чтения сохраненного пути: {ex.Message}");
            }

            return null;
        }

        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowInformationMessage(string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}