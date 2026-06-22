using System;
using System.Collections.Generic;
using System.Threading;
using System.Media;
using System.Linq;
using System.IO;
using CyberChatbotApplication.Models;
using CyberChatbotApplication.Services;

namespace CyberChatbotApplication
{
    public class CYBA
    {
        private string name;
        private string lastTopic;
        private List<string> userInterests;
        private Dictionary<string, List<string>> responses;
        private Random random;

        private readonly NLPService _nlp;
        private readonly ActivityLogService _activityLog;
        private readonly TaskService _taskService;
        private bool _dbAvailable;

        private bool _awaitingReminderForTask;
        private CyberTask _pendingTask;
        private bool _awaitingTaskDelete;
        private bool _awaitingTaskComplete;

        public CYBA()
        {
            userInterests = new List<string>();
            random = new Random();
            _nlp = new NLPService();
            _activityLog = new ActivityLogService();
            _taskService = new TaskService();
            InitializeResponses();
            InitialiseDatabase();
        }

        private void InitialiseDatabase()
        {
            try
            {
                TaskService.InitialiseDatabase();
                _dbAvailable = true;
            }
            catch (Exception ex)
            {
                _dbAvailable = false;
                System.Diagnostics.Debug.WriteLine("[CYBA] DB unavailable: " + ex.Message);
            }
        }

        private void InitializeResponses()
        {
            responses = new Dictionary<string, List<string>>
            {
                { "nice to meet you", new List<string> { "Well I'm glad to meet you too. I am CYBA, your Cybersecurity Awareness Assistant." } },
                { "how are you", new List<string> { "I'm doing great and ready to help! Ask me anything about cybersecurity." } },
                { "tell me more", new List<string> { "Cybersecurity is the practice of protecting computer systems, networks, and data from attacks." } },
                { "safety", new List<string> { "Don't share personal information like address, phone number or bank details." } },
                { "vpn", new List<string> {
                    "A VPN (Virtual Private Network) creates an encrypted tunnel for your internet traffic, protecting you on public Wi-Fi.",
                    "Always use a VPN when connecting to public Wi-Fi in coffee shops, airports, or hotels." } },
                { "online protection", new List<string> { "Online protection means securing yourself while using the internet through strong passwords, careful clicking, and privacy settings." } },
                { "cyber security", new List<string> { "Use strong unique passwords and enable multi-factor authentication for every important account." } },
                { "password", new List<string> {
                    "Make strong passwords with letters, numbers, and symbols. Minimum 8 characters. Never reuse passwords!",
                    "Use a passphrase of 4-6 random words - they're strong AND memorable! E.g. 'purple-rocket-fish-cloud'.",
                    "Enable Two-Factor Authentication (2FA) whenever possible for an extra layer of security." } },
                { "phishing", new List<string> {
                    "Phishing is a trick to steal your information by pretending to be someone trustworthy. Always verify the sender!",
                    "Never click suspicious links! Check the sender's email address carefully - scammers use lookalike domains.",
                    "Legitimate companies NEVER ask for your password via email. Report phishing attempts immediately." } },
                { "scam", new List<string> {
                    "Scammers often create urgency - 'Act NOW or lose your account!' Take time to verify before acting!",
                    "If something sounds too good to be true, it probably is. Legitimate lottery wins don't require upfront payment.",
                    "Romance scams are on the rise in South Africa. Never send money to someone you haven't met in person." } },
                { "privacy", new List<string> {
                    "Review your privacy settings on social media regularly - platforms often reset them after updates!",
                    "Be careful what personal info you share online. Your date of birth and home address can enable identity theft." } },
                { "malware", new List<string> {
                    "Malware is malicious software designed to damage or gain unauthorised access to systems. Keep your antivirus updated!",
                    "Never download software from unverified sources. Always scan downloads before opening them." } },
                { "ransomware", new List<string> {
                    "Ransomware encrypts your files and demands payment for the key. Back up your data regularly to an offline drive!",
                    "South Africa has seen major ransomware attacks. Keep software updated and never open unexpected email attachments." } },
                { "two factor", new List<string> { "Two-Factor Authentication (2FA) adds a second verification step after your password - making stolen passwords useless to attackers!" } },
                { "2fa", new List<string> { "Enable 2FA on all your accounts! Use an authenticator app like Google Authenticator rather than SMS for best security." } },
                { "safe browsing", new List<string> {
                    "Look for 'https://' and the padlock before entering any personal information on a website.",
                    "Use a reputable browser with built-in phishing protection, like Chrome or Firefox, and keep it updated." } },
                { "backup", new List<string> { "Follow the 3-2-1 backup rule: 3 copies of data, on 2 different media types, with 1 stored off-site or in the cloud." } },
                { "exit", new List<string> { "Stay safe out there! Goodbye!" } },
                { "what are you", new List<string> { "I'm CYBA - your Cybersecurity Awareness Bot, here to educate you on staying safe online!" } },
                { "what do you do", new List<string> { "I educate you about cyber threats, help you manage cybersecurity tasks, and quiz your knowledge!" } },
                { "what can you do", new List<string> { "Ask me about passwords, phishing, VPN, scams, privacy, malware, 2FA - or type 'help' for all features." } },
                { "thank", new List<string> { "You're welcome! Stay vigilant online!", "Happy to help! Remember - cybersecurity starts with awareness!" } },
            };
        }

