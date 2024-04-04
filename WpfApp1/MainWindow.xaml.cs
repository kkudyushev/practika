using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppDbContext db = new();
        private bool isPasswordVisible = true;
        private string userPassword = string.Empty;
        public MainWindow()
        {
            InitializeComponent();

            if (Settings.FailedAuthCount >= 3)
            {
                BlockInputs();

                errorLabel.Content = "Иди отсюда умник, решил он мою систему анти-брута обмануть";
            }
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

            if (Settings.FailedAuthCount >= 3)
            {
                BlockInputs();
                
                errorLabel.Content = "Иди отсюда умник, решил он мою систему анти-брута обмануть";
                return;
            }

            userPassword = txtPassword.Visibility == Visibility.Visible ? txtPassword.Text : txtPasswordBox.Password;

            string loginOrEmail = txtUsername.Text;

            txtUsername.BorderBrush = new SolidColorBrush(Colors.Gray);
            txtPassword.BorderBrush = new SolidColorBrush(Colors.Gray);
            txtPasswordBox.BorderBrush = new SolidColorBrush(Colors.Gray);

            User isValidUser = db.Users.SingleOrDefault(x => x.Login == loginOrEmail && x.Password == userPassword);

            if (isValidUser is null)
                isValidUser = db.Users.SingleOrDefault(x => x.Email == loginOrEmail && x.Password == userPassword);

            if (isValidUser is null)
            {
                errorLabel.Content = "Неправильный логин или пароль!";
                txtUsername.BorderBrush = new SolidColorBrush(Colors.Red);
                txtPassword.BorderBrush = new SolidColorBrush(Colors.Red);

                Settings.FailedAuthCount++;

                if (Settings.FailedAuthCount >= 3)
                {
                    if (!Settings.isBlocking)
                        BlockInputs();
                }

                return;
            }
            Settings.FailedAuthCount = 0;
            Window1 window1 = new Window1(loginOrEmail);
            window1.Show();
            this.Close();
        }

        private void LoginTimer_Tick(object sender, EventArgs e)
        {
            txtPassword.IsEnabled = true;
            txtUsername.IsEnabled = true;
            Settings.FailedAuthCount = 0;
        }

        private void txtUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox instance = (TextBox)sender;
            if (instance.Text == instance.Tag.ToString())
                instance.Text = "";
        }

        private void txtUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox instance = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(instance.Text))
                instance.Text = instance.Tag.ToString();
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

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isPasswordVisible)
            {
                Eye.Source = new BitmapImage(new Uri("Images/view.png", UriKind.Relative));
                txtPasswordBox.Visibility = Visibility.Visible;
                txtPassword.Visibility = Visibility.Collapsed;
                userPassword = txtPassword.Text;
            } else
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

        private void BlockInputs()
        {
            Settings.blockThread = new Thread(() =>
            {
                var start = DateTime.Now;
                var end = start.AddSeconds(15);
                while (true)
                {
                    var currentTime = DateTime.Now;
                    if (currentTime >= end)
                        break;

                    Settings.isBlocking = true;

                    Dispatcher.Invoke(() =>
                    {
                        txtUsername.IsEnabled = false;
                        txtPassword.IsEnabled = false;
                        txtPasswordBox.IsEnabled = false;
                        btnLogin.IsEnabled = false;
                    });

                    Thread.Sleep(400);

                }
                Dispatcher.Invoke(() =>
                {
                    txtUsername.IsEnabled = true;
                    txtPassword.IsEnabled = true;
                    txtPasswordBox.IsEnabled = true;
                    btnLogin.IsEnabled = true;
                });
                Settings.FailedAuthCount = 0;
                Settings.isBlocking = false;
            });

            Settings.blockThread.Start();
        }
    }
}