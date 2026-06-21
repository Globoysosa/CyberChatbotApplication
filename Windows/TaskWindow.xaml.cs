using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CyberChatbotApplication.Models;
using CyberChatbotApplication.Services;

namespace CyberChatbotApplication.Windows
{
    /// <summary>
    /// Task Assistant window.
    /// Allows the user to add, view, complete, and delete cybersecurity tasks.
    /// All changes sync to MySQL via TaskService.
    /// </summary>
    public partial class TaskWindow : Window
    {
        private readonly TaskService _taskService;
        private readonly ActivityLogService _activityLog;

        // ─── View model for ListView binding ─────────────────────────────────────

        private class TaskViewModel
        {
            public int Id { get; set; }
            public string StatusIcon => IsCompleted ? "✅" : "⏳";
            public string Title { get; set; }
            public string Description { get; set; }
            public bool IsCompleted { get; set; }
            public string ReminderText { get; set; }
        }

        // ─── Constructor ─────────────────────────────────────────────────────────

        public TaskWindow(TaskService taskService, ActivityLogService activityLog)
        {
            InitializeComponent();
            _taskService = taskService;
            _activityLog = activityLog;
            LoadTasks();

            // Set DatePicker minimum to today
            ReminderDate.DisplayDateStart = DateTime.Today;
        }

        // ─── Load / Refresh ───────────────────────────────────────────────────────

        private void LoadTasks()
        {
            try
            {
                var tasks = _taskService.GetAllTasks();
                var viewModels = new List<TaskViewModel>();
                foreach (var t in tasks)
                {
                    viewModels.Add(new TaskViewModel
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description ?? "",
                        IsCompleted = t.IsCompleted,
                        ReminderText = t.ReminderDate.HasValue ? t.ReminderDate.Value.ToString("dd MMM yyyy") : "—"
                    });
                }
                TaskListView.ItemsSource = viewModels;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not load tasks: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ─── Event Handlers ───────────────────────────────────────────────────────

        private void AddTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleBox.Text.Trim();
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Please enter a task title.", "CYBA", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string desc = DescBox.Text.Trim();
            if (desc == "Description (optional)") desc = "";

            DateTime? reminder = ReminderDate.SelectedDate;

            var task = new CyberTask(title, desc, reminder);
            try
            {
                _taskService.AddTask(task);
                string logDetail = $"'{title}'" + (reminder.HasValue ? $" — Reminder: {reminder.Value:dd MMM yyyy}" : "");
                _activityLog.Log("Task added via Task Window", logDetail);

                // Clear inputs
                TitleBox.Clear();
                DescBox.Text = "Description (optional)";
                ReminderDate.SelectedDate = null;

                LoadTasks();
                MessageBox.Show($"✅ Task '{title}' added successfully!", "CYBA", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not add task: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CompleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = TaskListView.SelectedItem as TaskViewModel;
            if (selected == null)
            {
                MessageBox.Show("Please select a task to mark as complete.", "CYBA", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (selected.IsCompleted)
            {
                MessageBox.Show("This task is already completed!", "CYBA", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                _taskService.MarkCompleted(selected.Id);
                _activityLog.Log("Task marked complete", $"'{selected.Title}'");
                LoadTasks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not update task: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = TaskListView.SelectedItem as TaskViewModel;
            if (selected == null)
            {
                MessageBox.Show("Please select a task to delete.", "CYBA", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var confirm = MessageBox.Show($"Delete task: '{selected.Title}'?", "Confirm Delete",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm == MessageBoxResult.Yes)
            {
                try
                {
                    _taskService.DeleteTask(selected.Id);
                    _activityLog.Log("Task deleted", $"'{selected.Title}'");
                    LoadTasks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not delete task: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e) => LoadTasks();

        private void CloseBtn_Click(object sender, RoutedEventArgs e) => Close();

        private void DescBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DescBox.Text == "Description (optional)")
            {
                DescBox.Text = "";
                DescBox.Foreground = System.Windows.Media.Brushes.White;
            }
        }
    }
}