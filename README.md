# CYBA — Cybersecurity Awareness Bot v3.0
### PROG6221 Portfolio of Evidence — Part 3

A WPF-based cybersecurity awareness chatbot built in C# (.NET Framework 4.7.2).

---

## Features Overview

| Feature | Part |
|---|---|
| Voice greeting (WAV) | Part 1 |
| ASCII art logo | Part 1 |
| Personalised greeting & name capture | Part 1 |
| Keyword responses (password, phishing, scam, privacy...) | Part 2 |
| Random responses & conversation flow | Part 2 |
| Sentiment detection (worried, frustrated, curious) | Part 2 |
| Memory / interest recall | Part 2 |
| **Task Assistant with MySQL DB** | **Part 3** |
| **Cybersecurity Mini-Game (12-question quiz)** | **Part 3** |
| **NLP simulation (keyword intent detection)** | **Part 3** |
| **Activity Log (last 10 / full history)** | **Part 3** |

---

## Prerequisites

- Visual Studio 2019 or newer (with .NET Desktop Development workload)
- .NET Framework 4.7.2
- MySQL Server 8.x (for Task Assistant)
- NuGet: `MySql.Data 8.0.33`

---

## MySQL Database Setup

The app auto-creates the database and table on first run. You only need:

1. Install MySQL Server — [https://dev.mysql.com/downloads/mysql/](https://dev.mysql.com/downloads/mysql/)
2. Start the MySQL service
3. Ensure the `root` user can connect on `localhost:3306` (default install)
4. Run the app — the database `cyba_db` and table `tasks` are created automatically

**Manual setup (if needed):**
```sql
CREATE DATABASE IF NOT EXISTS cyba_db;
USE cyba_db;
CREATE TABLE IF NOT EXISTS tasks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    is_completed TINYINT(1) DEFAULT 0,
    reminder_date DATETIME NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);
```

To change connection credentials, edit `Services/TaskService.cs`:
```csharp
private const string ConnectionString =
    "Server=localhost;Port=3306;Database=cyba_db;Uid=root;Pwd=YOUR_PASSWORD;SslMode=None;";
```

---

## Running the Application

1. Open `CyberChatbotApplication.sln` in Visual Studio
2. Restore NuGet packages (right-click solution → Restore NuGet Packages)
3. Build and run (`F5`)

---

## How to Use

### Chat Commands (Natural Language)
| What to type | What happens |
|---|---|
| Your name | Sets your name and personalises the session |
| `help` | Shows all features |
| `tell me about passwords` | Cybersecurity tip on passwords |
| `I'm worried about phishing` | Sentiment detection + tip |
| `add task to enable 2FA` | Creates a task in the DB |
| `show my tasks` | Lists all tasks from DB |
| `mark task complete` | Marks a task as done in DB |
| `delete task` | Removes a task from DB |
| `start quiz` | Opens the quiz mini-game |
| `show activity log` | Displays last 10 actions |
| `show full log` | Displays all logged actions |

### Toolbar Buttons
- **🔊 Greeting** — Replays the voice greeting
- **📋 Tasks** — Opens the Task Assistant window
- **🎮 Quiz** — Opens the cybersecurity quiz
- **📊 Activity Log** — Opens the activity log viewer
- **💻 Console** — Switches to console mode

---

## Project Structure

```
CyberChatbotApplication/
├── CYBA.cs                        # Core chatbot brain (Parts 1+2+3)
├── MainWindow.xaml/.cs            # Main GUI (Parts 1+2+3)
├── Models/
│   ├── CyberTask.cs               # Task data model
│   └── QuizQuestion.cs            # Quiz question model
├── Services/
│   ├── TaskService.cs             # MySQL CRUD operations
│   ├── QuizService.cs             # 12 quiz questions + scoring
│   ├── ActivityLogService.cs      # In-memory activity log
│   └── NLPService.cs              # Keyword intent detection
├── Windows/
│   ├── TaskWindow.xaml/.cs        # Task Assistant GUI
│   ├── QuizWindow.xaml/.cs        # Quiz mini-game GUI
│   └── ActivityLogWindow.xaml/.cs # Activity log viewer GUI
├── voice_folder/
│   └── greeting.wav.wav           # Voice greeting audio
└── ascii_folder/
    └── locked.jpeg                # Reference image
```

---

## GitHub Actions CI

CI workflow runs on every push and checks that the project builds successfully.

✅ See `.github/workflows/dotnet.yml` for the workflow configuration.

---

## GitHub Releases / Tags

| Tag | Description |
|---|---|
| `v1.0` | Part 1 — Console chatbot with voice & ASCII art |
| `v2.0` | Part 2 — WPF GUI, keyword recognition, sentiment, memory |
| `v3.0` | Part 3 — Task Assistant, Quiz, NLP, Activity Log |

---

## Student Information

- **Module:** Programming 2A (PROG6221/w)
- **Assessment:** Portfolio of Evidence — Part 3
- **Institution:** The Independent Institute of Education (IIE)
