using System.Text.RegularExpressions;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private string _username;
        private AppDbContext _db = new();

        public Window1(string username)
        {
            _username = username;
            InitializeComponent();

            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$"))
            {
                helloUser.Content = $"Здравствуйте, {_username}";
                return;
            }

            User user = _db.Users.FirstOrDefault(x => x.Email == _username);
            _username = user.Login;
            helloUser.Content = $"Здравствуйте, {_username}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UserPanel userPanel = new UserPanel(_username);
            userPanel.Show();
            this.Close();
        }
    }
}
