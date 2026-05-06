using System.Windows;
using API.DTOs;

namespace Desktop
{
    public partial class EditUserWindow : Window
    {
        private readonly UserResponseDto _user;
        private readonly ApiService _apiService;

        public EditUserWindow(UserResponseDto user, ApiService apiService)
        {
            InitializeComponent();
            _user = user;
            _apiService = apiService;

            FirstNameTextBox.Text = _user.FirstName;
            LastNameTextBox.Text = _user.LastName;
            EmailTextBox.Text = _user.Email;
        }



        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("All fields are required!");
                return;
            }

            _user.FirstName = FirstNameTextBox.Text;
            _user.LastName = LastNameTextBox.Text;
            _user.Email = EmailTextBox.Text;

            bool success = await _apiService.UpdateUserAsync(_user);
            if (success)
            {
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Error updating user.");
            }
        }
    }
}