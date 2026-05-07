using System.Windows;
using API.DTOs;
using System.Collections.Generic;

namespace Desktop
{
    public partial class LogsWindow : Window
    {
        public LogsWindow(List<StatusHistoryResponseDto> logs)
        {
            InitializeComponent();
            LogsDataGrid.ItemsSource = logs;
        }
    }
}