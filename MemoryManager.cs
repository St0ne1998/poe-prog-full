using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace CybersecurityBot
{
    public class ChatMessageRecord
    {
        public string Sender { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class MemoryManager
    {
        private readonly string _storageFilePath;
        private List<ChatMessageRecord> _sessionHistory;

        public MemoryManager(string fileName = "chat_memory.json")
        {
            string appDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CybersecurityBot");
            Directory.CreateDirectory(appDataDir);
            _storageFilePath = Path.Combine(appDataDir, fileName);
            _sessionHistory = new List<ChatMessageRecord>();
        }

        public void AppendMemory(string sender, string message)
        {
            _sessionHistory.Add(new ChatMessageRecord
            {
                Sender = sender,
                Message = message,
                Timestamp = DateTime.Now
            });
        }

        public IReadOnlyList<ChatMessageRecord> GetHistory()
        {
            return _sessionHistory.AsReadOnly();
        }

        public async Task SaveMemoryAsync()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(_sessionHistory, options);
                await File.WriteAllTextAsync(_storageFilePath, jsonString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save memory: {ex.Message}");
            }
        }

        public async Task LoadMemoryAsync()
        {
            try
            {
                if (File.Exists(_storageFilePath))
                {
                    string jsonString = await File.ReadAllTextAsync(_storageFilePath);
                    _sessionHistory = JsonSerializer.Deserialize<List<ChatMessageRecord>>(jsonString) ?? new List<ChatMessageRecord>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load memory: {ex.Message}");
                _sessionHistory = new List<ChatMessageRecord>();
            }
        }
    }
}