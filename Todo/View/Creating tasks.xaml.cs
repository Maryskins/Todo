using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Todo.View
{
    public partial class Creating_tasks : Window
    {
        public event Action<Todo.View.Main.Task> TaskCreated;

        public Creating_tasks()
        {
            InitializeComponent();
            InitializeDefaultValues();
            SetupEventHandlers();
        }

        private void InitializeDefaultValues()
        {
            SetDefaultTime();
            SetDefaultDate();
        }

        private void SetupEventHandlers()
        {

        }

        private void SetDefaultTime()
        {
            var currentTime = DateTime.Now;
            var currentHour = currentTime.Hour;
            var currentMinute = currentTime.Minute;

            SelectComboBoxItem(HoursComboBox, currentHour.ToString("00"));
            SelectComboBoxItem(MinutesComboBox, GetRoundedMinutes(currentMinute).ToString("00"));
        }

        private int GetRoundedMinutes(int minutes)
        {
            var rounded = (int)(Math.Round(minutes / 5.0) * 5);
            return rounded == 60 ? 0 : rounded;
        }

        private void SetDefaultDate()
        {
            if (TaskDatePicker != null)
            {
                TaskDatePicker.SelectedDate = DateTime.Today;
            }
        }

        private void SelectComboBoxItem(ComboBox comboBox, string value)
        {
            if (comboBox != null && comboBox.Items != null)
            {
                foreach (ComboBoxItem item in comboBox.Items)
                {
                    if (item.Content?.ToString() == value)
                    {
                        comboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void Time_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            try
            {
                var newTask = CreateNewTask();
                OnTaskCreated(newTask);
                ShowSuccessMessage(newTask);
                CloseWindow();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при создании задачи: {ex.Message}");
            }
        }

        private bool ValidateForm()
        {
            if (!ValidateFormComponents())
            {
                return false;
            }

            if (!ValidateTitle())
            {
                return false;
            }

            if (!ValidateCategory())
            {
                return false;
            }

            if (!ValidateDate())
            {
                return false;
            }

            return true;
        }

        private bool ValidateFormComponents()
        {
            if (TitleTextBox == null || CategoryComboBox == null || TaskDatePicker == null)
            {
                ShowErrorMessage("Ошибка инициализации формы");
                return false;
            }

            return true;
        }

        private bool ValidateTitle()
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                ShowWarningMessage("Введите название задачи");
                TitleTextBox.Focus();
                return false;
            }

            return true;
        }

        private bool ValidateCategory()
        {
            if (CategoryComboBox.SelectedItem == null)
            {
                ShowWarningMessage("Выберите категорию");
                CategoryComboBox.Focus();
                return false;
            }

            return true;
        }

        private bool ValidateDate()
        {
            if (!TaskDatePicker.SelectedDate.HasValue)
            {
                ShowWarningMessage("Выберите дату");
                TaskDatePicker.Focus();
                return false;
            }

            return true;
        }

        private Todo.View.Main.Task CreateNewTask()
        {
            return new Todo.View.Main.Task
            {
                Id = Guid.NewGuid().ToString(),
                Title = TitleTextBox.Text.Trim(),
                Category = GetSelectedCategory(),
                Description = GetTaskDescription(),
                Time = GetSelectedTime(),
                Date = GetFormattedDate(),
                IsCompleted = false
            };
        }

        private string GetSelectedCategory()
        {
            if (CategoryComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                return selectedItem.Content?.ToString() ?? "Без категории";
            }

            return "Без категории";
        }

        private string GetTaskDescription()
        {
            return DescriptionTextBox?.Text?.Trim() ?? string.Empty;
        }

        private string GetSelectedTime()
        {
            var hour = GetSelectedComboBoxValue(HoursComboBox) ?? "00";
            var minute = GetSelectedComboBoxValue(MinutesComboBox) ?? "00";
            return $"{hour}:{minute}";
        }

        private string GetFormattedDate()
        {
            return TaskDatePicker.SelectedDate.Value.ToString("dd MMMM yyyy");
        }

        private string GetSelectedComboBoxValue(ComboBox comboBox)
        {
            if (comboBox?.SelectedItem is ComboBoxItem selectedItem)
            {
                return selectedItem.Content?.ToString();
            }

            return null;
        }

        private void OnTaskCreated(Todo.View.Main.Task task)
        {
            TaskCreated?.Invoke(task);
        }

        private void ShowSuccessMessage(Todo.View.Main.Task task)
        {
            var message = $"Задача создана!\n" +
                         $"Название: {task.Title}\n" +
                         $"Категория: {task.Category}";

            MessageBox.Show(message, "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowWarningMessage(string message)
        {
            MessageBox.Show(message, "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            this.Close();
        }

        public DateTime GetSelectedDateTime()
        {
            if (TaskDatePicker?.SelectedDate.HasValue == true)
            {
                var time = GetSelectedTime();
                var dateString = TaskDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd");
                return DateTime.Parse($"{dateString} {time}");
            }

            return DateTime.Now;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}