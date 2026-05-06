using System.Windows;
using System.Windows.Media;

namespace Desktop
{
    public partial class MainWindow : Window
    {
        private readonly ApiService _apiService;

        public MainWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }



        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                StatusTextBlock.Text = "Fill all fields";
                StatusTextBlock.Foreground = Brushes.Red;
                return;
            }

            LoginButton.IsEnabled = false;
            StatusTextBlock.Text = "Connecting...";
            StatusTextBlock.Foreground = Brushes.Blue;

            bool success = await _apiService.LoginAsync(email, password);

            if (success)
            {
                AdminWindow adminWin = new AdminWindow(_apiService);
                adminWin.Show();
                this.Close();
            }
            else
            {
                StatusTextBlock.Text = "Invalid credentials";
                StatusTextBlock.Foreground = Brushes.Red;
                LoginButton.IsEnabled = true;
            }
        }
    }
}