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
using System.IO;

namespace Todo
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        private string _currentSelectedTaskId;
        private Dictionary<string, Task> _tasks;
        private Dictionary<string, Task> _completedTasks;
        private string _currentCategory = "Все";
        private bool _isHistoryMode = false;
        private BitmapImage _userPhoto;

        public class Task
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Time { get; set; }
            public string Date { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public bool IsCompleted { get; set; }
            public Border TaskBorder { get; set; }
            public CheckBox TaskCheckBox { get; set; }
            public TextBlock TitleTextBlock { get; set; }
        }

        public Main()
        {
            InitializeComponent();
            InitializeTasks();
            LoadTasks();
            LoadSavedPhoto();
        }

        public void SetUserPhoto(BitmapImage photo)
        {
            _userPhoto = photo;
            UpdatePhotoDisplay();
        }

        private void UpdatePhotoDisplay()
        {
            if (UserPhotoImage != null && _userPhoto != null)
            {
                UserPhotoImage.Source = _userPhoto;
            }
        }


        private void LoadSavedPhoto()
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string configFile = System.IO.Path.Combine(appDataPath, "WeddingAgency", "photo_path.txt");

                if (File.Exists(configFile))
                {
                    string savedPath = File.ReadAllText(configFile);
                    if (File.Exists(savedPath))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(savedPath);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();

                        _userPhoto = bitmap;
                        UpdatePhotoDisplay();
                        return;
                    }
                }

                LoadDefaultPhoto();
            }
            catch (Exception ex)
            {
                LoadDefaultPhoto();
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки фото: {ex.Message}");
            }
        }

        private void LoadDefaultPhoto()
        {
            try
            {
                string defaultPhotoPath = "фото_умолч.jpg";

                if (File.Exists(defaultPhotoPath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(defaultPhotoPath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    _userPhoto = bitmap;
                    UpdatePhotoDisplay();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки фото по умолчанию: {ex.Message}");
            }
        }

        // Обработчик события создания задачи
        private void OnTaskCreated(Todo.Main.Task newTask)
        {
            _tasks[newTask.Id] = newTask;
            LoadTasks(_currentCategory);

     
            _currentSelectedTaskId = newTask.Id;
            ShowTaskDetails(newTask);

            MessageBox.Show($"Задача '{newTask.Title}' добавлена в список!", "Успех",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void InitializeTasks()
        {
            _tasks = new Dictionary<string, Task>();
            _completedTasks = new Dictionary<string, Task>();
        }

        // Загрузка задач
        private void LoadTasks(string category = "Все")
        {
            TasksPanel.Children.Clear();
            _currentCategory = category;
            _isHistoryMode = false;

            UpdateCategoryButtons(category);
            UpdateHeaderText("Задачи");

            var tasksToShow = category == "Все"
                ? _tasks.Values
                : _tasks.Values.Where(t => t.Category == category);

            if (!tasksToShow.Any())
            {
                var noTasksText = new TextBlock
                {
                    Text = "Нет задач",
                    FontSize = 16,
                    Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                };
                TasksPanel.Children.Add(noTasksText);
                return;
            }

            foreach (var task in tasksToShow)
            {
                var taskBorder = CreateTaskBorder(task);
                TasksPanel.Children.Add(taskBorder);
            }
        }

        // История
        private void LoadHistory(string category = "Все")
        {
            TasksPanel.Children.Clear();
            _isHistoryMode = true;

            UpdateCategoryButtons(category);
            UpdateHeaderText("История задач");

            var completedTasksToShow = category == "Все"
                ? _completedTasks.Values
                : _completedTasks.Values.Where(t => t.Category == category);

            if (completedTasksToShow.Count() == 0)
            {
                var noTasksText = new TextBlock
                {
                    Text = "Нет выполненных задач",
                    FontSize = 16,
                    Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                };
                TasksPanel.Children.Add(noTasksText);
                return;
            }

            foreach (var task in completedTasksToShow)
            {
                var taskBorder = CreateHistoryTaskBorder(task);
                TasksPanel.Children.Add(taskBorder);
            }
        }

        // Перемещение между списками
        private void MoveTaskToHistory(Task task)
        {
            _tasks.Remove(task.Id);
            _completedTasks[task.Id] = task;

            if (!_isHistoryMode)
            {
                LoadTasks(_currentCategory);
            }
            else
            {
                LoadHistory(_currentCategory);
            }

            if (_currentSelectedTaskId == task.Id)
            {
                _currentSelectedTaskId = null;
                TaskDetailsContent.Visibility = Visibility.Collapsed;
                SelectedTaskTitle.Text = "Выберите задачу";
            }
        }

        // Когда задача выполнена
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isHistoryMode && sender is CheckBox checkBox)
            {
                var task = FindTaskByCheckBox(checkBox);
                if (task != null)
                {
                    task.IsCompleted = true;
                    ApplyCompletedStyle(task);
                    MoveTaskToHistory(task);

                    MessageBox.Show($"Задача '{task.Title}' выполнена и перемещена в историю!", "Задача выполнена",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        // Отметка выполненной
        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (!_isHistoryMode && !string.IsNullOrEmpty(_currentSelectedTaskId) && _tasks.ContainsKey(_currentSelectedTaskId))
            {
                var task = _tasks[_currentSelectedTaskId];
                task.IsCompleted = true;
                MoveTaskToHistory(task);

                MessageBox.Show($"Задача '{task.Title}' отмечена как выполненная и перемещена в историю!", "Готово",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

  
        private void UpdateHeaderText(string text)
        {
            SelectedTaskTitle.Text = text;
            TaskDetailsContent.Visibility = Visibility.Collapsed;
        }

        private void UpdateCategoryButtons(string activeCategory)
        {
            ShowCategoryButtons();
            ResetCategoryButtons();

            switch (activeCategory)
            {
                case "Дом":
                    DomButton.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255));
                    DomButton.BorderBrush = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                    break;
                case "Работа":
                    RabotaButton.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255));
                    RabotaButton.BorderBrush = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                    break;
                case "Учеба":
                    UchebaButton.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255));
                    UchebaButton.BorderBrush = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                    break;
                case "Отдых":
                    OtdyhButton.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255));
                    OtdyhButton.BorderBrush = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                    break;
            }
        }

        private void ShowCategoryButtons()
        {
            DomButton.Visibility = Visibility.Visible;
            RabotaButton.Visibility = Visibility.Visible;
            UchebaButton.Visibility = Visibility.Visible;
            OtdyhButton.Visibility = Visibility.Visible;
        }

 
        private void ResetCategoryButtons()
        {
            DomButton.Background = Brushes.White;
            DomButton.BorderBrush = Brushes.White;

            RabotaButton.Background = Brushes.White;
            RabotaButton.BorderBrush = Brushes.White;

            UchebaButton.Background = Brushes.White;
            UchebaButton.BorderBrush = Brushes.White;

            OtdyhButton.Background = Brushes.White;
            OtdyhButton.BorderBrush = Brushes.White;
        }

        // Переключение категорий
        private void DomButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isHistoryMode)
                LoadHistory("Дом");
            else
                LoadTasks("Дом");
        }

        private void RabotaButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isHistoryMode)
                LoadHistory("Работа");
            else
                LoadTasks("Работа");
        }

        private void UchebaButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isHistoryMode)
                LoadHistory("Учеба");
            else
                LoadTasks("Учеба");
        }

        private void OtdyhButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isHistoryMode)
                LoadHistory("Отдых");
            else
                LoadTasks("Отдых");
        }

        private void VseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isHistoryMode)
                LoadHistory("Все");
            else
                LoadTasks("Все");
        }

        // Создание элемента задачи
        private Border CreateTaskBorder(Task task)
        {
            var border = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(15),
                Width = 270,
                Tag = task.Id
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var checkBox = new CheckBox
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                IsChecked = false
            };

            checkBox.Checked += CheckBox_Checked;
            checkBox.Template = CreateCheckBoxTemplate();

            var stackPanel = new StackPanel();

            var titleTextBlock = new TextBlock
            {
                Text = task.Title,
                FontWeight = FontWeights.SemiBold,
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(51, 51, 51))
            };

            var timeTextBlock = new TextBlock
            {
                Text = $"{task.Time} • {task.Date}",
                Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
                FontSize = 12,
                Margin = new Thickness(0, 5, 0, 0)
            };

            stackPanel.Children.Add(titleTextBlock);
            stackPanel.Children.Add(timeTextBlock);

            Grid.SetColumn(checkBox, 0);
            Grid.SetColumn(stackPanel, 1);

            grid.Children.Add(checkBox);
            grid.Children.Add(stackPanel);

            border.Child = grid;
            border.MouseLeftButtonDown += (s, e) => Task_MouseLeftButtonDown(s, e, task);

            task.TaskBorder = border;
            task.TaskCheckBox = checkBox;
            task.TitleTextBlock = titleTextBlock;

            return border;
        }

        private Border CreateHistoryTaskBorder(Task task)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(245, 245, 245)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(15),
                Width = 270,
                Tag = task.Id
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var checkBox = new CheckBox
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                IsChecked = true,
                Template = CreateCheckBoxTemplate()
            };

            var stackPanel = new StackPanel();

            var titleTextBlock = new TextBlock
            {
                Text = task.Title,
                FontWeight = FontWeights.SemiBold,
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150)),
                TextDecorations = TextDecorations.Strikethrough
            };

            var detailsTextBlock = new TextBlock
            {
                Text = $"{task.Time} • {task.Date} • {task.Category}",
                Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150)),
                FontSize = 12,
                Margin = new Thickness(0, 5, 0, 0)
            };

            stackPanel.Children.Add(titleTextBlock);
            stackPanel.Children.Add(detailsTextBlock);

            var statusTextBlock = new TextBlock
            {
                Text = "✓ Выполнено",
                Foreground = new SolidColorBrush(Color.FromRgb(118, 223, 147)),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };

            Grid.SetColumn(checkBox, 0);
            Grid.SetColumn(stackPanel, 1);
            Grid.SetColumn(statusTextBlock, 2);

            grid.Children.Add(checkBox);
            grid.Children.Add(stackPanel);
            grid.Children.Add(statusTextBlock);

            border.Child = grid;
            border.MouseLeftButtonDown += (s, e) => Task_MouseLeftButtonDown(s, e, task);

            return border;
        }

        private void Task_MouseLeftButtonDown(object sender, MouseButtonEventArgs e, Task task)
        {
            if (_isHistoryMode)
            {
                foreach (var completedTask in _completedTasks.Values)
                {
                    if (completedTask.TaskBorder != null)
                    {
                        completedTask.TaskBorder.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                        completedTask.TaskBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200));
                    }
                }
            }
            else
            {
                foreach (var activeTask in _tasks.Values)
                {
                    if (activeTask.TaskBorder != null)
                    {
                        activeTask.TaskBorder.Background = Brushes.White;
                        activeTask.TaskBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                    }
                }
            }

            if (sender is Border selectedBorder)
            {
                if (_isHistoryMode)
                {
                    selectedBorder.Background = new SolidColorBrush(Color.FromRgb(230, 240, 255));
                    selectedBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                }
                else
                {
                    selectedBorder.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255));
                    selectedBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(100, 149, 237));
                }

                _currentSelectedTaskId = task.Id;
                ShowTaskDetails(task);
            }
        }

        // Показ деталей
        private void ShowTaskDetails(Task task)
        {
            TaskDetailsContent.Visibility = Visibility.Visible;

            SelectedTaskTitle.Text = task.Title;
            TaskTime.Text = task.Time;
            TaskDate.Text = task.Date;
            TaskDescription.Text = task.Description;

            if (_isHistoryMode)
            {
                CompleteButton.Visibility = Visibility.Collapsed;
                DeleteButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                CompleteButton.Visibility = Visibility.Visible;
                DeleteButton.Visibility = Visibility.Visible;
            }
        }

        private Task FindTaskByCheckBox(CheckBox checkBox)
        {
            foreach (var task in _tasks.Values)
            {
                if (task.TaskCheckBox == checkBox)
                {
                    return task;
                }
            }

            foreach (var task in _completedTasks.Values)
            {
                if (task.TaskCheckBox == checkBox)
                {
                    return task;
                }
            }
            return null;
        }

        private void ApplyCompletedStyle(Task task)
        {
            if (task.TaskBorder != null)
            {
                task.TaskBorder.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                task.TaskBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200));

                if (task.TitleTextBlock != null)
                {
                    task.TitleTextBlock.TextDecorations = TextDecorations.Strikethrough;
                    task.TitleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150));
                }
            }
        }

        // Удаление
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (!_isHistoryMode && !string.IsNullOrEmpty(_currentSelectedTaskId) && _tasks.ContainsKey(_currentSelectedTaskId))
            {
                var task = _tasks[_currentSelectedTaskId];

                var result = MessageBox.Show($"Вы уверены, что хотите удалить задачу '{task.Title}'?",
                                           "Подтверждение удаления",
                                           MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _tasks.Remove(_currentSelectedTaskId);
                    LoadTasks(_currentCategory);

                    TaskDetailsContent.Visibility = Visibility.Collapsed;
                    SelectedTaskTitle.Text = "Выберите задачу";
                    _currentSelectedTaskId = null;

                    MessageBox.Show("Задача удалена", "Удалено",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private ControlTemplate CreateCheckBoxTemplate()
        {
            var template = new ControlTemplate(typeof(CheckBox));

            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            var ellipseFactory = new FrameworkElementFactory(typeof(Ellipse));
            ellipseFactory.SetValue(Ellipse.StrokeProperty, new SolidColorBrush(Color.FromRgb(46, 80, 252)));
            ellipseFactory.SetValue(Ellipse.StrokeThicknessProperty, 2.0);
            ellipseFactory.SetValue(Ellipse.FillProperty, Brushes.Transparent);
            ellipseFactory.SetValue(Ellipse.WidthProperty, 20.0);
            ellipseFactory.SetValue(Ellipse.HeightProperty, 20.0);
            ellipseFactory.SetValue(Ellipse.NameProperty, "outerEllipse");

            var contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            gridFactory.AppendChild(ellipseFactory);
            gridFactory.AppendChild(contentFactory);

            template.VisualTree = gridFactory;
            return template;
        }


        public void AddNewTask(Task newTask)
        {
            _tasks[newTask.Id] = newTask;
            LoadTasks(_currentCategory);
            _currentSelectedTaskId = newTask.Id;
            ShowTaskDetails(newTask);
        }

        private void Zadachi_Click(object sender, RoutedEventArgs e)
        {
            LoadTasks("Все");
            _currentSelectedTaskId = null;
            TaskDetailsContent.Visibility = Visibility.Collapsed;
            SelectedTaskTitle.Text = "Выберите задачу";
        }

        private void Istoria_Click(object sender, RoutedEventArgs e)
        {
            LoadHistory("Все");
            _currentSelectedTaskId = null;
            TaskDetailsContent.Visibility = Visibility.Collapsed;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Creating_tasks window6 = new Creating_tasks();

      
            window6.TaskCreated += OnTaskCreated;

         
            window6.Closed += (s, args) =>
            {
                window6.TaskCreated -= OnTaskCreated;
            };

            window6.Show();
        }
    }
}