using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CyberChatbotApplication.Services
{
    /// <summary>
    /// Simulates Natural Language Processing using keyword detection and
    /// basic string manipulation. Identifies the user's intent from free-text
    /// input so the chatbot can route to the correct feature.
    /// </summary>
    public class NLPService
    {
        // ─── Intent Constants ────────────────────────────────────────────────────

        public const string IntentAddTask = "ADD_TASK";
        public const string IntentViewTasks = "VIEW_TASKS";
        public const string IntentDeleteTask = "DELETE_TASK";
        public const string IntentCompleteTask = "COMPLETE_TASK";
        public const string IntentSetReminder = "SET_REMINDER";
        public const string IntentStartQuiz = "START_QUIZ";
        public const string IntentViewLog = "VIEW_LOG";
        public const string IntentViewFullLog = "VIEW_FULL_LOG";
        public const string IntentHelp = "HELP";
        public const string IntentUnknown = "UNKNOWN";

        // ─── Keyword Maps ────────────────────────────────────────────────────────

        private readonly Dictionary<string, string[]> _intentKeywords = new Dictionary<string, string[]>
        {
            // Task management
            { IntentAddTask,     new[] { "add task", "create task", "new task", "add a task", "create a task",
                                         "remind me to", "add reminder", "set reminder", "schedule", "todo" } },
            { IntentViewTasks,   new[] { "show tasks", "view tasks", "my tasks", "list tasks", "see tasks",
                                         "what tasks", "show my tasks", "pending tasks" } },
            { IntentDeleteTask,  new[] { "delete task", "remove task", "cancel task", "delete the task" } },
            { IntentCompleteTask,new[] { "complete task", "done with task", "finish task", "mark complete",
                                         "mark as done", "task done", "completed" } },
            { IntentSetReminder, new[] { "remind me", "set a reminder", "reminder for", "remind in" } },

            // Quiz
            { IntentStartQuiz,   new[] { "quiz", "start quiz", "take a quiz", "play game", "mini game",
                                         "test me", "cybersecurity quiz", "start game", "play quiz" } },

            // Activity log
            { IntentViewLog,     new[] { "show activity log", "activity log", "what have you done",
                                         "show log", "recent actions", "history", "what did you do",
                                         "show actions", "log" } },
            { IntentViewFullLog, new[] { "show full log", "full log", "all actions", "complete history" } },

            // Help
            { IntentHelp,        new[] { "help", "what can you do", "commands", "options", "menu",
                                         "features", "how do i", "what can i ask" } },
        };

        // ─── Public Methods ──────────────────────────────────────────────────────

        /// <summary>
        /// Detects the user's intent from their input string.
        /// Uses substring matching with cleaned (lower-case, trimmed) input.
        /// </summary>
        public string DetectIntent(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return IntentUnknown;

            string clean = input.ToLower().Trim();

            foreach (var kvp in _intentKeywords)
            {
                foreach (string keyword in kvp.Value)
                {
                    if (clean.Contains(keyword))
                        return kvp.Key;
                }
            }

            return IntentUnknown;
        }

        /// <summary>
        /// Extracts a task title from a user's natural-language instruction.
        /// E.g. "Add a task to enable 2FA" → "enable 2FA"
        ///      "Remind me to update my password" → "update my password"
        /// </summary>
        public string ExtractTaskTitle(string input)
        {
            string clean = input.Trim();

            // Strip common prefixes using regex
            string[] patterns = new[]
            {
                @"(?i)add\s+(a\s+)?task\s+(to\s+|for\s+)?",
                @"(?i)create\s+(a\s+)?task\s+(to\s+|for\s+)?",
                @"(?i)remind\s+me\s+to\s+",
                @"(?i)new\s+task[:.\s]+",
                @"(?i)set\s+(a\s+)?reminder\s+(to\s+|for\s+)?",
                @"(?i)add\s+(a\s+)?reminder\s+(to\s+|for\s+)?"
            };

            foreach (string pattern in patterns)
            {
                clean = Regex.Replace(clean, pattern, "", RegexOptions.IgnoreCase).Trim();
            }

            // Capitalise first letter
            if (clean.Length > 0)
                clean = char.ToUpper(clean[0]) + clean.Substring(1);

            return string.IsNullOrWhiteSpace(clean) ? "Cybersecurity task" : clean;
        }

        /// <summary>
        /// Tries to extract a number of days from a reminder phrase.
        /// E.g. "remind me in 3 days" → 3
        ///      "remind me in a week" → 7
        ///      Returns null if no timeframe found.
        /// </summary>
        public int? ExtractReminderDays(string input)
        {
            string clean = input.ToLower();

            // "in X days / weeks"
            Match daysMatch = Regex.Match(clean, @"in\s+(\d+)\s+day");
            if (daysMatch.Success && int.TryParse(daysMatch.Groups[1].Value, out int days))
                return days;

            Match weeksMatch = Regex.Match(clean, @"in\s+(\d+)\s+week");
            if (weeksMatch.Success && int.TryParse(weeksMatch.Groups[1].Value, out int weeks))
                return weeks * 7;

            // Natural language shorthands
            if (clean.Contains("tomorrow")) return 1;
            if (clean.Contains("a week") || clean.Contains("one week")) return 7;
            if (clean.Contains("two weeks")) return 14;
            if (clean.Contains("a month") || clean.Contains("one month")) return 30;
            if (clean.Contains("3 days")) return 3;
            if (clean.Contains("5 days")) return 5;
            if (clean.Contains("7 days")) return 7;

            return null;
        }

        /// <summary>Returns a user-friendly help message listing available commands.</summary>
        public string GetHelpMessage()
        {
            return "🛡️ Here's what I can help you with:\n\n" +
                   "💬 CHATBOT: Ask me about passwords, phishing, scams, VPNs, privacy, safe browsing...\n\n" +
                   "📋 TASKS: Type any of:\n" +
                   "   • 'Add task to enable 2FA'\n" +
                   "   • 'Show my tasks'\n" +
                   "   • 'Mark task complete'\n" +
                   "   • 'Delete task'\n\n" +
                   "🎮 QUIZ: Type 'start quiz' or 'take a quiz'\n\n" +
                   "📊 LOG: Type 'show activity log' or 'what have you done for me'\n\n" +
                   "💡 I understand natural language — just speak normally!";
        }
    }
}
