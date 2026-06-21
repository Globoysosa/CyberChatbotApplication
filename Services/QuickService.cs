using CyberChatbotApplication.Models;
using System;
using System.Collections.Generic;

namespace CyberChatbotApplication.Services
{
    /// <summary>
    /// Provides the quiz questions for the Cybersecurity Mini-Game.
    /// Contains more than 10 questions covering phishing, passwords,
    /// safe browsing, and social engineering.
    /// </summary>
    public class QuizService
    {
        private List<QuizQuestion> _allQuestions;
        private Random _random;

        public QuizService()
        {
            _random = new Random();
            _allQuestions = BuildQuestionBank();
        }

        /// <summary>Returns a shuffled list of questions for a quiz session.</summary>
        public List<QuizQuestion> GetQuizQuestions(int count = 10)
        {
            // Shuffle a copy and return the requested number
            var shuffled = new List<QuizQuestion>(_allQuestions);
            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                var tmp = shuffled[i];
                shuffled[i] = shuffled[j];
                shuffled[j] = tmp;
            }
            return shuffled.GetRange(0, System.Math.Min(count, shuffled.Count));
        }

        /// <summary>
        /// Evaluates a final score and returns appropriate feedback text.
        /// </summary>
        public string GetScoreFeedback(int correct, int total)
        {
            double pct = (double)correct / total * 100;
            if (pct >= 90)
                return $"🏆 Outstanding! {correct}/{total} — You're a true Cybersecurity Pro! Keep defending the digital world!";
            if (pct >= 70)
                return $"🌟 Great job! {correct}/{total} — Solid cybersecurity knowledge. A little more study and you'll be unstoppable!";
            if (pct >= 50)
                return $"👍 Good effort! {correct}/{total} — You're on the right track. Review the tips and try again to improve!";
            return $"📚 Keep learning! {correct}/{total} — Cybersecurity is a journey. Study the tips and give it another go!";
        }

        // ─── Question Bank ──────────────────────────────────────────────────────

        private List<QuizQuestion> BuildQuestionBank()
        {
            return new List<QuizQuestion>
            {
                // 1 — Phishing
                new QuizQuestion(
                    "What should you do if you receive an email asking for your password?",
                    new List<string> { "A) Reply with your password", "B) Delete the email", "C) Report it as phishing", "D) Ignore it" },
                    2,
                    "✅ Reporting phishing emails helps protect others. Never share passwords via email — legitimate organisations never ask for them.",
                    "multiple"),

                // 2 — Password strength
                new QuizQuestion(
                    "Which of the following is the STRONGEST password?",
                    new List<string> { "A) password123", "B) MyDog2019", "C) P@ssw0rd!", "D) Tr0ub4dor&3" },
                    3,
                    "✅ A passphrase like 'Tr0ub4dor&3' combining random words with symbols is both strong and memorable.",
                    "multiple"),

                // 3 — Two-factor authentication (True/False)
                new QuizQuestion(
                    "True or False: Two-factor authentication (2FA) makes your account significantly more secure even if your password is stolen.",
                    new List<string> { "True", "False" },
                    0,
                    "✅ True! 2FA adds a second verification step (like an OTP), so stolen passwords alone can't unlock your account.",
                    "truefalse"),

                // 4 — Phishing URL recognition
                new QuizQuestion(
                    "You receive a link: http://paypa1.com/login. What is suspicious?",
                    new List<string> { "A) It uses HTTP instead of HTTPS", "B) The number '1' replaces the letter 'l'", "C) Both A and B", "D) Nothing — it looks fine" },
                    2,
                    "✅ Both are red flags! Typosquatting (swapping letters for numbers) and missing HTTPS are classic phishing tactics.",
                    "multiple"),

                // 5 — Social engineering
                new QuizQuestion(
                    "A caller claims to be from IT Support and asks for your login credentials to fix an issue. What do you do?",
                    new List<string> { "A) Provide the credentials — they said they're from IT", "B) Hang up and call IT directly using the official number", "C) Email them the credentials instead", "D) Give only your username" },
                    1,
                    "✅ Legitimate IT staff never need your password. Always verify by calling the official number independently.",
                    "multiple"),

                // 6 — VPN (True/False)
                new QuizQuestion(
                    "True or False: Using public Wi-Fi without a VPN is completely safe as long as you log out after each session.",
                    new List<string> { "True", "False" },
                    1,
                    "✅ False! Public Wi-Fi can expose your data to eavesdroppers. A VPN encrypts your traffic, making interception much harder.",
                    "truefalse"),

                // 7 — Malware
                new QuizQuestion(
                    "What is ransomware?",
                    new List<string> { "A) Software that speeds up your computer", "B) Malware that encrypts your files and demands payment", "C) A type of antivirus", "D) A browser plugin" },
                    1,
                    "✅ Ransomware locks or encrypts your files and demands a ransom. Regular backups are your best defence.",
                    "multiple"),

                // 8 — Software updates (True/False)
                new QuizQuestion(
                    "True or False: Delaying software updates is fine because updates only add new features.",
                    new List<string> { "True", "False" },
                    1,
                    "✅ False! Updates often patch critical security vulnerabilities. Delaying them leaves you exposed to known exploits.",
                    "truefalse"),

                // 9 — Privacy settings
                new QuizQuestion(
                    "Which action best protects your privacy on social media?",
                    new List<string> { "A) Using the same password as your email", "B) Sharing your location in every post", "C) Setting your profile to private and limiting who can see your posts", "D) Accepting all friend requests to grow your network" },
                    2,
                    "✅ A private profile limits who can see your personal information, reducing exposure to identity theft and social engineering.",
                    "multiple"),

                // 10 — Safe browsing
                new QuizQuestion(
                    "What does the padlock icon 🔒 in your browser's address bar indicate?",
                    new List<string> { "A) The website is 100% trustworthy", "B) Your connection to the site is encrypted (HTTPS)", "C) The site is owned by a verified company", "D) There are no ads on the page" },
                    1,
                    "✅ The padlock means the connection is encrypted (HTTPS), but it does NOT guarantee the site is legitimate. Always verify the URL.",
                    "multiple"),

                // 11 — Password reuse (True/False)
                new QuizQuestion(
                    "True or False: It is acceptable to reuse your strong password across multiple accounts to make it easier to remember.",
                    new List<string> { "True", "False" },
                    1,
                    "✅ False! If one site is breached, attackers try the same credentials elsewhere (credential stuffing). Use unique passwords and a password manager.",
                    "truefalse"),

                // 12 — Spear phishing
                new QuizQuestion(
                    "What distinguishes 'spear phishing' from regular phishing?",
                    new List<string> { "A) It targets fish in the ocean", "B) It is a generic mass email attack", "C) It is a targeted attack using personalised information about the victim", "D) It only works on mobile devices" },
                    2,
                    "✅ Spear phishing uses personal details (name, employer, etc.) to craft convincing targeted attacks. Always verify unexpected requests.",
                    "multiple"),
            };
        }
    }
}

