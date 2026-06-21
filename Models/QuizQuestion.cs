using System.Collections.Generic;

namespace CyberChatbotApplication.Models
{
    /// <summary>
    /// Represents a single quiz question with multiple choice or true/false format.
    /// </summary>
    public class QuizQuestion
    {
        public string Question { get; set; }

        /// <summary>For multiple choice: list of options. For true/false: ["True","False"].</summary>
        public List<string> Options { get; set; }

        /// <summary>Zero-based index of the correct option in Options list.</summary>
        public int CorrectIndex { get; set; }

        /// <summary>Explanation shown after the user answers.</summary>
        public string Explanation { get; set; }

        /// <summary>QuestionType: "multiple" or "truefalse"</summary>
        public string QuestionType { get; set; }

        public QuizQuestion(string question, List<string> options, int correctIndex, string explanation, string questionType = "multiple")
        {
            Question = question;
            Options = options;
            CorrectIndex = correctIndex;
            Explanation = explanation;
            QuestionType = questionType;
        }
    }
}