        // ── Public Accessors ──────────────────────────────────────────────────────

        public string GetName() => name;
        public void SetName(string userName) => name = userName;
        public ActivityLogService GetActivityLog() => _activityLog;
        public bool IsDatabaseAvailable() => _dbAvailable;

        // ── Main Input Processor ──────────────────────────────────────────────────

        public string ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Please type a message so I can help you!";

            string cleanInput = input.ToLower().Trim();

            // Name capture on first interaction
            if (string.IsNullOrEmpty(name))
            {
                bool looksLikeCommand = cleanInput.Contains("how are") || cleanInput.Contains("what is") ||
                                        cleanInput.Contains("help") || cleanInput.Contains("quiz");
                if (!looksLikeCommand)
                {
                    name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cleanInput);
                    _activityLog.Log("User identified", "Name set to '" + name + "'");
                    return "Hello, " + name + "! I am CYBA, your Cybersecurity Awareness Assistant.\n\n" +
                           "I can help you with:\n" +
                           "- Cybersecurity tips and advice\n" +
                           "- Managing cybersecurity tasks\n" +
                           "- A cybersecurity knowledge quiz\n" +
                           "- Activity logging\n\n" +
                           "Type 'help' anytime to see what I can do!";
                }
            }

            // Multi-turn: awaiting reminder days
            if (_awaitingReminderForTask && _pendingTask != null)
                return HandleReminderResponse(input);

            // Multi-turn: awaiting task number to delete
            if (_awaitingTaskDelete)
                return HandleDeleteTaskResponse(input);

            // Multi-turn: awaiting task number to complete
            if (_awaitingTaskComplete)
                return HandleCompleteTaskResponse(input);

            // NLP intent detection
            string intent = _nlp.DetectIntent(cleanInput);

            switch (intent)
            {
                case NLPService.IntentHelp:
                    _activityLog.Log("Help requested");
                    return _nlp.GetHelpMessage();

                case NLPService.IntentAddTask:
                case NLPService.IntentSetReminder:
                    return HandleAddTask(input);

                case NLPService.IntentViewTasks:
                    return HandleViewTasks();

                case NLPService.IntentDeleteTask:
                    return HandleDeleteTask();

                case NLPService.IntentCompleteTask:
                    return HandleCompleteTask();

                case NLPService.IntentStartQuiz:
                    _activityLog.Log("Quiz initiated", "User requested the cybersecurity quiz");
                    return "__START_QUIZ__";

                case NLPService.IntentViewFullLog:
                    _activityLog.Log("Full activity log viewed");
                    return _activityLog.BuildFullLogSummary();

                case NLPService.IntentViewLog:
                    _activityLog.Log("Activity log viewed");
                    return _activityLog.BuildLogSummary(10);
            }

