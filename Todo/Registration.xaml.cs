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
        private string usernamePlaceholderText = "Введите имя пользователя";
        private string emailPlaceholderText = "exam@yandex.ru";
        private string passwordPlaceholderText = "Введите пароль";
        private string confirmPasswordPlaceholderText = "Повторите пароль";

        private UserRepository _userRepository;
        public Registration()
        {
            InitializeComponent();
            _userRepository = new UserRepository();


            SetPlaceholder(TextBox_Username, usernamePlaceholderText);
            SetPlaceholder(TextBox_Email, emailPlaceholderText);
            SetPlaceholder(TextBox_Password, passwordPlaceholderText);
            SetPlaceholder(TextBox_ConfirmPassword, confirmPasswordPlaceholderText);


            Зарегистрироваться.Click += Зарегистрироваться_Click;
            Назад.Click += Назад_Click;


            TextBox_Username.GotFocus += TextBox_Username_GotFocus;
            TextBox_Username.LostFocus += TextBox_Username_LostFocus;
            TextBox_Email.GotFocus += TextBox_Email_GotFocus;
            TextBox_Email.LostFocus += TextBox_Email_LostFocus;


            TextBox_Password.GotFocus += TextBox_Password_GotFocus;
            TextBox_Password.LostFocus += TextBox_Password_LostFocus;
            TextBox_ConfirmPassword.GotFocus += TextBox_ConfirmPassword_GotFocus;
            TextBox_ConfirmPassword.LostFocus += TextBox_ConfirmPassword_LostFocus;
        }
        private void SetPlaceholder(TextBox textBox, string placeholder)
        {
            if (string.IsNullOrEmpty(textBox.Text) || textBox.Text == placeholder)
            {
                textBox.Text = placeholder;
                textBox.Foreground = Brushes.Gray;
            }
        }

        //очистка подсказки
        private void ClearPlaceholder(TextBox textBox)
        {

            if (textBox.Text == usernamePlaceholderText ||
                textBox.Text == emailPlaceholderText ||
                textBox.Text == passwordPlaceholderText ||
                textBox.Text == confirmPasswordPlaceholderText)
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }


        private void Назад_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }


        private void Зарегистрироваться_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = TextBox_Username.Text;
                string email = TextBox_Email.Text;
                string password = TextBox_Password.Text;
                string confirmPassword = TextBox_ConfirmPassword.Text;

                InputValidator validator = new InputValidator();


                if (username == usernamePlaceholderText || string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show("Пожалуйста, введите имя пользователя.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (email == emailPlaceholderText || string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show("Пожалуйста, введите email.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (password == passwordPlaceholderText || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Пожалуйста, введите пароль.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (confirmPassword == confirmPasswordPlaceholderText || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    MessageBox.Show("Пожалуйста, повторите пароль.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Валидация данных
                if (!validator.IsValidUsername(username))
                {
                    MessageBox.Show("Имя пользователя должно содержать не менее 3 символов.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!validator.IsValidEmail(email))
                {
                    MessageBox.Show("Пожалуйста, введите корректный email.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!validator.IsValidPassword(password))
                {
                    MessageBox.Show("Пароль должен содержать не менее 6 символов.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (password != confirmPassword)
                {
                    MessageBox.Show("Пароли не совпадают.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Регистрация пользователя через репозиторий
                bool registrationSuccess = _userRepository.RegisterUser(username, password, email);

                if (registrationSuccess)
                {
                    MessageBox.Show("Регистрация прошла успешно!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    Main_empty window2 = new Main_empty();
                    window2.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBox_Username_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearPlaceholder(TextBox_Username);
        }

        private void TextBox_Username_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBox_Username.Text))
            {
                SetPlaceholder(TextBox_Username, usernamePlaceholderText);
            }
        }

        private void TextBox_Email_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearPlaceholder(TextBox_Email);
        }

        private void TextBox_Email_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBox_Email.Text))
            {
                SetPlaceholder(TextBox_Email, emailPlaceholderText);
            }
        }

        private void TextBox_Password_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearPlaceholder((TextBox)sender);
        }

        private void TextBox_Password_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBox_Password.Text))
            {
                SetPlaceholder(TextBox_Password, passwordPlaceholderText);
            }
        }

        private void TextBox_ConfirmPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearPlaceholder((TextBox)sender);
        }

        private void TextBox_ConfirmPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBox_ConfirmPassword.Text))
            {
                SetPlaceholder(TextBox_ConfirmPassword, confirmPasswordPlaceholderText);
            }
        }
    }

    //проверка валидации
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
                var addr = new MailAddress(email);
                return addr.Address == email;
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
