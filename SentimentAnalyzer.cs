using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityBot
{
    public enum ThreatPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class SentimentResult
    {
        public ThreatPriority Priority { get; set; }
        public double Score { get; set; }
        public List<string> DetectedKeywords { get; set; } = new List<string>();
    }

    public class SentimentAnalyzer
    {
        private readonly Dictionary<string, ThreatPriority> _keywordWeights = new Dictionary<string, ThreatPriority>(StringComparer.OrdinalIgnoreCase)
        {
            { "breach", ThreatPriority.Critical },
            { "ransomware", ThreatPriority.Critical },
            { "hack", ThreatPriority.High },
            { "malware", ThreatPriority.High },
            { "phishing", ThreatPriority.Medium },
            { "leak", ThreatPriority.High },
            { "suspicious", ThreatPriority.Medium },
            { "unauthorized", ThreatPriority.High },
            { "password", ThreatPriority.Low },
            { "help", ThreatPriority.Low }
        };

        public SentimentResult Analyze(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new SentimentResult { Priority = ThreatPriority.Low, Score = 0.0 };
            }

            var words = input.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            var foundKeywords = new List<string>();
            int accumulatedScore = 0;

            foreach (var word in words)
            {
                if (_keywordWeights.TryGetValue(word, out var priority))
                {
                    foundKeywords.Add(word);
                    accumulatedScore += (int)priority + 1;
                }
            }

            ThreatPriority determinedPriority = ThreatPriority.Low;
            if (accumulatedScore >= 6) determinedPriority = ThreatPriority.Critical;
            else if (accumulatedScore >= 4) determinedPriority = ThreatPriority.High;
            else if (accumulatedScore >= 2) determinedPriority = ThreatPriority.Medium;

            return new SentimentResult
            {
                Priority = determinedPriority,
                Score = accumulatedScore,
                DetectedKeywords = foundKeywords
            };
        }
    }
}