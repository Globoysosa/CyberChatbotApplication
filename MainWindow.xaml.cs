using System;
using System.Media;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CyberChatbotApplication
{
    public partial class MainWindow : Window
    {
        private CYBA _cyba;

        public MainWindow()
        {
            InitializeComponent();
            _cyba = new CYBA();
            PlayVoiceGreeting();
            AddBotMessage("Hello! Welcome to the Cybersecurity Awareness Bot. I'm here to help you stay safe online.");
            AddBotMessage("What's your name?");
        }

        private void PlayVoiceGreeting()
        {
            string path = @"C:\Users\Student\Desktop\poe_part1\poe_part1\CyberChatbotApplication-master\voice_folder\greeting.wav.wav";

            if (File.Exists(path))
            {
                try
                {
                    SoundPlayer player = new SoundPlayer(path);
                    player.Play();
                    AddSystemMessage("🔊 Voice greeting played!");
                }
                catch (Exception ex)
                {
                    AddSystemMessage($"⚠️ Could not play voice greeting: {ex.Message}");
                }
            }
            else
            {
                AddSystemMessage($"ℹ️ Voice greeting file not found at: {path}");
            }
        }

        private void VoiceButton_Click(object sender, RoutedEventArgs e)
        {
            PlayVoiceGreeting();
            AddSystemMessage("🔊 Voice greeting played");
        }

        private void ConsoleButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Launch console version? Your GUI session will close.", "Switch to Console", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var cyba = new CYBA();
                var consoleThread = new System.Threading.Thread(() =>
                {
                    cyba.StartCYBA();
                });
                consoleThread.Start();
                this.Close();
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserInput();
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessUserInput();
            }
        }

        private void ProcessUserInput()
        {
            string userMessage = UserInput.Text.Trim();

            if (string.IsNullOrEmpty(userMessage))
                return;

            AddUserMessage(userMessage);
            UserInput.Clear();

            string response = _cyba.ProcessInput(userMessage);
            AddBotMessage(response);

            ChatScrollViewer.ScrollToBottom();
        }

        private void AddUserMessage(string message)
        {
            Border bubble = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F6F8B")),
                CornerRadius = new CornerRadius(15),
                Padding = new Thickness(12, 8, 12, 8),
                Margin = new Thickness(50, 5, 10, 5),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            TextBlock text = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap
            };

            bubble.Child = text;
            ChatPanel.Children.Add(bubble);
        }

        private void AddBotMessage(string message)
        {
            Border bubble = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D333B")),
                CornerRadius = new CornerRadius(15),
                Padding = new Thickness(12, 8, 12, 8),
                Margin = new Thickness(10, 5, 50, 5),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            StackPanel content = new StackPanel();

            TextBlock nameTag = new TextBlock
            {
                Text = "🛡️ CYBA",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00B4D8")),
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };

            TextBlock text = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap
            };

            content.Children.Add(nameTag);
            content.Children.Add(text);
            bubble.Child = content;

            ChatPanel.Children.Add(bubble);
        }

        private void AddSystemMessage(string message)
        {
            TextBlock systemText = new TextBlock
            {
                Text = message,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8B949E")),
                FontSize = 11,
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(10, 2, 10, 2),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            ChatPanel.Children.Add(systemText);
        }
    }
}