using System.Windows;
using System.Windows.Controls;
using API.DTOs;

namespace Desktop
{
    public partial class AdminWindow : Window
    {
        private readonly ApiService _apiService;

        public AdminWindow(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            LoadData();
        }

        private async void LoadData()
        {
            var users = await _apiService.GetUsersAsync();
            UsersDataGrid.ItemsSource = users;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int userId)
            {
                var result = MessageBox.Show($"Delete user {userId}?", "Confirm", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    bool deleted = await _apiService.DeleteUserAsync(userId);
                    if (deleted)
                    {
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Delete failed. Check if user has related records.");
                    }
                }
            }
        }
    }
}