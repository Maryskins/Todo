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
        public MainWindow()
        {
            InitializeComponent();


            Войти.Click += Войти_Click;
            Регистрация.Click += Регистрация_Click;


            EmailTextBox.GotFocus += EmailTextBox_GotFocus;
            EmailTextBox.LostFocus += EmailTextBox_LostFocus;
            PasswordTextBox.GotFocus += PasswordTextBox_GotFocus;
            PasswordTextBox.LostFocus += PasswordTextBox_LostFocus;



            EmailTextBox.Text = "Введите почту";
            EmailTextBox.Foreground = Brushes.Gray;

            PasswordTextBox.Text = "Введите пароль";
            PasswordTextBox.Foreground = Brushes.Gray;
        }
        private void Войти_Click(object sender, RoutedEventArgs e)
        {

            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Text;



            if (email == "Введите почту" || password == "Введите пароль")
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }


            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов!");
                return;
            }


            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email имеет неверный формат.");
                return;
            }




            MessageBox.Show("Вход выполнен успешно!");
            Window5 window5 = new Window5();
            window5.Show();
            this.Close();
        }

        private void Регистрация_Click(object sender, RoutedEventArgs e)
        {

            Window1 window1 = new Window1();
            window1.Show();
            this.Close();
        }


        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }


            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";


            return Regex.IsMatch(email, pattern);

        }


        private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (EmailTextBox.Text == "Введите почту")
            {
                EmailTextBox.Text = "";
                EmailTextBox.Foreground = Brushes.Black;
            }
        }

        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(EmailTextBox.Text))
            {
                EmailTextBox.Text = "Введите почту";
                EmailTextBox.Foreground = Brushes.Gray;
            }
        }


        private void PasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordTextBox.Text == "Введите пароль")
            {
                PasswordTextBox.Text = "";
                PasswordTextBox.Foreground = Brushes.Black;
            }
        }

        private void PasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordTextBox.Text))
            {
                PasswordTextBox.Text = "Введите пароль";
                PasswordTextBox.Foreground = Brushes.Gray;
            }
        }
    }
}
