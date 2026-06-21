using System;

namespace CyberChatbotApplication.Models
{
    /// <summary>
    /// Represents a cybersecurity task that the user wants to complete.
    /// Stored in MySQL database via TaskService.
    /// </summary>
    public class CyberTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? ReminderDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public CyberTask()
        {
            CreatedAt = DateTime.Now;
            IsCompleted = false;
        }

        public CyberTask(string title, string description, DateTime? reminderDate = null)
        {
            Title = title;
            Description = description;
            ReminderDate = reminderDate;
            IsCompleted = false;
            CreatedAt = DateTime.Now;
        }

        public override string ToString()
        {
            string status = IsCompleted ? "✅" : "⏳";
            string reminder = ReminderDate.HasValue ? $" | Reminder: {ReminderDate.Value:dd MMM yyyy}" : "";
            return $"{status} {Title}{reminder}";
        }
    }
}
