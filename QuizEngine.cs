using System.Collections.Generic;

namespace CybersecurityBot
{
    public class QuizQuestion
    {
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public int CorrectIndex { get; set; }
        public string Explanation { get; set; }
    }

    public class QuizEngine
    {
        private readonly List<QuizQuestion> _questions;
        private int _currentIndex = 0;
        private int _score = 0;

        public QuizEngine()
        {
            _questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    QuestionText = "1. What is the most secure practice for creating a password?",
                    Options = new List<string> { "Using your pet's name", "Using a short word", "Using a long passphrase with mixed characters", "Reusing passwords" },
                    CorrectIndex = 2,
                    Explanation = "Long passphrases significantly increase resistance to brute-force attacks."
                },
                new QuizQuestion
                {
                    QuestionText = "2. Which of the following is a classic indicator of a phishing email?",
                    Options = new List<string> { "Personalized ID greeting", "Urgent demands and generic sender domains", "Clear links to official corporate sites", "Sent during work hours" },
                    CorrectIndex = 1,
                    Explanation = "Phishing emails create false urgency and deceptive domains to trap users."
                },
                new QuizQuestion
                {
                    QuestionText = "3. True or False: Multi-Factor Authentication (MFA) requires two or more verification factors.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 0,
                    Explanation = "MFA adds a crucial secondary layer of security beyond just a password."
                },
                new QuizQuestion
                {
                    QuestionText = "4. What should you do if you receive an unexpected email asking for your login credentials?",
                    Options = new List<string> { "Reply immediately", "Delete or report it as phishing", "Click the link to check", "Forward it to friends" },
                    CorrectIndex = 1,
                    Explanation = "Reporting or deleting prevents unauthorized account takeovers."
                },
                new QuizQuestion
                {
                    QuestionText = "5. Which protocol should you look for in a URL when entering sensitive financial information?",
                    Options = new List<string> { "http://", "ftp://", "https://", "telnet://" },
                    CorrectIndex = 2,
                    Explanation = "HTTPS ensures that data transmitted between your browser and the server is encrypted."
                },
                new QuizQuestion
                {
                    QuestionText = "6. True or False: Public Wi-Fi networks are generally as secure as a private home network.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "Public networks can expose unencrypted traffic to packet sniffing; a VPN should be used."
                },
                new QuizQuestion
                {
                    QuestionText = "7. What is ransomware?",
                    Options = new List<string> { "Software that speeds up PC backups", "Malware that encrypts your files and demands payment", "An anti-virus scanning utility", "A secure firewall tool" },
                    CorrectIndex = 1,
                    Explanation = "Ransomware locks system data until a ransom is paid."
                },
                new QuizQuestion
                {
                    QuestionText = "8. True or False: Regularly updating software helps patch security vulnerabilities.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 0,
                    Explanation = "Patch management closes known security gaps exploited by malware."
                },
                new QuizQuestion
                {
                    QuestionText = "9. What is social engineering in cybersecurity?",
                    Options = new List<string> { "Networking on social media sites", "Manipulating individuals into divulging confidential info", "Automated code compiling", "Server load balancing" },
                    CorrectIndex = 1,
                    Explanation = "Social engineering exploits human psychology rather than technical software bugs."
                },
                new QuizQuestion
                {
                    QuestionText = "10. Which backup strategy is widely recommended to protect against data loss?",
                    Options = new List<string> { "Single USB drive kept next to the PC", "The 3-2-1 backup rule", "Saving files to your desktop", "Never backing up data" },
                    CorrectIndex = 1,
                    Explanation = "The 3-2-1 rule maintains 3 copies, across 2 different media types, with 1 offsite copy."
                }
            };
        }

        public bool HasMoreQuestions => _currentIndex < _questions.Count;
        public QuizQuestion GetCurrentQuestion() => HasMoreQuestions ? _questions[_currentIndex] : null;

        public bool AnswerQuestion(int selectedIndex, out string explanation)
        {
            var q = GetCurrentQuestion();
            explanation = q.Explanation;
            bool correct = (q.CorrectIndex == selectedIndex);
            if (correct) _score++;
            _currentIndex++;
            return correct;
        }

        public int GetScore() => _score;
        public int GetTotalQuestions() => _questions.Count;
        public void Reset() { _currentIndex = 0; _score = 0; }

        public string GetPerformanceFeedback()
        {
            double percentage = ((double)_score / _questions.Count) * 100;
            if (percentage >= 80)
                return $"Great job! You scored {_score}/{_questions.Count}. You're a cybersecurity pro!";
            else if (percentage >= 50)
                return $"Good effort! You scored {_score}/{_questions.Count}. Keep learning to stay safe online!";
            else
                return $"You scored {_score}/{_questions.Count}. Keep learning and reviewing cybersecurity fundamentals to stay safe online!";
        }
    }
}