using System.Windows;
using API.DTOs;
using Domain.Enums;

namespace Desktop
{
    public partial class EditWorkplaceWindow : Window
    {
        private readonly WorkplaceResponseDto? _workplace;
        private readonly ApiService _apiService;

        public EditWorkplaceWindow(WorkplaceResponseDto? workplace, ApiService apiService)
        {
            InitializeComponent();
            _workplace = workplace;
            _apiService = apiService;

            if (_workplace != null)
            {
                Title = "Edit Workplace";
                WorkspaceIdTextBox.Text = _workplace.WorkspaceId.ToString();
                NameTextBox.Text = _workplace.Name;
                WorkspaceIdTextBox.IsEnabled = false;

                int currentStatusId = (int)_workplace.CurrentStatus;
                foreach (System.Windows.Controls.ComboBoxItem item in StatusComboBox.Items)
                {
                    if (item.Tag != null && item.Tag.ToString() == currentStatusId.ToString())
                    {
                        StatusComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
            else
            {
                Title = "Add Workplace";
                StatusComboBox.SelectedIndex = 0;
                StatusComboBox.IsEnabled = false;
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Please enter a name for the workplace.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (int.TryParse(WorkspaceIdTextBox.Text, out int wId))
            {
                if (_workplace != null)
                {
                    _workplace.Name = NameTextBox.Text;

                    if (StatusComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem &&
                        int.TryParse(selectedItem.Tag?.ToString(), out int statusInt))
                    {
                        _workplace.CurrentStatus = (Domain.Enums.WorkplaceStatus)statusInt;
                    }

                    if (await _apiService.UpdateWorkplaceAsync(_workplace))
                        DialogResult = true;
                    else
                        MessageBox.Show("Error updating workplace.");
                }
                else
                {
                    var newDto = new CreateWorkplaceDto { WorkspaceId = wId, Name = NameTextBox.Text };
                    if (await _apiService.CreateWorkplaceAsync(newDto))
                        DialogResult = true;
                    else
                        MessageBox.Show("Error creating workplace.");
                }
            }
            else
            {
                MessageBox.Show("Workspace ID must be a valid integer!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}