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
            UsersDataGrid.ItemsSource = await _apiService.GetUsersAsync();
            WorkspacesDataGrid.ItemsSource = await _apiService.GetWorkspacesAsync();
            WorkplacesDataGrid.ItemsSource = await _apiService.GetWorkplacesAsync();
        }



        // ================= USERS =================
        private async void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is int id && MessageBox.Show("Delete user?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (await _apiService.DeleteUserAsync(id)) LoadData();
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is UserResponseDto user)
            {
                var editWindow = new EditUserWindow(user, _apiService) { Owner = this };
                if (editWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
        }

        // ================= WORKSPACES =================
        private void AddWorkspace_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new EditWorkspaceWindow(null, _apiService) { Owner = this };
            if (addWindow.ShowDialog() == true) LoadData();
        }

        private void EditWorkspace_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is WorkspaceResponseDto workspace)
            {
                var editWindow = new EditWorkspaceWindow(workspace, _apiService) { Owner = this };
                if (editWindow.ShowDialog() == true) LoadData();
            }
        }

        private async void DeleteWorkspace_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is int id && MessageBox.Show("Delete workspace and all related workplaces?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                bool success = await _apiService.DeleteWorkspaceAsync(id);
                if (success)
                {
                    LoadData();
                }
                else
                {
                    MessageBox.Show($"Не удалось удалить здание с ID {id}. Проверь работу сервера.");
                }
            }
        }

        // ================= WORKPLACES =================
        private void AddWorkplace_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new EditWorkplaceWindow(null, _apiService) { Owner = this };
            if (addWindow.ShowDialog() == true) LoadData();
        }

        private void EditWorkplace_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is WorkplaceResponseDto workplace)
            {
                var editWindow = new EditWorkplaceWindow(workplace, _apiService) { Owner = this };
                if (editWindow.ShowDialog() == true) LoadData();
            }
        }

        private async void DeleteWorkplace_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is int id && MessageBox.Show("Delete workplace?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (await _apiService.DeleteWorkplaceAsync(id)) LoadData();
            }
        }
    }
}