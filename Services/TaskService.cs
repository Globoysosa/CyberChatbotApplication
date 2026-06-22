using CyberChatbotApplication.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CyberChatbotApplication.Services
{
    /// <summary>
    /// Handles all database operations for CyberTask objects.
    /// Connects to a MySQL database called 'cyba_db'.
    /// 
    /// Setup SQL:
    ///   CREATE DATABASE IF NOT EXISTS cyba_db;
    ///   USE cyba_db;
    ///   CREATE TABLE IF NOT EXISTS tasks (
    ///       id INT AUTO_INCREMENT PRIMARY KEY,
    ///       title VARCHAR(255) NOT NULL,
    ///       description TEXT,
    ///       is_completed TINYINT(1) DEFAULT 0,
    ///       reminder_date DATETIME NULL,
    ///       created_at DATETIME DEFAULT CURRENT_TIMESTAMP
    ///   );
    /// </summary>
    public class TaskService
    {
        // Connection string — update Server/Uid/Pwd as needed for the local MySQL instance
        private const string ConnectionString =
            "Server=localhost;Port=3306;Database=cyba_db;Uid=root;Pwd=Thokozani06!;SslMode=None;";

        // ─── Initialisation ────────────────────────────────────────────────────

        /// <summary>
        /// Creates the database and tasks table if they don't already exist.
        /// Should be called once at application startup.
        /// </summary>
        public static void InitialiseDatabase()
        {
            // Connect without specifying database first so we can create it
            string rootConn = "Server=localhost;Port=3306;Uid=root;Pwd=Thokozani06!;SslMode=None;";
            try
            {
                using (var conn = new MySqlConnection(rootConn))
                {
                    conn.Open();

                    // Create database
                    string createDb = "CREATE DATABASE IF NOT EXISTS cyba_db;";
                    using (var cmd = new MySqlCommand(createDb, conn))
                        cmd.ExecuteNonQuery();

                    // Switch to cyba_db and create table
                    string createTable = @"
                        USE cyba_db;
                        CREATE TABLE IF NOT EXISTS tasks (
                            id INT AUTO_INCREMENT PRIMARY KEY,
                            title VARCHAR(255) NOT NULL,
                            description TEXT,
                            is_completed TINYINT(1) DEFAULT 0,
                            reminder_date DATETIME NULL,
                            created_at DATETIME DEFAULT CURRENT_TIMESTAMP
                        );";
                    using (var cmd = new MySqlCommand(createTable, conn))
                        cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Log but don't crash — app still runs without DB
                System.Diagnostics.Debug.WriteLine($"[TaskService] DB init failed: {ex.Message}");
                throw; // Re-throw so caller can inform the user
            }
        }

        // ─── CRUD Operations ────────────────────────────────────────────────────

        /// <summary>Inserts a new task and returns its generated ID.</summary>
        public int AddTask(CyberTask task)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO tasks (title, description, is_completed, reminder_date, created_at)
                               VALUES (@title, @desc, 0, @reminder, @created);
                               SELECT LAST_INSERT_ID();";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@title", task.Title);
                    cmd.Parameters.AddWithValue("@desc", task.Description ?? "");
                    cmd.Parameters.AddWithValue("@reminder", task.ReminderDate.HasValue ? (object)task.ReminderDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@created", task.CreatedAt);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>Retrieves all tasks ordered by creation date descending.</summary>
        public List<CyberTask> GetAllTasks()
        {
            var list = new List<CyberTask>();
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "SELECT id, title, description, is_completed, reminder_date, created_at FROM tasks ORDER BY created_at DESC;";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var t = new CyberTask
                        {
                            Id = reader.GetInt32("id"),
                            Title = reader.GetString("title"),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString("description"),
                            IsCompleted = reader.GetBoolean("is_completed"),
                            ReminderDate = reader.IsDBNull(reader.GetOrdinal("reminder_date")) ? (DateTime?)null : reader.GetDateTime("reminder_date"),
                            CreatedAt = reader.GetDateTime("created_at")
                        };
                        list.Add(t);
                    }
                }
            }
            return list;
        }

        /// <summary>Marks a task as completed in the database.</summary>
        public void MarkCompleted(int taskId)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "UPDATE tasks SET is_completed = 1 WHERE id = @id;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", taskId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>Deletes a task permanently from the database.</summary>
        public void DeleteTask(int taskId)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "DELETE FROM tasks WHERE id = @id;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", taskId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>Updates an existing task's title, description and reminder date.</summary>
        public void UpdateTask(CyberTask task)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = @"UPDATE tasks SET title=@title, description=@desc, reminder_date=@reminder
                               WHERE id=@id;";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@title", task.Title);
                    cmd.Parameters.AddWithValue("@desc", task.Description ?? "");
                    cmd.Parameters.AddWithValue("@reminder", task.ReminderDate.HasValue ? (object)task.ReminderDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", task.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>Checks whether the database connection can be opened successfully.</summary>
        public static bool TestConnection()
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    return conn.State == ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