            // Sentiment detection
            string sentimentResponse = CheckSentiment(cleanInput);
            if (sentimentResponse != null) return sentimentResponse;

            // Conversation continuity
            if (cleanInput.Contains("tell me more") || cleanInput.Contains("explain more") ||
                cleanInput.Contains("another tip") || cleanInput.Contains("give me more"))
            {
                if (!string.IsNullOrEmpty(lastTopic) && responses.ContainsKey(lastTopic))
                    return responses[lastTopic][random.Next(responses[lastTopic].Count)];
                return "What would you like to know more about? Ask me about passwords, phishing, scams, or privacy!";
            }

            // Interest memory
            if (cleanInput.Contains("i'm interested in") || cleanInput.Contains("i am interested in"))
            {
                string topic = ExtractInterest(cleanInput);
                if (!string.IsNullOrEmpty(topic))
                {
                    userInterests.Add(topic);
                    _activityLog.Log("User interest noted", "Interested in '" + topic + "'");
                    return "Great! I'll remember that you're interested in " + topic + ". " +
                           "It's a crucial part of staying safe online. Ask me anything about it!";
                }
            }

            // Keyword matching
            foreach (var keyword in responses.Keys)
            {
                if (cleanInput.Contains(keyword))
                {
                    lastTopic = keyword;
                    string response = responses[keyword][random.Next(responses[keyword].Count)];

                    if (userInterests.Count > 0 && userInterests.Contains(keyword))
                        response += "\n\nAs someone interested in " + keyword + ", you might also want to review your account settings!";

                    _activityLog.Log("Cybersecurity advice given", "Topic: " + keyword);
                    return response;
                }
            }

