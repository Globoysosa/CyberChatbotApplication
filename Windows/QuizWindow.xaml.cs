using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CyberChatbotApplication.Models;
using CyberChatbotApplication.Services;

namespace CyberChatbotApplication.Windows
{
    /// <summary>
    /// Cybersecurity Mini-Game quiz window.
    /// Presents 10 shuffled questions (multiple choice + true/false),
    /// provides immediate feedback after each answer, and tracks the user's score.
    /// </summary>
    public partial class QuizWindow : Window
    {
        // ─── Fields ──────────────────────────────────────────────────────────────

        private readonly QuizService _quizService;
        private readonly ActivityLogService _activityLog;

        private List<QuizQuestion> _questions;
        private int _currentIndex;
        private int _score;
        private bool _answered;

        // Colour constants
        private static readonly SolidColorBrush ColourCorrect = new SolidColorBrush(Color.FromRgb(31, 139, 76));
        private static readonly SolidColorBrush ColourWrong = new SolidColorBrush(Color.FromRgb(139, 26, 26));
        private static readonly SolidColorBrush ColourDefault = new SolidColorBrush(Color.FromRgb(31, 111, 139));
        private static readonly SolidColorBrush ColourHighlight = new SolidColorBrush(Color.FromRgb(0, 180, 216));

        // ─── Constructor ─────────────────────────────────────────────────────────

        public QuizWindow(QuizService quizService, ActivityLogService activityLog)
        {
            InitializeComponent();
            _quizService = quizService;
            _activityLog = activityLog;
            StartQuiz();
        }

        // ─── Quiz Logic ───────────────────────────────────────────────────────────

        private void StartQuiz()
        {
            _questions = _quizService.GetQuizQuestions(10);
            _currentIndex = 0;
            _score = 0;
            TotalLabel.Text = _questions.Count.ToString();
            ProgressBar.Maximum = _questions.Count;
            _activityLog.Log("Quiz started", $"{_questions.Count} questions");
            ShowQuestion();
        }

        private void ShowQuestion()
        {
            if (_currentIndex >= _questions.Count)
            {
                ShowFinalResult();
                return;
            }

            _answered = false;
            FeedbackBorder.Visibility = Visibility.Collapsed;

            var q = _questions[_currentIndex];
            QuestionCountLabel.Text = $"Question {_currentIndex + 1} of {_questions.Count}";
            ProgressBar.Value = _currentIndex;
            ScoreLabel.Text = _score.ToString();
            QuestionText.Text = q.Question;

            // Build option buttons
            OptionsPanel.Children.Clear();
            for (int i = 0; i < q.Options.Count; i++)
            {
                int capturedIndex = i; // capture for closure
                var btn = new Button
                {
                    Content = q.Options[i],
                    Background = ColourDefault,
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(15, 12, 15, 12),
                    Margin = new Thickness(0, 0, 0, 8),
                    FontSize = 13,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Tag = capturedIndex
                };
                btn.Click += OptionBtn_Click;
                OptionsPanel.Children.Add(btn);
            }
        }

        private void OptionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_answered) return;
            _answered = true;

            var btn = (Button)sender;
            int selectedIndex = (int)btn.Tag;
            var q = _questions[_currentIndex];
            bool isCorrect = selectedIndex == q.CorrectIndex;

            if (isCorrect)
            {
                _score++;
                btn.Background = ColourCorrect;
                FeedbackBorder.Background = new SolidColorBrush(Color.FromRgb(14, 42, 20));
                FeedbackBorder.BorderBrush = ColourCorrect;
                FeedbackText.Text = "✅ Correct!\n\n" + q.Explanation;
            }
            else
            {
                btn.Background = ColourWrong;
                // Highlight correct answer
                if (q.CorrectIndex < OptionsPanel.Children.Count)
                    ((Button)OptionsPanel.Children[q.CorrectIndex]).Background = ColourCorrect;
                FeedbackBorder.Background = new SolidColorBrush(Color.FromRgb(42, 14, 14));
                FeedbackBorder.BorderBrush = ColourWrong;
                FeedbackText.Text = $"❌ Not quite!\n\nCorrect answer: {q.Options[q.CorrectIndex]}\n\n{q.Explanation}";
            }

            // Disable all buttons
            foreach (Button b in OptionsPanel.Children)
                b.IsEnabled = false;

            ScoreLabel.Text = _score.ToString();
            FeedbackBorder.Visibility = Visibility.Visible;

            // Change "Next" button label on last question
            if (_currentIndex == _questions.Count - 1)
                NextBtn.Content = "See Results 🏆";
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentIndex++;
            ShowQuestion();
        }

        private void ShowFinalResult()
        {
            OptionsPanel.Children.Clear();
            FeedbackBorder.Visibility = Visibility.Collapsed;

            string feedback = _quizService.GetScoreFeedback(_score, _questions.Count);
            _activityLog.Log("Quiz completed", $"Score: {_score}/{_questions.Count}");

            QuestionText.Text = "Quiz Complete! 🎉";
            ProgressBar.Value = _questions.Count;
            ScoreLabel.Text = _score.ToString();

            // Show result in a styled panel
            var resultPanel = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(14, 26, 42)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(20),
                BorderBrush = ColourHighlight,
                BorderThickness = new Thickness(1)
            };
            var sp = new StackPanel();

            sp.Children.Add(new TextBlock
            {
                Text = $"Final Score: {_score} / {_questions.Count}",
                Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0)),
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 15)
            });
            sp.Children.Add(new TextBlock
            {
                Text = feedback,
                Foreground = Brushes.White,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            });

            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };

            var retryBtn = new Button
            {
                Content = "🔄 Try Again",
                Background = ColourDefault,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(20, 10, 20, 10),
                FontSize = 13,
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 0, 10, 0)
            };
            retryBtn.Click += (s, ev) => StartQuiz();

            var closeBtn = new Button
            {
                Content = "✖ Close",
                Background = new SolidColorBrush(Color.FromRgb(48, 54, 61)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(20, 10, 20, 10),
                FontSize = 13,
                Cursor = System.Windows.Input.Cursors.Hand
            };
            closeBtn.Click += (s, ev) => Close();

            btnPanel.Children.Add(retryBtn);
            btnPanel.Children.Add(closeBtn);
            sp.Children.Add(btnPanel);
            resultPanel.Child = sp;
            OptionsPanel.Children.Add(resultPanel);
        }
    }
}
