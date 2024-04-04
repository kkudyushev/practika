using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для UserPanel.xaml
    /// </summary>
    public partial class UserPanel : Window
    {
        private string _username;
        private AppDbContext _db = new();


        public UserPanel(string username)
        {
            InitializeComponent();
            _username = username;

            User user = _db.Users.FirstOrDefault(x => x.Login == _username);

            currentLogin.Content += $" {_username}";
            currentMail.Content += $" {user.Email}";
            currentPassword.Content += $" {user.Password}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            User user = _db.Users.FirstOrDefault(x => x.Login == _username);

            if (!Regex.IsMatch(newEmail.Text, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$"))
            {
                MessageBox.Show("Введите правильную почту!");
                return;
            }

            user.Email = newEmail.Text;

            _db.SaveChanges();
            UpdateLabels();
            MessageBox.Show("Почта успешно изменена");
        }

        private void UpdateLabels()
        {
            User user = _db.Users.FirstOrDefault(x => x.Login == _username);

            currentLogin.Content = $"Текущий логин: {_username}";
            currentMail.Content = $"Текущая почта: {user.Email}";
            currentPassword.Content = $"Текущий пароль: {user.Password}";
        }

        private bool IsPasswordStrong(string password)
        {
            string upperCasePattern = @"[A-Z]";
            string lowerCasePattern = @"[a-z]";
            string digitPattern = @"\d";
            string specialCharPattern = @"[!@#$%^&*(),.?:{}|<>]";

            if (!Regex.IsMatch(password, upperCasePattern))
            {
                return false;
            }

            if (!Regex.IsMatch(password, lowerCasePattern))
            {
                return false;
            }

            if (!Regex.IsMatch(password, digitPattern))
            {
                return false;
            }

            if (!Regex.IsMatch(password, specialCharPattern))
            {
                return false;
            }

            return true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            User user = _db.Users.FirstOrDefault(x => x.Login == _username);
            
            if (!IsPasswordStrong(newPassword.Text))
            {
                MessageBox.Show("Ваш пароль не достаточно сложный! Используйте спец. символы, цифры и буквы разного регистра");
                return;
            }

            user.Password = newPassword.Text;

            _db.SaveChanges();
            UpdateLabels();
            MessageBox.Show("Пароль успешно изменен");
        }
    }
}