            // Default fallback
            return "Sorry, I didn't understand that. Can you rephrase? Try asking about passwords, phishing, or cybersecurity tips.";
        }

        // ── Sentiment Detection ───────────────────────────────────────────────────

        private string CheckSentiment(string cleanInput)
        {
            if (cleanInput.Contains("worried") || cleanInput.Contains("scared") || cleanInput.Contains("anxious") || cleanInput.Contains("afraid"))
            {
                _activityLog.Log("Sentiment detected", "User expressed worry/fear");
                return "It's completely normal to feel concerned about cybersecurity threats.\n\n" +
                       "Here's a quick tip: Enable Two-Factor Authentication (2FA) on your email and banking accounts right now. " +
                       "It's the single most effective step you can take!\n\n" +
                       "Would you like me to add 'Enable 2FA' as a task? Just type 'add task to enable 2FA'!";
            }

            if (cleanInput.Contains("frustrated") || cleanInput.Contains("confused") || cleanInput.Contains("overwhelmed"))
            {
                _activityLog.Log("Sentiment detected", "User expressed frustration/confusion");
                return "I understand cybersecurity can be overwhelming. Let's take it step by step!\n\n" +
                       "What specific topic would you like to understand better? Try starting with 'password safety'.";
            }

            if (cleanInput.Contains("curious") || cleanInput.Contains("interested") || cleanInput.Contains("want to learn"))
            {
                _activityLog.Log("Sentiment detected", "User expressed curiosity");
                return "That's great! Curiosity helps you learn. What cybersecurity topic interests you most?\n\n" +
                       "Or try the quiz to test your knowledge - type 'start quiz'!";
            }

            if (cleanInput.Contains("happy") || cleanInput.Contains("great") || cleanInput.Contains("awesome") || cleanInput.Contains("thanks"))
            {
                _activityLog.Log("Sentiment detected", "User expressed positivity");
                return responses["thank"][random.Next(responses["thank"].Count)];
            }

            return null;
        }

        // ── Task Handlers ─────────────────────────────────────────────────────────

        private string HandleAddTask(string input)
        {
            if (!_dbAvailable)
                return "The task database is not available. Please ensure MySQL is running and restart the app.";

            string title = _nlp.ExtractTaskTitle(input);
            _pendingTask = new CyberTask(title, "Added via chatbot: " + title);

            int? days = _nlp.ExtractReminderDays(input);
            if (days.HasValue)
            {
                _pendingTask.ReminderDate = DateTime.Now.AddDays(days.Value);
                return SavePendingTask();
            }

            _awaitingReminderForTask = true;
            return "Task ready: \"" + title + "\"\n\nWould you like a reminder? Say how many days (e.g. 'remind me in 3 days') or type 'no reminder'.";
        }

        private string HandleReminderResponse(string input)
        {
            _awaitingReminderForTask = false;

            if (input.ToLower().Contains("no") || input.ToLower().Contains("skip"))
                return SavePendingTask();

            int? days = _nlp.ExtractReminderDays(input);
            if (days.HasValue)
            {
                _pendingTask.ReminderDate = DateTime.Now.AddDays(days.Value);
                return SavePendingTask();
            }

            if (int.TryParse(input.Trim(), out int plainDays))
            {
                _pendingTask.ReminderDate = DateTime.Now.AddDays(plainDays);
                return SavePendingTask();
            }

            return SavePendingTask();
        }

        private string SavePendingTask()
        {
            try
            {
                int id = _taskService.AddTask(_pendingTask);
                _pendingTask.Id = id;

                string reminderText = _pendingTask.ReminderDate.HasValue
                    ? "Reminder set for " + _pendingTask.ReminderDate.Value.ToString("dd MMM yyyy") + "."
                    : "No reminder set.";

                _activityLog.Log("Task added", "'" + _pendingTask.Title + "' - " + reminderText);
                string result = "Task added: \"" + _pendingTask.Title + "\"\n" + reminderText + "\n\nView all tasks by typing 'show my tasks'.";
                _pendingTask = null;
                return result;
            }
            catch (Exception ex)
            {
                _pendingTask = null;
                return "Could not save task: " + ex.Message;
            }
        }

        private string HandleViewTasks()
        {
            if (!_dbAvailable)
                return "Database unavailable. Please ensure MySQL is running.";

            try
            {
                var tasks = _taskService.GetAllTasks();
                _activityLog.Log("Tasks viewed", tasks.Count + " tasks retrieved");

                if (tasks.Count == 0)
                    return "You have no tasks yet! Try: 'Add task to enable 2FA'";

                string result = "Your Cybersecurity Tasks (" + tasks.Count + "):\n\n";
                for (int i = 0; i < tasks.Count; i++)
                {
                    string status = tasks[i].IsCompleted ? "[Done]" : "[Pending]";
                    result += (i + 1) + ". " + status + " " + tasks[i].Title + "\n";
                    if (!string.IsNullOrEmpty(tasks[i].Description))
                        result += "   " + tasks[i].Description + "\n";
                    if (tasks[i].ReminderDate.HasValue)
                        result += "   Reminder: " + tasks[i].ReminderDate.Value.ToString("dd MMM yyyy") + "\n";
                }
                result += "\nTo complete a task: type 'mark task complete'\nTo delete a task: type 'delete task'";
                return result;
            }
            catch (Exception ex)
            {
                return "Could not retrieve tasks: " + ex.Message;
            }
        }

        private string HandleDeleteTask()
        {
            if (!_dbAvailable)
                return "Database unavailable. Please ensure MySQL is running.";

            try
            {
                var tasks = _taskService.GetAllTasks();
                if (tasks.Count == 0)
                    return "You have no tasks to delete.";

                _awaitingTaskDelete = true;
                string list = "Which task would you like to delete? Reply with the number:\n\n";
                for (int i = 0; i < tasks.Count; i++)
                {
                    string status = tasks[i].IsCompleted ? "[Done]" : "[Pending]";
                    list += (i + 1) + ". " + tasks[i].Title + " " + status + "\n";
                }
                return list;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string HandleDeleteTaskResponse(string input)
        {
            _awaitingTaskDelete = false;
            try
            {
                var tasks = _taskService.GetAllTasks();
                if (int.TryParse(input.Trim(), out int idx) && idx >= 1 && idx <= tasks.Count)
                {
                    var task = tasks[idx - 1];
                    _taskService.DeleteTask(task.Id);
                    _activityLog.Log("Task deleted", "'" + task.Title + "'");
                    return "Task \"" + task.Title + "\" has been deleted.";
                }
                return "I couldn't identify that task number. Please try 'delete task' again.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string HandleCompleteTask()
        {
            if (!_dbAvailable)
                return "Database unavailable. Please ensure MySQL is running.";

            try
            {
                var tasks = _taskService.GetAllTasks().Where(t => !t.IsCompleted).ToList();
                if (tasks.Count == 0)
                    return "All your tasks are already completed! Great work!";

                _awaitingTaskComplete = true;
                string list = "Which task would you like to mark as complete? Reply with the number:\n\n";
                for (int i = 0; i < tasks.Count; i++)
                    list += (i + 1) + ". " + tasks[i].Title + "\n";
                return list;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string HandleCompleteTaskResponse(string input)
        {
            _awaitingTaskComplete = false;
            try
            {
                var tasks = _taskService.GetAllTasks().Where(t => !t.IsCompleted).ToList();
                if (int.TryParse(input.Trim(), out int idx) && idx >= 1 && idx <= tasks.Count)
                {
                    var task = tasks[idx - 1];
                    _taskService.MarkCompleted(task.Id);
                    _activityLog.Log("Task completed", "'" + task.Title + "'");
                    return "Well done! Task \"" + task.Title + "\" marked as completed!";
                }
                return "I couldn't identify that task number. Please try again.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private string ExtractInterest(string input)
        {
            string[] prefixes = { "i'm interested in ", "i am interested in " };
            foreach (string prefix in prefixes)
            {
                int idx = input.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                    return input.Substring(idx + prefix.Length).Trim();
            }
            return null;
        }

        // ── Console mode (Part 1 preserved) ──────────────────────────────────────

        public void logo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
   ██████╗██╗   ██╗██████╗  █████╗ 
  ██╔════╝╚██╗ ██╔╝██╔══██╗██╔══██╗
  ██║      ╚████╔╝ ██████╔╝███████║
  ██║       ╚██╔╝  ██╔══██╗██╔══██║
  ╚██████╗   ██║   ██████╔╝██║  ██║
   ╚═════╝   ╚═╝   ╚═════╝ ╚═╝  ╚═╝
                                                     
          CYBA - CYBER AWARENESS BOT
            ");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void PlayGreeting()
        {
            string path = @"C:\Users\Student\Desktop\poe_part1\poe_part1\CyberChatbotApplication-master\voice_folder\greeting.wav.wav";

            if (File.Exists(path))
            {
                try
                {
                    SoundPlayer player = new SoundPlayer(path);
                    player.PlaySync();
                }
                catch { }
            }
        }

        public void delayEffect(string text)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(30);
            }
            Console.WriteLine();
        }

        public void StartCYBA()
        {
            logo();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n================================================================================================");
            Console.WriteLine("                  WELCOME TO CYBERSECURITY AWARENESS ASSISTANT");
            Console.WriteLine("================================================================================================");
            PlayGreeting();
            Console.Write("\n Please enter your name : ");
            string inputName = Console.ReadLine()?.Trim();
            while (string.IsNullOrEmpty(inputName))
            {
                Console.Write(" Name cannot be empty. Please enter your name: ");
                inputName = Console.ReadLine()?.Trim();
            }
            SetName(inputName);
            Console.WriteLine($"\n Hello, {GetName()}! I am CYBA.");
            Console.WriteLine(" Ask me anything about online safety, passwords, phishing, or cybersecurity.");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n Type 'exit' to quit.\n");
            Console.ForegroundColor = ConsoleColor.White;
            RunCYBA();
        }

        private void RunCYBA()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($" {GetName()}: ");
                string input = Console.ReadLine()?.Trim().ToLower();
                if (string.IsNullOrEmpty(input))
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(" CYBA : Please ask a question.");
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }
                if (input == "exit")
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(" CYBA: Stay safe out there! Goodbye!");
                    break;
                }
                string response = ProcessInput(input);
                Console.Write(" CYBA : ");
                Console.ForegroundColor = ConsoleColor.Green;
                delayEffect(response);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}