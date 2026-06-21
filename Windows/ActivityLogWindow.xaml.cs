using System.Collections.Generic;
using System.Windows;
using CyberChatbotApplication.Services;

namespace CyberChatbotApplication.Windows
{
    /// <summary>
    /// Displays the chatbot's activity log.
    /// Shows 10 entries by default; "Show More" loads all entries.
    /// </summary>
    public partial class ActivityLogWindow : Window
    {
        private readonly ActivityLogService _activityLog;
        private bool _showAll = false;

        // View model for ListView binding
        private class LogViewModel
        {
            public int Index { get; set; }
            public string Time { get; set; }
            public string Action { get; set; }
            public string Details { get; set; }
        }

        public ActivityLogWindow(ActivityLogService activityLog)
        {
            InitializeComponent();
            _activityLog = activityLog;
            LoadLog();
        }

        private void LoadLog()
        {
            var entries = _showAll
                ? _activityLog.GetAllEntries()
                : _activityLog.GetAllEntries().GetRange(0, System.Math.Min(10, _activityLog.TotalCount));

            var viewModels = new List<LogViewModel>();
            for (int i = 0; i < entries.Count; i++)
            {
                viewModels.Add(new LogViewModel
                {
                    Index = i + 1,
                    Time = entries[i].Timestamp.ToString("HH:mm dd MMM"),
                    Action = entries[i].Action,
                    Details = entries[i].Details
                });
            }
            LogListView.ItemsSource = viewModels;

            int shown = entries.Count;
            int total = _activityLog.TotalCount;
            CountLabel.Text = $"Showing {shown} of {total} actions";
            SummaryText.Text = $"Total actions recorded: {total}";
            ShowMoreBtn.IsEnabled = (!_showAll && total > 10);
        }

        private void ShowMoreBtn_Click(object sender, RoutedEventArgs e)
        {
            _showAll = true;
            LoadLog();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e) => LoadLog();
        private void CloseBtn_Click(object sender, RoutedEventArgs e) => Close();
    }
}
