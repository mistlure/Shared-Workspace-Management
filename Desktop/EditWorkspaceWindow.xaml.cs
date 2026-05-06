using System.Windows;
using API.DTOs;

namespace Desktop
{
    public partial class EditWorkspaceWindow : Window
    {
        private readonly WorkspaceResponseDto? _workspace;
        private readonly ApiService _apiService;

        public EditWorkspaceWindow(WorkspaceResponseDto? workspace, ApiService apiService)
        {
            InitializeComponent();
            _workspace = workspace;
            _apiService = apiService;

            if (_workspace != null)
            {
                Title = "Edit Workspace";
                NameTextBox.Text = _workspace.Name;
                LocationTextBox.Text = _workspace.Location;
                MaxHoursTextBox.Text = _workspace.MaxOccupationHours.ToString();
                PriceTextBox.Text = _workspace.PricePerHour.ToString();
            }
            else
            {
                Title = "Add Workspace";
            }
        }



        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(MaxHoursTextBox.Text, out int hours) && decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                if (_workspace != null)
                {
                    _workspace.Name = NameTextBox.Text;
                    _workspace.Location = LocationTextBox.Text;
                    _workspace.MaxOccupationHours = hours;
                    _workspace.PricePerHour = price;
                    if (await _apiService.UpdateWorkspaceAsync(_workspace)) DialogResult = true;
                }
                else
                {
                    var newDto = new CreateWorkspaceDto { Name = NameTextBox.Text, Location = LocationTextBox.Text, MaxOccupationHours = hours, PricePerHour = price };
                    if (await _apiService.CreateWorkspaceAsync(newDto)) DialogResult = true;
                }
            }
            else MessageBox.Show("Invalid number format.");
        }
    }
}