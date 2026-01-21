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
    /// Логика взаимодействия для Creating_tasks.xaml
    /// </summary>
    public partial class Creating_tasks : Window
    {
        // Изменено: убрано static, теперь это экземплярное событие
        public event Action<Todo.Main.Task> TaskCreated;

        public Creating_tasks()
        {
            InitializeComponent();
            SetDefaultTime();
            SetDefaultDate();
        }

        private void SetDefaultTime()
        {
            var now = DateTime.Now;

            if (HoursComboBox != null && HoursComboBox.Items != null)
            {
                foreach (ComboBoxItem item in HoursComboBox.Items)
                {
                    if (item.Content?.ToString() == now.Hour.ToString("00"))
                    {
                        HoursComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            var roundedMinutes = (int)(Math.Round(now.Minute / 5.0) * 5);
            if (roundedMinutes == 60) roundedMinutes = 0;

            if (MinutesComboBox != null && MinutesComboBox.Items != null)
            {
                foreach (ComboBoxItem item in MinutesComboBox.Items)
                {
                    if (item.Content?.ToString() == roundedMinutes.ToString("00"))
                    {
                        MinutesComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void SetDefaultDate()
        {
            if (TaskDatePicker != null)
            {
                TaskDatePicker.SelectedDate = DateTime.Today;
            }
        }

        private void Time_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Метод может остаться пустым или быть реализован по необходимости
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (TitleTextBox == null || CategoryComboBox == null || TaskDatePicker == null)
            {
                MessageBox.Show("Ошибка инициализации формы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Введите название задачи", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TitleTextBox.Focus();
                return;
            }

            if (CategoryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                CategoryComboBox.Focus();
                return;
            }

            if (!TaskDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TaskDatePicker.Focus();
                return;
            }

            try
            {
                // Создание новой задачи
                var newTask = new Todo.Main.Task
                {
                    Id = Guid.NewGuid().ToString(), // Уникальный ID
                    Title = TitleTextBox.Text.Trim(),
                    Category = ((ComboBoxItem)CategoryComboBox.SelectedItem).Content?.ToString() ?? "Без категории",
                    Description = DescriptionTextBox?.Text?.Trim() ?? "",
                    Time = GetSelectedTime(),
                    Date = TaskDatePicker.SelectedDate.Value.ToString("dd MMMM yyyy"),
                    IsCompleted = false
                };

                // Вызов события с созданной задачей
                TaskCreated?.Invoke(newTask);

                MessageBox.Show($"Задача создана!\n" +
                              $"Название: {newTask.Title}\n" +
                              $"Категория: {newTask.Category}",
                              "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании задачи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string GetSelectedTime()
        {
            if (HoursComboBox?.SelectedItem != null && MinutesComboBox?.SelectedItem != null)
            {
                string hour = ((ComboBoxItem)HoursComboBox.SelectedItem).Content?.ToString() ?? "00";
                string minute = ((ComboBoxItem)MinutesComboBox.SelectedItem).Content?.ToString() ?? "00";
                return $"{hour}:{minute}";
            }
            return "00:00";
        }

        public DateTime GetSelectedDateTime()
        {
            if (TaskDatePicker?.SelectedDate.HasValue == true)
            {
                string time = GetSelectedTime();
                string dateString = TaskDatePicker.SelectedDate.Value.ToString("yyyy-MM-dd");
                return DateTime.Parse($"{dateString} {time}");
            }
            return DateTime.Now;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Метод может остаться пустым или быть реализован по необходимости
        }
    }
}