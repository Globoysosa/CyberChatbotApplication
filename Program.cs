using System;
using System.Collections.Generic;
using System.Threading;
using System.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace CyberChatbotApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create instance of CYBA chatbot and start the application
            CYBA cyba = new CYBA();
            cyba.StartCYBA();
        }
    }

    class CYBA
    {
        private string name;                    // Stores the user's name

        // Keyword-based responses (keyword, response)
        private string[,] responses = new string[,]
        {
            { "nice to meet you" , "Well I'm glad to meet you too. I am CYBA, your Cybersecurity Awareness Assistant."},
            { "how are you" , "I'm doing great and ready to help! Ask me anything about cybersecurity."},
            { "tell me more" , "Cybersecurity is the practice of protecting computer systems, networks, and data from attacks."},
            { "safety" , "Don't share personal information like address, phone number or bank details."},
            { "vpn" , "VPN creates a secure tunnel for your internet traffic."},
            { "online protection" , "It is the security you have while using the internet."},
            { "cyber security" , "Use strong unique passwords and enable multi-factor authentication."},
            { "password" , "Make strong passwords with letters, numbers, and symbols. Minimum 8 characters. Never reuse passwords."},
            { "phishing", "Phishing is a trick to steal your information by pretending to be someone trustworthy." },
            { "exit", "Stay safe out there! Goodbye!" },
            { "what are you" , "I'm CYBA, a cybersecurity awareness chatbot." },
            { "What do you do" ,"I educate you about cyber threats and how to stay safe online."},
            { "what can you do", "Ask me about passwords, phishing, VPN, or any online safety topic." },
        };

        public void StartCYBA()
        {
            logo();                                 // Display the CYBA logo

            // Print welcome header
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n================================================================================================");
            Console.WriteLine("                  WELCOME TO CYBERSECURITY AWARENESS ASSISTANT");
            Console.WriteLine("================================================================================================");

            PlayGreeting();                         // Play voice greeting if file exists

            // Get user's name with validation
            Console.Write("\n Please enter your name : ");
            name = Console.ReadLine()?.Trim();

            while (string.IsNullOrEmpty(name))
            {
                Console.Write(" Name cannot be empty. Please enter your name: ");
                name = Console.ReadLine()?.Trim();
            }

            Console.WriteLine($"\n Hello, {name}! I am CYBA.");
            Console.WriteLine(" Ask me anything about online safety, passwords, phishing, or cybersecurity.");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n Type 'exit' to quit.\n");
            Console.ForegroundColor = ConsoleColor.White;

            RunCYBA();                              // Start the main chat loop
        }

        // Main chat loop - handles user input and responses
        private void RunCYBA()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($" {name}: ");
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

                string response = FindKeywords(input);

                Console.Write(" CYBA : ");
                Console.ForegroundColor = ConsoleColor.Green;
                delayEffect(response);              // Show response with typing effect
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        // Searches for matching keywords in user input and returns appropriate response
        private string FindKeywords(string input)
        {
            string cleanInput = input.ToLower().Trim();
            cleanInput = new string(cleanInput.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());

            for (int i = 0; i < responses.GetLength(0); i++)
            {
                if (cleanInput.Contains(responses[i, 0].ToLower()))
                {
                    return responses[i, 1];
                }
            }
            return "Sorry, I didn't understand that. Can you rephrase?";
        }

        // Typing effect - prints text character by character
        private void delayEffect(string text)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(30);                   // Adjust speed here if needed (lower = faster)
            }
            Console.WriteLine();
        }

        // Plays greeting sound if the wav file exists
        private void PlayGreeting()
        {
            string path = @"C:\Users\RC_Student_lab\source\repos\CyberChatbotApplication\voice_folder\greeting.wav";
            if (File.Exists(path))
            {
                SoundPlayer player = new SoundPlayer(path);
                player.PlaySync();
            }
        }

        // Displays the new CYBA logo (original logo removed and replaced)
        private void logo()
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
    }
}