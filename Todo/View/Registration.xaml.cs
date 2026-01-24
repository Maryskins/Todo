using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Todo.Repository;
using Todo;

namespace Todo.View
{
    public partial class Registration : Window
    {
        private const string UsernamePlaceholder = "Введите имя пользователя";
        private const string EmailPlaceholder = "exam@yandex.ru";
        private const string PasswordPlaceholder = "Введите пароль";
        private const string ConfirmPasswordPlaceholder = "Повторите пароль";

        private readonly UserRepository _userRepository;
        private readonly InputValidator _inputValidator;

        private readonly Brush _placeholderBrush = Brushes.Gray;
        private readonly Brush _activeBrush = Brushes.Black;

        public Registration()
        {
            InitializeComponent();

            _userRepository = new UserRepository();
            _inputValidator = new InputValidator();

            SetupEventHandlers();
            InitializePlaceholderTexts();
        }

        private void SetupEventHandlers()
        {
            RegisterButton.Click += HandleRegistration;
            BackButton.Click += HandleBackNavigation;

            AttachPlaceholderHandlers(UsernameTextBox, UsernamePlaceholder);
            AttachPlaceholderHandlers(EmailTextBox, EmailPlaceholder);
            AttachPlaceholderHandlers(PasswordTextBox, PasswordPlaceholder);
            AttachPlaceholderHandlers(ConfirmPasswordTextBox, ConfirmPasswordPlaceholder);
        }

        private void AttachPlaceholderHandlers(TextBox textBox, string placeholderText)
        {
            textBox.GotFocus += (sender, e) => ClearPlaceholder(textBox, placeholderText);
            textBox.LostFocus += (sender, e) => RestorePlaceholderIfEmpty(textBox, placeholderText);
        }

        private void InitializePlaceholderTexts()
        {
            SetPlaceholderText(UsernameTextBox, UsernamePlaceholder);
            SetPlaceholderText(EmailTextBox, EmailPlaceholder);
            SetPlaceholderText(PasswordTextBox, PasswordPlaceholder);
            SetPlaceholderText(ConfirmPasswordTextBox, ConfirmPasswordPlaceholder);
        }

        private void HandleRegistration(object sender, RoutedEventArgs e)
        {
            try
            {
                var registrationData = GatherRegistrationData();

                if (!ValidateRegistrationData(registrationData))
                {
                    return;
                }

                ProcessRegistration(registrationData);
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка регистрации: {ex.Message}");
            }
        }

        private RegistrationData GatherRegistrationData()
        {
            return new RegistrationData
            {
                Username = UsernameTextBox.Text,
                Email = EmailTextBox.Text,
                Password = PasswordTextBox.Text,
                ConfirmPassword = ConfirmPasswordTextBox.Text
            };
        }

        private bool ValidateRegistrationData(RegistrationData data)
        {
            if (HasEmptyFields(data))
            {
                ShowWarningMessage("Пожалуйста, заполните все поля.");
                return false;
            }

            if (!_inputValidator.IsValidUsername(data.Username))
            {
                ShowWarningMessage("Имя пользователя должно содержать не менее 3 символов.");
                return false;
            }

            if (!_inputValidator.IsValidEmail(data.Email))
            {
                ShowWarningMessage("Пожалуйста, введите корректный email.");
                return false;
            }

            if (!_inputValidator.IsValidPassword(data.Password))
            {
                ShowWarningMessage("Пароль должен содержать не менее 6 символов.");
                return false;
            }

            if (data.Password != data.ConfirmPassword)
            {
                ShowWarningMessage("Пароли не совпадают.");
                return false;
            }

            return true;
        }

        private bool HasEmptyFields(RegistrationData data)
        {
            return IsPlaceholderOrEmpty(data.Username, UsernamePlaceholder) ||
                   IsPlaceholderOrEmpty(data.Email, EmailPlaceholder) ||
                   IsPlaceholderOrEmpty(data.Password, PasswordPlaceholder) ||
                   IsPlaceholderOrEmpty(data.ConfirmPassword, ConfirmPasswordPlaceholder);
        }

        private void ProcessRegistration(RegistrationData data)
        {
            bool isRegistered = _userRepository.RegisterUser(data.Username, data.Password, data.Email);

            if (isRegistered)
            {
                ShowSuccessMessage();
                NavigateToMainWindow();
            }
        }

        private void HandleBackNavigation(object sender, RoutedEventArgs e)
        {
            NavigateToLoginWindow();
        }

        private void NavigateToMainWindow()
        {
            var mainWindow = new Todo.View.Main_empty();
            mainWindow.Show();


            var currentWindow = Window.GetWindow(this);
            if (currentWindow != null)
            {
                currentWindow.Close();
            }
        }

        private void NavigateToLoginWindow()
        {
            var loginWindow = new Todo.MainWindow();
            loginWindow.Show();

            var currentWindow = Window.GetWindow(this);
            if (currentWindow != null)
            {
                currentWindow.Close();
            }
        }

        private void ClearPlaceholder(TextBox textBox, string placeholderText)
        {
            if (textBox.Text == placeholderText)
            {
                textBox.Text = string.Empty;
                textBox.Foreground = _activeBrush;
            }
        }

        private void RestorePlaceholderIfEmpty(TextBox textBox, string placeholderText)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                SetPlaceholderText(textBox, placeholderText);
            }
        }

        private void SetPlaceholderText(TextBox textBox, string placeholderText)
        {
            textBox.Text = placeholderText;
            textBox.Foreground = _placeholderBrush;
        }

        private bool IsPlaceholderOrEmpty(string text, string placeholderText)
        {
            return text == placeholderText || string.IsNullOrWhiteSpace(text);
        }

        private void ShowWarningMessage(string message)
        {
            MessageBox.Show(message, "Предупреждение",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ShowSuccessMessage()
        {
            MessageBox.Show("Регистрация прошла успешно!", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    internal class RegistrationData
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// Класс для проверки валидации входных данных
    /// </summary>
    public class InputValidator
    {
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 6;
        }

        public bool IsValidUsername(string username)
        {
            return !string.IsNullOrWhiteSpace(username) && username.Length >= 3;
        }
    }
}