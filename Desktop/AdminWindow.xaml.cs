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

        private async Task LoadAllDataFromServer()
        {
            try
            {
                var users = await _apiService.GetUsersAsync();
                UsersDataGrid.ItemsSource = users;

                var workspaces = await _apiService.GetWorkspacesAsync();
                WorkspacesDataGrid.ItemsSource = workspaces;

                var workplaces = await _apiService.GetWorkplacesAsync();
                WorkplacesDataGrid.ItemsSource = workplaces;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing data: {ex.Message}");
            }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btn.Content = "Refreshing...";
            btn.IsEnabled = false;

            await LoadAllDataFromServer();

            btn.Content = "Refresh Data";
            btn.IsEnabled = true;
        }

        private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                await LoadAllDataFromServer();
            }
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
            // Теперь мы получаем объект целиком из Tag="{Binding}"
            if ((sender as Button)?.Tag is WorkplaceResponseDto workplace)
            {
                // Жесткая проверка: блокируем удаление занятого стола на уровне кода
                if (workplace.CurrentStatus == Domain.Enums.WorkplaceStatus.Occupied)
                {
                    MessageBox.Show("Cannot delete an occupied desk! Please release it first.", "Warning");
                    return;
                }

                if (MessageBox.Show("Are you sure you want to delete this desk?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (await _apiService.DeleteWorkplaceAsync(workplace.Id))
                        LoadData();
                    else
                        MessageBox.Show("Failed to delete the desk.");
                }
            }
        }

        private async void ReleaseWorkplace_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is WorkplaceResponseDto workplace)
            {
                if (workplace.CurrentStatus == Domain.Enums.WorkplaceStatus.Available)
                {
                    MessageBox.Show("This desk is already available.");
                    return;
                }

                var confirmMsg = $"Are you sure you want to release desk '{workplace.Name}'? This will end the current user's booking.";
                if (MessageBox.Show(confirmMsg, "Confirm Release", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var activeOccupancy = await _apiService.GetActiveOccupancyByWorkplaceIdAsync(workplace.Id);

                    bool success = false;

                    if (activeOccupancy != null)
                    {
                        success = await _apiService.FinishOccupancyAsync(activeOccupancy.Id);
                    }
                    else
                    {
                        workplace.CurrentStatus = Domain.Enums.WorkplaceStatus.Available;
                        success = await _apiService.UpdateWorkplaceAsync(workplace);
                    }

                    if (success)
                    {
                        await LoadAllDataFromServer();
                    }
                    else
                    {
                        MessageBox.Show("Failed to release the workplace.");
                    }
                }
            }
        }

        private async void ViewLogs_Click(object sender, RoutedEventArgs e)
        {
            var logs = await _apiService.GetStatusHistoryAsync();

            LogsWindow logsWin = new LogsWindow(logs);
            logsWin.Owner = this;
            logsWin.ShowDialog();
        }
    }

    public class StatusConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return false;

            int status = System.Convert.ToInt32(value);
            string action = parameter as string;

            if (action == "IsEditEnabled")
            {
                return status != 2;
            }
            if (action == "ReleaseVisibility")
            {
                return status == 2 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => throw new NotImplementedException();
    }
}