using System;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CyberChatbotApplication.Services;
using CyberChatbotApplication.Windows;

namespace CyberChatbotApplication
{
    /// <summary>
    /// Main window for CYBA v3.0 — integrates Parts 1, 2, and 3:
    ///   Part 1: Voice greeting, ASCII art, basic Q&A
    ///   Part 2: GUI chat, keyword recognition, sentiment, memory
    ///   Part 3: Task Assistant, Quiz, Activity Log, NLP routing
    /// </summary>
    public partial class MainWindow : Window
    {
        // ─── Fields ──────────────────────────────────────────────────────────────

        private CYBA _cyba;
        private QuizService _quizService;

        // ─── Constructor ─────────────────────────────────────────────────────────

        public MainWindow()
        {
            InitializeComponent();
            _cyba = new CYBA();
            _quizService = new QuizService();

            PlayVoiceGreeting();
            AddBotMessage("Hello! 👋 Welcome to the Cybersecurity Awareness Bot v3.0. I'm here to help you stay safe online.");
            AddBotMessage("What's your name?");

            // Show DB status
            if (!_cyba.IsDatabaseAvailable())
            {
                AddSystemMessage("⚠️ MySQL database not available. Task features require MySQL. See README for setup instructions.");
            }
            else
            {
                AddSystemMessage("✅ Database connected. Task features are active!");
            }
        }

        // ─── Voice Greeting (Part 1) ──────────────────────────────────────────────

        private void PlayVoiceGreeting()
        {
            // Try relative path first, then absolute
            string relativePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "voice_folder", "greeting.wav.wav");
            string absolutePath = @"C:\Users\Student\Desktop\poe_part1\poe_part1\CyberChatbotApplication-master\voice_folder\greeting.wav.wav";

            string path = File.Exists(relativePath) ? relativePath :
                          File.Exists(absolutePath) ? absolutePath : null;

            if (path != null)
            {
                try
                {
                    var player = new SoundPlayer(path);
                    player.Play();
                    AddSystemMessage("🔊 Voice greeting played!");
                }
                catch (Exception ex)
                {
                    AddSystemMessage($"⚠️ Could not play voice: {ex.Message}");
                }
            }
            else
            {
                AddSystemMessage("ℹ️ Voice greeting file not found. Place greeting.wav.wav in /voice_folder/.");
            }
        }

        // ─── Input Processing ─────────────────────────────────────────────────────

        private void ProcessUserInput()
        {
            string userMessage = UserInput.Text.Trim();
            if (string.IsNullOrEmpty(userMessage)) return;

            AddUserMessage(userMessage);
            UserInput.Clear();

            string response = _cyba.ProcessInput(userMessage);

            // Special signal to open QuizWindow
            if (response == "__START_QUIZ__")
            {
                AddBotMessage("🎮 Opening the Cybersecurity Quiz... Good luck!");
                OpenQuizWindow();
                return;
            }

            AddBotMessage(response);
            ChatScrollViewer.ScrollToBottom();
        }

        // ─── Toolbar Button Handlers ──────────────────────────────────────────────

        private void VoiceButton_Click(object sender, RoutedEventArgs e)
        {
            PlayVoiceGreeting();
        }

        private void TasksButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_cyba.IsDatabaseAvailable())
            {
                MessageBox.Show(
                    "MySQL database is not available.\n\n" +
                    "Setup steps:\n" +
                    "1. Install MySQL Server (https://dev.mysql.com/downloads/)\n" +
                    "2. Start the MySQL service\n" +
                    "3. Ensure user 'root' can connect on localhost:3306\n" +
                    "4. Restart CYBA — the database/table will be created automatically.",
                    "Database Required", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _cyba.GetActivityLog().Log("Task Window opened");
            var taskWindow = new TaskWindow(new TaskService(), _cyba.GetActivityLog());
            taskWindow.Owner = this;
            taskWindow.ShowDialog();
            AddSystemMessage("📋 Task Assistant closed.");
        }

        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            OpenQuizWindow();
        }

        private void OpenQuizWindow()
        {
            var quizWindow = new QuizWindow(_quizService, _cyba.GetActivityLog());
            quizWindow.Owner = this;
            quizWindow.ShowDialog();
            AddSystemMessage("🎮 Quiz session ended. Type 'show activity log' to see your results!");
        }

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            _cyba.GetActivityLog().Log("Activity Log Window opened");
            var logWindow = new ActivityLogWindow(_cyba.GetActivityLog());
            logWindow.Owner = this;
            logWindow.ShowDialog();
        }

        private void ConsoleButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Launch console version? Your GUI session will close.",
                "Switch to Console", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var consoleCyba = new CYBA();
                var thread = new System.Threading.Thread(() => consoleCyba.StartCYBA());
                thread.IsBackground = false;
                thread.Start();
                this.Close();
            }
        }

        // ─── Input Event Handlers ─────────────────────────────────────────────────

        private void SendButton_Click(object sender, RoutedEventArgs e) => ProcessUserInput();

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ProcessUserInput();
        }

        // ─── Chat Bubble Builders ─────────────────────────────────────────────────

        private void AddUserMessage(string message)
        {
            var bubble = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F6F8B")),
                CornerRadius = new CornerRadius(15, 15, 0, 15),
                Padding = new Thickness(12, 8, 12, 8),
                Margin = new Thickness(80, 5, 10, 5),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            bubble.Child = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap
            };
            ChatPanel.Children.Add(bubble);
        }

        private void AddBotMessage(string message)
        {
            var bubble = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D333B")),
                CornerRadius = new CornerRadius(15, 15, 15, 0),
                Padding = new Thickness(12, 8, 12, 8),
                Margin = new Thickness(10, 5, 80, 5),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var content = new StackPanel();
            content.Children.Add(new TextBlock
            {
                Text = "🛡️ CYBA",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00B4D8")),
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 4)
            });
            content.Children.Add(new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap
            });
            bubble.Child = content;
            ChatPanel.Children.Add(bubble);
            ChatScrollViewer.ScrollToBottom();
        }

        private void AddSystemMessage(string message)
        {
            ChatPanel.Children.Add(new TextBlock
            {
                Text = message,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8B949E")),
                FontSize = 11,
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(10, 2, 10, 2),
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            });
        }
    }
}
