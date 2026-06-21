using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberChatbotApplication.Services
{
    /// <summary>
    /// Maintains an in-memory activity log for all chatbot actions.
    /// Stores up to MaxLogSize entries; older entries are removed automatically.
    /// Users can request the log via "show activity log" or "what have you done for me".
    /// </summary>
    public class ActivityLogService
    {
        private const int MaxLogSize = 50;
        private List<ActivityEntry> _log;

        public ActivityLogService()
        {
            _log = new List<ActivityEntry>();
        }

        // ─── Logging Actions ────────────────────────────────────────────────────

        /// <summary>Adds a new entry to the activity log.</summary>
        public void Log(string action, string details = "")
        {
            _log.Add(new ActivityEntry(action, details));
            // Trim to max size (remove oldest)
            if (_log.Count > MaxLogSize)
                _log.RemoveAt(0);
        }

        // ─── Retrieval ──────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the most recent entries as formatted strings.
        /// </summary>
        /// <param name="count">How many recent entries to return (default 10).</param>
        public List<string> GetRecentLog(int count = 10)
        {
            return _log
                .OrderByDescending(e => e.Timestamp)
                .Take(count)
                .Select((e, i) => $"{i + 1}. [{e.Timestamp:HH:mm dd MMM}] {e.Action}" +
                                  (string.IsNullOrEmpty(e.Details) ? "" : $" — {e.Details}"))
                .ToList();
        }

        /// <summary>Returns all stored log entries.</summary>
        public List<ActivityEntry> GetAllEntries() => new List<ActivityEntry>(_log.OrderByDescending(e => e.Timestamp));

        /// <summary>Returns total number of logged actions.</summary>
        public int TotalCount => _log.Count;

        // ─── Formatted Summary ──────────────────────────────────────────────────

        /// <summary>Builds the response string shown to the user when they request the activity log.</summary>
        public string BuildLogSummary(int count = 10)
        {
            var recent = GetRecentLog(count);
            if (recent.Count == 0)
                return "📋 No activity recorded yet. Start chatting, adding tasks, or playing the quiz!";

            string summary = $"📋 Here's a summary of my recent actions (last {recent.Count}):\n\n";
            summary += string.Join("\n", recent);

            if (_log.Count > count)
                summary += $"\n\n💡 Showing {count} of {_log.Count} total actions. Type 'show full log' to see all.";

            return summary;
        }

        /// <summary>Builds the full log summary without the entry limit.</summary>
        public string BuildFullLogSummary()
        {
            var all = GetRecentLog(MaxLogSize);
            if (all.Count == 0)
                return "📋 No activity recorded yet.";

            return $"📋 Full Activity Log ({all.Count} entries):\n\n" + string.Join("\n", all);
        }
    }

    // ─── ActivityEntry ──────────────────────────────────────────────────────────

    /// <summary>A single timestamped log entry.</summary>
    public class ActivityEntry
    {
        public DateTime Timestamp { get; }
        public string Action { get; }
        public string Details { get; }

        public ActivityEntry(string action, string details = "")
        {
            Timestamp = DateTime.Now;
            Action = action;
            Details = details;
        }
    }
}
