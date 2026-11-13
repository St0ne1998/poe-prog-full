using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CybersecurityBot
{
    public class CybersecurityTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ReminderTimeframe { get; set; }
        public bool IsCompleted { get; set; }

        public override string ToString()
        {
            string status = IsCompleted ? "[Completed]" : "[Pending]";
            return $"{status} {Title} (Reminder: {ReminderTimeframe})";
        }
    }

    public class TaskDatabaseManager
    {
        private readonly string _connectionString = "Server=localhost;Database=cybersecurity_db;Uid=root;Pwd=;";
        private readonly List<CybersecurityTask> _memoryFallback = new List<CybersecurityTask>();
        private int _fallbackIdCounter = 1;

        public bool AddTask(string title, string description, string reminder)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO tasks (title, description, reminder, is_completed) VALUES (@title, @desc, @rem, 0)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@desc", description);
                        cmd.Parameters.AddWithValue("@rem", reminder);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch
            {
                _memoryFallback.Add(new CybersecurityTask
                {
                    Id = _fallbackIdCounter++,
                    Title = title,
                    Description = description,
                    ReminderTimeframe = reminder,
                    IsCompleted = false
                });
                return true;
            }
        }

        public List<CybersecurityTask> GetAllTasks()
        {
            var tasks = new List<CybersecurityTask>();
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT id, title, description, reminder, is_completed FROM tasks";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tasks.Add(new CybersecurityTask
                                {
                                    Id = reader.GetInt32("id"),
                                    Title = reader.GetString("title"),
                                    Description = reader.GetString("description"),
                                    ReminderTimeframe = reader.GetString("reminder"),
                                    IsCompleted = reader.GetBoolean("is_completed")
                                });
                            }
                        }
                    }
                }
            }
            catch
            {
                tasks.AddRange(_memoryFallback);
            }
            return tasks;
        }

        public bool MarkCompleted(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE tasks SET is_completed = 1 WHERE id = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch
            {
                var task = _memoryFallback.Find(t => t.Id == id);
                if (task != null) { task.IsCompleted = true; return true; }
                return false;
            }
        }

        public bool DeleteTask(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM tasks WHERE id = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch
            {
                var task = _memoryFallback.Find(t => t.Id == id);
                if (task != null) { _memoryFallback.Remove(task); return true; }
                return false;
            }
        }
    }
}}
