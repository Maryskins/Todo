using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Todo;

namespace Todo.View
{
    public partial class Main : Window
    {
       

        private string _currentSelectedTaskId;
        private readonly Dictionary<string, Task> _tasks;
        private readonly Dictionary<string, Task> _completedTasks;
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
            // Важно: InitializeComponent должен быть первым!
            InitializeComponent();

            _tasks = new Dictionary<string, Task>();
            _completedTasks = new Dictionary<string, Task>();

            LoadSavedPhoto();
            InitializeTasksDisplay();
        }
        private void LoadSavedPhoto()
        {
            try
            {
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var configFile = System.IO.Path.Combine(appDataPath, "WeddingAgency", "photo_path.txt");

                if (System.IO.File.Exists(configFile))
                {
                    var savedPath = System.IO.File.ReadAllText(configFile);
                    if (System.IO.File.Exists(savedPath))
                    {
                        LoadPhotoFromPath(savedPath);
                        return;
                    }
                }

                LoadDefaultPhoto();
            }
            catch (Exception ex)
            {
                LoadDefaultPhoto();
                Console.WriteLine($"Ошибка загрузки фото: {ex.Message}");
            }
        }

        private void LoadPhotoFromPath(string path)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            _userPhoto = bitmap;
            UpdatePhotoDisplay();
        }

        private void LoadDefaultPhoto()
        {
            try
            {
                const string defaultPhotoPath = "фото_умолч.jpg";
                if (System.IO.File.Exists(defaultPhotoPath))
                {
                    LoadPhotoFromPath(defaultPhotoPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки фото по умолчанию: {ex.Message}");
            }
        }

        private void UpdatePhotoDisplay()
        {
            if (UserPhotoImage != null && _userPhoto != null)
            {
                UserPhotoImage.Source = _userPhoto;
            }
        }

        public void SetUserPhoto(BitmapImage photo)
        {
            _userPhoto = photo;
            UpdatePhotoDisplay();
        }

        private void InitializeTasksDisplay()
        {
            LoadTasks();
        }


        private void OnTaskCreated(Todo.View.Main.Task newTask)
        {

            var localTask = new Task
            {
                Id = newTask.Id,
                Title = newTask.Title,
                Time = newTask.Time,
                Date = newTask.Date,
                Description = newTask.Description,
                Category = newTask.Category,
                IsCompleted = newTask.IsCompleted
            };

            _tasks[localTask.Id] = localTask;
            LoadTasks(_currentCategory);
            _currentSelectedTaskId = localTask.Id;
            ShowTaskDetails(localTask);

            ShowSuccessMessage($"Задача '{localTask.Title}' добавлена в список!");
        }

        private void LoadTasks(string category = "Все")
        {
            TasksPanel.Children.Clear();
            _currentCategory = category;
            _isHistoryMode = false;

            UpdateCategoryButtons(category);
            UpdateHeaderText("Задачи");

            var tasksToShow = GetTasksForCategory(category, _tasks.Values);

            if (!tasksToShow.Any())
            {
                ShowNoTasksMessage("Нет задач");
                return;
            }

            foreach (var task in tasksToShow)
            {
                var taskBorder = CreateTaskBorder(task);
                TasksPanel.Children.Add(taskBorder);
            }
        }

        private void LoadHistory(string category = "Все")
        {
            TasksPanel.Children.Clear();
            _isHistoryMode = true;

            UpdateCategoryButtons(category);
            UpdateHeaderText("История задач");

            var completedTasksToShow = GetTasksForCategory(category, _completedTasks.Values);

            if (!completedTasksToShow.Any())
            {
                ShowNoTasksMessage("Нет выполненных задач");
                return;
            }

            foreach (var task in completedTasksToShow)
            {
                var taskBorder = CreateHistoryTaskBorder(task);
                TasksPanel.Children.Add(taskBorder);
            }
        }

        private IEnumerable<Task> GetTasksForCategory(string category, IEnumerable<Task> tasks)
        {
            return category == "Все"
                ? tasks
                : tasks.Where(t => t.Category == category);
        }

        private void ShowNoTasksMessage(string message)
        {
            var noTasksText = new TextBlock
            {
                Text = message,
                FontSize = 16,
                Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 50, 0, 0)
            };
            TasksPanel.Children.Add(noTasksText);
        }

        private void MoveTaskToHistory(Task task)
        {
            _tasks.Remove(task.Id);
            _completedTasks[task.Id] = task;
            UpdateTaskDisplay();

            if (_currentSelectedTaskId == task.Id)
            {
                ClearSelectedTask();
            }
        }

        private void UpdateTaskDisplay()
        {
            if (!_isHistoryMode)
            {
                LoadTasks(_currentCategory);
            }
            else
            {
                LoadHistory(_currentCategory);
            }
        }

        private void ClearSelectedTask()
        {
            _currentSelectedTaskId = null;
            TaskDetailsContent.Visibility = Visibility.Collapsed;
            SelectedTaskTitle.Text = "Выберите задачу";
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isHistoryMode && sender is CheckBox checkBox)
            {
                var task = FindTaskByCheckBox(checkBox);
                if (task != null)
                {
                    CompleteTask(task);
                }
            }
        }

        private void CompleteTask(Task task)
        {
            task.IsCompleted = true;
            ApplyCompletedStyle(task);
            MoveTaskToHistory(task);

            ShowSuccessMessage($"Задача '{task.Title}' выполнена и перемещена в историю!");
        }

        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (!_isHistoryMode && !string.IsNullOrEmpty(_currentSelectedTaskId) && _tasks.ContainsKey(_currentSelectedTaskId))
            {
                var task = _tasks[_currentSelectedTaskId];
                CompleteTask(task);
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

            var activeButton = GetCategoryButton(activeCategory);
            if (activeButton != null)
            {
                SetActiveButtonStyle(activeButton);
            }
        }

        private Button GetCategoryButton(string category)
        {
            switch (category)
            {
                case "Дом":
                    return DomButton;
                case "Работа":
                    return RabotaButton;
                case "Учеба":
                    return UchebaButton;
                case "Отдых":
                    return OtdyhButton;
                default:
                    return null;
            }
        }

        private void SetActiveButtonStyle(Button button)
        {
            button.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255));
            button.BorderBrush = new SolidColorBrush(Color.FromRgb(100, 149, 237));
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
            var buttons = new[] { DomButton, RabotaButton, UchebaButton, OtdyhButton };
            foreach (var button in buttons)
            {
                button.Background = Brushes.White;
                button.BorderBrush = Brushes.White;
            }
        }

        private void DomButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleCategory("Дом");
        }

        private void RabotaButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleCategory("Работа");
        }

        private void UchebaButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleCategory("Учеба");
        }

        private void OtdyhButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleCategory("Отдых");
        }

        private void ToggleCategory(string category)
        {
            if (_isHistoryMode)
                LoadHistory(category);
            else
                LoadTasks(category);
        }

        private void Zadachi_Click(object sender, RoutedEventArgs e)
        {
            LoadTasks("Все");
            ClearSelectedTask();
        }

        private void Istoria_Click(object sender, RoutedEventArgs e)
        {
            LoadHistory("Все");
            ClearSelectedTask();
        }

        private Border CreateTaskBorder(Task task)
        {
            var border = CreateBaseTaskBorder(task.Id, Brushes.White, Color.FromRgb(224, 224, 224));
            var grid = CreateTaskGrid();

            var checkBox = CreateCheckBox();
            var taskContent = CreateTaskContent(task);

            Grid.SetColumn(checkBox, 0);
            Grid.SetColumn(taskContent, 1);

            grid.Children.Add(checkBox);
            grid.Children.Add(taskContent);
            border.Child = grid;

            border.MouseLeftButtonDown += (s, e) => Task_MouseLeftButtonDown(s, e, task);

            task.TaskBorder = border;
            task.TaskCheckBox = checkBox;
            var titleTextBlock = taskContent.Children[0] as TextBlock;
            if (titleTextBlock != null)
            {
                task.TitleTextBlock = titleTextBlock;
            }

            return border;
        }

        private Border CreateHistoryTaskBorder(Task task)
        {
            var border = CreateBaseTaskBorder(task.Id,
                new SolidColorBrush(Color.FromRgb(245, 245, 245)),
                Color.FromRgb(200, 200, 200));

            var grid = CreateHistoryTaskGrid();

            var checkBox = CreateCheckedCheckBox();
            var taskContent = CreateHistoryTaskContent(task);
            var statusLabel = CreateCompletedStatusLabel();

            Grid.SetColumn(checkBox, 0);
            Grid.SetColumn(taskContent, 1);
            Grid.SetColumn(statusLabel, 2);

            grid.Children.Add(checkBox);
            grid.Children.Add(taskContent);
            grid.Children.Add(statusLabel);
            border.Child = grid;

            border.MouseLeftButtonDown += (s, e) => Task_MouseLeftButtonDown(s, e, task);

            return border;
        }

        private Border CreateBaseTaskBorder(string taskId, Brush background, Color borderColor)
        {
            return new Border
            {
                Background = background,
                BorderBrush = new SolidColorBrush(borderColor),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(15),
                Width = 270,
                Tag = taskId
            };
        }

        private Grid CreateTaskGrid()
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            return grid;
        }

        private Grid CreateHistoryTaskGrid()
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            return grid;
        }

        private CheckBox CreateCheckBox()
        {
            return new CheckBox
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                IsChecked = false,
                Template = CreateCheckBoxTemplate()
            };
        }

        private CheckBox CreateCheckedCheckBox()
        {
            var checkBox = CreateCheckBox();
            checkBox.IsChecked = true;
            return checkBox;
        }

        private StackPanel CreateTaskContent(Task task)
        {
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

            return stackPanel;
        }

        private StackPanel CreateHistoryTaskContent(Task task)
        {
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

            return stackPanel;
        }

        private TextBlock CreateCompletedStatusLabel()
        {
            return new TextBlock
            {
                Text = "✓ Выполнено",
                Foreground = new SolidColorBrush(Color.FromRgb(118, 223, 147)),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };
        }

        private void Task_MouseLeftButtonDown(object sender, MouseButtonEventArgs e, Task task)
        {
            UpdateTaskSelection(task);

            if (sender is Border selectedBorder)
            {
                HighlightSelectedBorder(selectedBorder);
                _currentSelectedTaskId = task.Id;
                ShowTaskDetails(task);
            }
        }

        private void UpdateTaskSelection(Task selectedTask)
        {
            var tasksToUpdate = _isHistoryMode ? _completedTasks.Values : _tasks.Values;

            foreach (var task in tasksToUpdate)
            {
                if (task.TaskBorder != null)
                {
                    var backgroundColor = _isHistoryMode ?
                        new SolidColorBrush(Color.FromRgb(245, 245, 245)) :
                        Brushes.White;

                    var borderColor = _isHistoryMode ?
                        new SolidColorBrush(Color.FromRgb(200, 200, 200)) :
                        new SolidColorBrush(Color.FromRgb(224, 224, 224));

                    task.TaskBorder.Background = backgroundColor;
                    task.TaskBorder.BorderBrush = borderColor;
                }
            }
        }

        private void HighlightSelectedBorder(Border border)
        {
            if (_isHistoryMode)
            {
                border.Background = new SolidColorBrush(Color.FromRgb(230, 240, 255));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(100, 149, 237));
            }
            else
            {
                border.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255));
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(100, 149, 237));
            }
        }

        private void ShowTaskDetails(Task task)
        {
            TaskDetailsContent.Visibility = Visibility.Visible;

            SelectedTaskTitle.Text = task.Title;
            TaskTime.Text = task.Time;
            TaskDate.Text = task.Date;
            TaskDescription.Text = task.Description;

            CompleteButton.Visibility = _isHistoryMode ? Visibility.Collapsed : Visibility.Visible;
            DeleteButton.Visibility = _isHistoryMode ? Visibility.Collapsed : Visibility.Visible;
        }

        private Task FindTaskByCheckBox(CheckBox checkBox)
        {
            return _tasks.Values.Concat(_completedTasks.Values)
                .FirstOrDefault(t => t.TaskCheckBox == checkBox);
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

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (!_isHistoryMode && !string.IsNullOrEmpty(_currentSelectedTaskId) && _tasks.ContainsKey(_currentSelectedTaskId))
            {
                var task = _tasks[_currentSelectedTaskId];

                if (ConfirmDeletion(task.Title))
                {
                    _tasks.Remove(_currentSelectedTaskId);
                    LoadTasks(_currentCategory);
                    ClearSelectedTask();
                    ShowSuccessMessage("Задача удалена");
                }
            }
        }

        private bool ConfirmDeletion(string taskTitle)
        {
            var result = MessageBox.Show($"Вы уверены, что хотите удалить задачу '{taskTitle}'?",
                                       "Подтверждение удаления",
                                       MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        private ControlTemplate CreateCheckBoxTemplate()
        {
            var template = new ControlTemplate(typeof(CheckBox));

            var gridFactory = new FrameworkElementFactory(typeof(Grid));
            var ellipseFactory = CreateEllipseFactory();
            var contentFactory = CreateContentPresenterFactory();

            gridFactory.AppendChild(ellipseFactory);
            gridFactory.AppendChild(contentFactory);
            template.VisualTree = gridFactory;

            return template;
        }

        private FrameworkElementFactory CreateEllipseFactory()
        {
            var factory = new FrameworkElementFactory(typeof(Ellipse));
            factory.SetValue(Ellipse.StrokeProperty, new SolidColorBrush(Color.FromRgb(46, 80, 252)));
            factory.SetValue(Ellipse.StrokeThicknessProperty, 2.0);
            factory.SetValue(Ellipse.FillProperty, Brushes.Transparent);
            factory.SetValue(Ellipse.WidthProperty, 20.0);
            factory.SetValue(Ellipse.HeightProperty, 20.0);
            factory.SetValue(Ellipse.NameProperty, "outerEllipse");
            return factory;
        }

        private FrameworkElementFactory CreateContentPresenterFactory()
        {
            var factory = new FrameworkElementFactory(typeof(ContentPresenter));
            factory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            factory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            return factory;
        }

        public void AddNewTask(Task newTask)
        {
            _tasks[newTask.Id] = newTask;
            LoadTasks(_currentCategory);
            _currentSelectedTaskId = newTask.Id;
            ShowTaskDetails(newTask);
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

        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}