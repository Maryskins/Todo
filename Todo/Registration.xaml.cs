using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
using Todo.Repository;

namespace Todo
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        private const string UsernamePlaceholderText = "Введите имя пользователя";
        private const string EmailPlaceholderText = "exam@yandex.ru";
        private const string PasswordPlaceholderText = "Введите пароль";
        private const string ConfirmPasswordPlaceholderText = "Повторите пароль";

        private readonly UserRepository _userRepository;
        private readonly InputValidator _inputValidator;

        public Registration()
        {
            InitializeComponent();

            _userRepository = new UserRepository();
            _inputValidator = new InputValidator();

            InitializePlaceholders();
            SubscribeToEvents();
        }

        private void InitializePlaceholders()
        {
            SetTextBoxPlaceholder(UsernameTextBox, UsernamePlaceholderText);
            SetTextBoxPlaceholder(EmailTextBox, EmailPlaceholderText);
            SetTextBoxPlaceholder(PasswordTextBox, PasswordPlaceholderText);
            SetTextBoxPlaceholder(ConfirmPasswordTextBox, ConfirmPasswordPlaceholderText);
        }

        private void SubscribeToEvents()
        {
            RegisterButton.Click += OnRegisterButtonClick;
            BackButton.Click += OnBackButtonClick;

            UsernameTextBox.GotFocus += OnUsernameTextBoxGotFocus;
            UsernameTextBox.LostFocus += OnUsernameTextBoxLostFocus;

            EmailTextBox.GotFocus += OnEmailTextBoxGotFocus;
            EmailTextBox.LostFocus += OnEmailTextBoxLostFocus;

            PasswordTextBox.GotFocus += OnPasswordTextBoxGotFocus;
            PasswordTextBox.LostFocus += OnPasswordTextBoxLostFocus;

            ConfirmPasswordTextBox.GotFocus += OnConfirmPasswordTextBoxGotFocus;
            ConfirmPasswordTextBox.LostFocus += OnConfirmPasswordTextBoxLostFocus;
        }

        private void SetTextBoxPlaceholder(TextBox textBox, string placeholderText)
        {
            if (string.IsNullOrEmpty(textBox.Text) || textBox.Text == placeholderText)
            {
                textBox.Text = placeholderText;
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void ClearTextBoxPlaceholder(TextBox textBox)
        {
            if (textBox.Text == UsernamePlaceholderText ||
                textBox.Text == EmailPlaceholderText ||
                textBox.Text == PasswordPlaceholderText ||
                textBox.Text == ConfirmPasswordPlaceholderText)
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void OnRegisterButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = UsernameTextBox.Text;
                string email = EmailTextBox.Text;
                string password = PasswordTextBox.Text;
                string confirmPassword = ConfirmPasswordTextBox.Text;

                if (!ValidateRegistrationFields(username, email, password, confirmPassword))
                {
                    return;
                }

                bool registrationSuccessful = _userRepository.RegisterUser(username, password, email);

                if (registrationSuccessful)
                {
                    ShowSuccessMessage();

                    Main_empty mainWindow = new Main_empty();
                    mainWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка регистрации: {ex.Message}");
            }
        }

        private bool ValidateRegistrationFields(string username, string email, string password, string confirmPassword)
        {
            // Проверка на заполнение полей
            if (IsPlaceholderOrEmpty(username, UsernamePlaceholderText))
            {
                ShowWarningMessage("Пожалуйста, введите имя пользователя.");
                return false;
            }

            if (IsPlaceholderOrEmpty(email, EmailPlaceholderText))
            {
                ShowWarningMessage("Пожалуйста, введите email.");
                return false;
            }

            if (IsPlaceholderOrEmpty(password, PasswordPlaceholderText))
            {
                ShowWarningMessage("Пожалуйста, введите пароль.");
                return false;
            }

            if (IsPlaceholderOrEmpty(confirmPassword, ConfirmPasswordPlaceholderText))
            {
                ShowWarningMessage("Пожалуйста, повторите пароль.");
                return false;
            }

            // Валидация данных
            if (!_inputValidator.IsValidUsername(username))
            {
                ShowWarningMessage("Имя пользователя должно содержать не менее 3 символов.");
                return false;
            }

            if (!_inputValidator.IsValidEmail(email))
            {
                ShowWarningMessage("Пожалуйста, введите корректный email.");
                return false;
            }

            if (!_inputValidator.IsValidPassword(password))
            {
                ShowWarningMessage("Пароль должен содержать не менее 6 символов.");
                return false;
            }

            if (password != confirmPassword)
            {
                ShowWarningMessage("Пароли не совпадают.");
                return false;
            }

            return true;
        }

        private bool IsPlaceholderOrEmpty(string text, string placeholderText)
        {
            return text == placeholderText || string.IsNullOrWhiteSpace(text);
        }

        private void ShowWarningMessage(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ShowSuccessMessage()
        {
            MessageBox.Show("Регистрация прошла успешно!", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnUsernameTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            ClearTextBoxPlaceholder(UsernameTextBox);
        }

        private void OnUsernameTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                SetTextBoxPlaceholder(UsernameTextBox, UsernamePlaceholderText);
            }
        }

        private void OnEmailTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            ClearTextBoxPlaceholder(EmailTextBox);
        }

        private void OnEmailTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                SetTextBoxPlaceholder(EmailTextBox, EmailPlaceholderText);
            }
        }

        private void OnPasswordTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            ClearTextBoxPlaceholder(PasswordTextBox);
        }

        private void OnPasswordTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordTextBox.Text))
            {
                SetTextBoxPlaceholder(PasswordTextBox, PasswordPlaceholderText);
            }
        }

        private void OnConfirmPasswordTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            ClearTextBoxPlaceholder(ConfirmPasswordTextBox);
        }

        private void OnConfirmPasswordTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ConfirmPasswordTextBox.Text))
            {
                SetTextBoxPlaceholder(ConfirmPasswordTextBox, ConfirmPasswordPlaceholderText);
            }
        }
    }

    /// <summary>
    /// Класс для проверки валидации входных данных
    /// </summary>
    public class InputValidator
    {
        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            try
            {
                var mailAddress = new MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            return password.Length >= 6;
        }

        public bool IsValidUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            return username.Length >= 3;
        }
    }
}