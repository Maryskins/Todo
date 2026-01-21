using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Todo
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DefaultEmailText = "Введите почту";
        private const string DefaultPasswordText = "Введите пароль";

        public MainWindow()
        {
            InitializeComponent();

          
            LoginButton.Click += OnLoginButtonClick;
            RegisterButton.Click += OnRegisterButtonClick;

       
            EmailTextBox.GotFocus += OnEmailTextBoxGotFocus;
            EmailTextBox.LostFocus += OnEmailTextBoxLostFocus;
            PasswordTextBox.GotFocus += OnPasswordTextBoxGotFocus;
            PasswordTextBox.LostFocus += OnPasswordTextBoxLostFocus;

       
            SetDefaultTextFields();
        }

        private void SetDefaultTextFields()
        {
            EmailTextBox.Text = DefaultEmailText;
            EmailTextBox.Foreground = Brushes.Gray;

            PasswordTextBox.Text = DefaultPasswordText;
            PasswordTextBox.Foreground = Brushes.Gray;
        }

        private void OnLoginButtonClick(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Text;

       
            if (email == DefaultEmailText || password == DefaultPasswordText)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

      
            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов!");
                return;
            }

    
            if (!IsEmailValid(email))
            {
                MessageBox.Show("Email имеет неверный формат.");
                return;
            }

           
            MessageBox.Show("Вход выполнен успешно!");

   
            Main mainWindow = new Main();
            mainWindow.Show();
            this.Close();
        }

        private void OnRegisterButtonClick(object sender, RoutedEventArgs e)
        {
            Registration registrationWindow = new Registration();
            registrationWindow.Show();
            this.Close();
        }

        private bool IsEmailValid(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(email, emailPattern);
        }

        private void OnEmailTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (EmailTextBox.Text == DefaultEmailText)
            {
                EmailTextBox.Text = "";
                EmailTextBox.Foreground = Brushes.Black;
            }
        }

        private void OnEmailTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(EmailTextBox.Text))
            {
                EmailTextBox.Text = DefaultEmailText;
                EmailTextBox.Foreground = Brushes.Gray;
            }
        }

        private void OnPasswordTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordTextBox.Text == DefaultPasswordText)
            {
                PasswordTextBox.Text = "";
                PasswordTextBox.Foreground = Brushes.Black;
            }
        }

        private void OnPasswordTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordTextBox.Text))
            {
                PasswordTextBox.Text = DefaultPasswordText;
                PasswordTextBox.Foreground = Brushes.Gray;
            }
        }
    }
}