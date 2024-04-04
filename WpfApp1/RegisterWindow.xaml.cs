using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private AppDbContext db = new();
        private bool isPasswordVisible = true;
        private string userPassword = string.Empty;

        private bool isConfirmPasswordVisible = true;
        private string userConfirmPassword = string.Empty;


        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
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

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            txtUsername.BorderBrush = new SolidColorBrush(Colors.Gray);
            txtEmail.BorderBrush = new SolidColorBrush(Colors.Gray);
            confirmTxtPassword.BorderBrush = new SolidColorBrush(Colors.Gray);
            txtPassword.BorderBrush = new SolidColorBrush(Colors.Gray);

            txtPasswordBox.BorderBrush = new SolidColorBrush(Colors.Gray);
            confirmtxtPasswordBox.BorderBrush = new SolidColorBrush(Colors.Gray);

            string password = txtPassword.Visibility == Visibility.Visible ? txtPassword.Text : txtPasswordBox.Password;
            string confirmPassword = confirmTxtPassword.Visibility == Visibility.Visible ? confirmTxtPassword.Text : confirmtxtPasswordBox.Password;

            if (txtUsername.Text.Trim().Length < 8)
            {
                errorLabel.Content = "Логин должен быть больше 8 символов.";
                txtUsername.BorderBrush = Brushes.Red;
                return;
            }

            if (txtEmail.Text.Trim().Length == 0)
            {
                errorLabel.Content = "Укажите почту!";
                txtEmail.BorderBrush = Brushes.Red;
                return;
            }

            if (password.Trim().Length < 8)
            {
                errorLabel.Content = "Пароль должен быть больше 8 символов.";
                txtPassword.BorderBrush = Brushes.Red;
                txtPasswordBox.BorderBrush = Brushes.Red;
                return;
            }
            if (password != confirmPassword)
            {
                errorLabel.Content = "Пароли должны совпадать";
                txtPassword.BorderBrush = Brushes.Red;
                confirmTxtPassword.BorderBrush = Brushes.Red;

                txtPasswordBox.BorderBrush = Brushes.Red;
                confirmtxtPasswordBox.BorderBrush = Brushes.Red;

                return;
            }

            if (!Regex.IsMatch(txtEmail.Text, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$"))
            {
                errorLabel.Content = "Введите правильную почту!";
                txtEmail.BorderBrush = Brushes.Red;
                return;
            }

            if (!IsPasswordStrong(txtPassword.Text))
            {
                errorLabel.Content = "Вы должны использовать буквы, цифры и спец. символы";
                txtPassword.BorderBrush = Brushes.Red;
                return;
            }

            string login = txtUsername.Text;

            User isUserExist = db.Users.FirstOrDefault(x => x.Login == login);

            if (isUserExist is not null)
            {
                errorLabel.Content = "Данный логин уже занят";
                txtUsername.BorderBrush = Brushes.Red;
                return;
            }
            User user = new User()
            {
                Login = login,
                Password = password,
                Email = txtEmail.Text,
            };

            db.Users.Add(user);
            db.SaveChanges();

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }


        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox instance = (TextBox)sender;
            if (instance.Text == instance.Tag.ToString())
                instance.Text = "";
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox instance = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(instance.Text))
                instance.Text = instance.Tag.ToString();
        }

        private void Eye_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isPasswordVisible)
            {
                Eye.Source = new BitmapImage(new Uri("Images/view.png", UriKind.Relative));
                txtPasswordBox.Visibility = Visibility.Visible;
                txtPassword.Visibility = Visibility.Collapsed;
                userPassword = txtPassword.Text;
            }
            else
            {
                Eye.Source = new BitmapImage(new Uri("Images/hide.png", UriKind.Relative));
                txtPassword.Visibility = Visibility.Visible;
                txtPasswordBox.Visibility = Visibility.Collapsed;
                userPassword = txtPasswordBox.Password;
            }
            txtPassword.Text = userPassword;
            txtPasswordBox.Password = userPassword;

            isPasswordVisible = !isPasswordVisible;
        }

        private void Eye2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isConfirmPasswordVisible)
            {
                Eye2.Source = new BitmapImage(new Uri("Images/view.png", UriKind.Relative));
                confirmtxtPasswordBox.Visibility = Visibility.Visible;
                confirmTxtPassword.Visibility = Visibility.Collapsed;
                userConfirmPassword = confirmTxtPassword.Text;
            }
            else
            {
                Eye.Source = new BitmapImage(new Uri("Images/hide.png", UriKind.Relative));
                confirmTxtPassword.Visibility = Visibility.Visible;
                confirmtxtPasswordBox.Visibility = Visibility.Collapsed;
                userConfirmPassword = confirmtxtPasswordBox.Password;
            }
            confirmTxtPassword.Text = userConfirmPassword;
            confirmtxtPasswordBox.Password = userConfirmPassword;

            isConfirmPasswordVisible = !isConfirmPasswordVisible;
        }
    }
}
