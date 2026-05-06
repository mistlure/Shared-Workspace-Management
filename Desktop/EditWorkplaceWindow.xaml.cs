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
                StatusTextBox.Text = ((int)_workplace.CurrentStatus).ToString();
                WorkspaceIdTextBox.IsEnabled = false;
            }
            else
            {
                Title = "Add Workplace";
                StatusTextBox.Text = "0";
                StatusTextBox.IsEnabled = false;
            }
        }



        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(WorkspaceIdTextBox.Text, out int wId) && int.TryParse(StatusTextBox.Text, out int statusInt))
            {
                if (_workplace != null)
                {
                    _workplace.Name = NameTextBox.Text;
                    _workplace.CurrentStatus = (Domain.Enums.WorkplaceStatus)statusInt;
                    if (await _apiService.UpdateWorkplaceAsync(_workplace)) DialogResult = true;
                    else MessageBox.Show("Error updating workplace.");
                }
                else
                {
                    var newDto = new CreateWorkplaceDto { WorkspaceId = wId, Name = NameTextBox.Text };
                    if (await _apiService.CreateWorkplaceAsync(newDto)) DialogResult = true;
                    else MessageBox.Show("Error creating workplace.");
                }
            }
            else
            {
                MessageBox.Show("Workspace ID and Status must be integers! Do not leave them empty.");
            }
        }
    }
}