using System;
using System.Collections.Generic;
using System.Threading;
using System.Media;
using System.Linq;
using System.IO;

namespace CyberChatbotApplication
{
    public class CYBA
    {
        private string name;
        private string lastTopic;
        private List<string> userInterests;
        private Dictionary<string, List<string>> responses;
        private Random random;

        public CYBA()
        {
            userInterests = new List<string>();
            random = new Random();
            InitializeResponses();
        }

        private void InitializeResponses()
        {
            responses = new Dictionary<string, List<string>>
            {
                { "nice to meet you", new List<string> { "Well I'm glad to meet you too. I am CYBA, your Cybersecurity Awareness Assistant." } },
                { "how are you", new List<string> { "I'm doing great and ready to help! Ask me anything about cybersecurity." } },
                { "tell me more", new List<string> { "Cybersecurity is the practice of protecting computer systems, networks, and data from attacks." } },
                { "safety", new List<string> { "Don't share personal information like address, phone number or bank details." } },
                { "vpn", new List<string> { "VPN creates a secure tunnel for your internet traffic." } },
                { "online protection", new List<string> { "It is the security you have while using the internet." } },
                { "cyber security", new List<string> { "Use strong unique passwords and enable multi-factor authentication." } },
                { "password", new List<string> {
                    "Make strong passwords with letters, numbers, and symbols. Minimum 8 characters. Never reuse passwords.",
                    "Use a passphrase of 4-6 random words - they're strong AND memorable!",
                    "Enable Two-Factor Authentication whenever possible for extra security."
                }},
                { "phishing", new List<string> {
                    "Phishing is a trick to steal your information by pretending to be someone trustworthy.",
                    "Never click suspicious links! Always verify the sender's email address.",
                    "Legitimate companies never ask for passwords via email. Report phishing attempts!"
                }},
                { "scam", new List<string> {
                    "Scammers often create urgency. Take time to verify before acting!",
                    "If something sounds too good to be true, it probably is!"
                }},
                { "privacy", new List<string> {
                    "Review your privacy settings on social media regularly!",
                    "Be careful what personal info you share online."
                }},
                { "exit", new List<string> { "Stay safe out there! Goodbye!" } },
                { "what are you", new List<string> { "I'm CYBA, a cybersecurity awareness chatbot." } },
                { "what do you do", new List<string> { "I educate you about cyber threats and how to stay safe online." } },
                { "what can you do", new List<string> { "Ask me about passwords, phishing, VPN, scams, privacy, or any online safety topic." } }
            };
        }

        public string GetName() => name;

        public void SetName(string userName)
        {
            name = userName;
        }

        public string ProcessInput(string input)
        {
            string cleanInput = input.ToLower().Trim();

            if (string.IsNullOrEmpty(name) && !cleanInput.Contains("how are") && !cleanInput.Contains("what is"))
            {
                name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cleanInput);
                return $"Hello, {name}! I am CYBA. Ask me anything about online safety, passwords, phishing, or cybersecurity.";
            }

            if (cleanInput.Contains("tell me more") || cleanInput.Contains("explain more") || cleanInput.Contains("another tip"))
            {
                if (!string.IsNullOrEmpty(lastTopic) && responses.ContainsKey(lastTopic))
                {
                    return responses[lastTopic][random.Next(responses[lastTopic].Count)];
                }
                return "What would you like to know more about? Ask me about passwords, phishing, scams, or privacy!";
            }

            if (cleanInput.Contains("worried") || cleanInput.Contains("scared") || cleanInput.Contains("anxious"))
            {
                return "It's completely normal to feel concerned about cybersecurity. Let me help you stay safe with some practical tips!";
            }

            if (cleanInput.Contains("frustrated") || cleanInput.Contains("confused"))
            {
                return "I understand cybersecurity can be overwhelming. Let's take it step by step. What specific topic would you like to learn about?";
            }

            if (cleanInput.Contains("curious") || cleanInput.Contains("interested"))
            {
                return "That's great! Curiosity helps you learn. What cybersecurity topic interests you most?";
            }

            foreach (var keyword in responses.Keys)
            {
                if (cleanInput.Contains(keyword))
                {
                    lastTopic = keyword;
                    return responses[keyword][random.Next(responses[keyword].Count)];
                }
            }

            return "Sorry, I didn't understand that. Can you rephrase? Try asking about passwords, phishing, or cybersecurity tips.";
        }

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