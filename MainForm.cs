using System;
using System.Drawing;
using System.Windows.Forms;

namespace CybersecurityBot
{
    public partial class MainForm : Form
    {
        private TextBox txtChatLog;
        private TextBox txtUserInput;
        private Button btnSend;
        private Button btnOpenQuiz;
        private Button btnViewTasks;
        private Button btnViewLogs;
        private Label lblStatus;

        private TaskDatabaseManager _dbManager;
        private QuizEngine _quizEngine;
        private ActivityLogManager _activityLog;
        private SentimentAnalyzer _sentimentAnalyzer;
        private AudioPlayer _audioPlayer;

        public MainForm()
        {
            InitializeComponentCustom();

            _dbManager = new TaskDatabaseManager();
            _quizEngine = new QuizEngine();
            _activityLog = new ActivityLogManager();
            _sentimentAnalyzer = new SentimentAnalyzer();
            _audioPlayer = new AudioPlayer();

            _ = _audioPlayer.PlayNotificationAsync("greeting.wav");
            _activityLog.LogAction("Chatbot session initialized.");
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Cybersecurity Assistant - Part 3 Dashboard";
            this.Size = new Size(800, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 243, 246);

            txtChatLog = new TextBox
            {
                Location = new Point(20, 20),
                Size = new Size(740, 360),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White
            };
            this.Controls.Add(txtChatLog);

            lblStatus = new Label
            {
                Location = new Point(20, 390),
                Size = new Size(300, 25),
                Text = "Status: Ready | Threat Priority: Low",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.DarkSlateGray
            };
            this.Controls.Add(lblStatus);

            btnOpenQuiz = new Button
            {
                Text = "Security Quiz",
                Location = new Point(330, 385),
                Size = new Size(130, 32),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnOpenQuiz.Click += (s, e) => LaunchQuiz();
            this.Controls.Add(btnOpenQuiz);

            btnViewTasks = new Button
            {
                Text = "Manage Tasks",
                Location = new Point(470, 385),
                Size = new Size(130, 32),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnViewTasks.Click += (s, e) => LaunchTaskManager();
            this.Controls.Add(btnViewTasks);

            btnViewLogs = new Button
            {
                Text = "Activity Log",
                Location = new Point(610, 385),
                Size = new Size(150, 32),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnViewLogs.Click += (s, e) => {
                string summary = _activityLog.GetRecentLogSummary(10);
                AppendChat("CyberBot", summary);
            };
            this.Controls.Add(btnViewLogs);

            txtUserInput = new TextBox
            {
                Location = new Point(20, 445),
                Size = new Size(605, 35),
                Font = new Font("Segoe UI", 11)
            };
            txtUserInput.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnSend_Click(s, e); };
            this.Controls.Add(txtUserInput);

            btnSend = new Button
            {
                Text = "Send",
                Location = new Point(635, 444),
                Size = new Size(125, 36),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnSend.Click += BtnSend_Click;
            this.Controls.Add(btnSend);
        }

        private void LaunchQuiz()
        {
            _activityLog.LogAction("Quiz started.");
            using (var qForm = new QuizForm(_quizEngine))
            {
                qForm.OnQuizCompleted = (score, total) => {
                    _activityLog.LogAction($"Quiz completed - {score}/{total} correct.");
                    AppendChat("CyberBot", $"Quiz finished! Score: {score}/{total}.");
                };
                qForm.ShowDialog(this);
            }
        }

        private void LaunchTaskManager()
        {
            _activityLog.LogAction("Opened Task Manager window.");
            using (var taskForm = new TaskManagerForm(_dbManager))
            {
                taskForm.ShowDialog(this);
            }
        }

        private async void BtnSend_Click(object sender, EventArgs e)
        {
            string input = txtUserInput.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            AppendChat("You", input);
            txtUserInput.Clear();

            var sentiment = _sentimentAnalyzer.Analyze(input);
            lblStatus.Text = $"Status: Active | Threat Priority: {sentiment.Priority}";

            if (sentiment.Priority >= ThreatPriority.High)
            {
                lblStatus.ForeColor = Color.Crimson;
                await _audioPlayer.PlayNotificationAsync("alert.wav");
            }
            else
            {
                lblStatus.ForeColor = Color.DarkSlateGray;
            }

            string botResponse = ProcessNLPInput(input);
            AppendChat("CyberBot", botResponse);
        }

        private string ProcessNLPInput(string input)
        {
            string lower = input.ToLower();

            // 1. Direct Choice Handling (User replying 1 or 2, or choosing quiz/tasks)
            if (lower == "1" || lower.Contains("quiz") || lower.Contains("game"))
            {
                this.BeginInvoke((MethodInvoker)delegate {
                    LaunchQuiz();
                });
                return "Launching the cybersecurity mini-game quiz window now!";
            }

            if (lower == "2" || lower.Contains("task") || lower.Contains("question"))
            {
                this.BeginInvoke((MethodInvoker)delegate {
                    LaunchTaskManager();
                });
                return "Opening your security tasks and management window!";
            }

            // 2. Activity Log Commands
            if (lower.Contains("activity log") || lower.Contains("what have you done") || lower.Contains("recent actions"))
            {
                _activityLog.LogAction("Requested activity log summary.");
                return _activityLog.GetRecentLogSummary(10);
            }

            // 3. Reminder / Task Creation Command
            if (lower.Contains("reminder") || lower.Contains("remind me to") || lower.Contains("add a task"))
            {
                string title = input;
                string reminderTime = "in 3 days";

                if (lower.Contains("remind me to"))
                {
                    title = input.Substring(lower.IndexOf("remind me to") + 12).Trim();
                }
                else if (lower.Contains("add a task to"))
                {
                    title = input.Substring(lower.IndexOf("add a task to") + 13).Trim();
                }

                _dbManager.AddTask(title, "Secured via NLP assistant prompt.", reminderTime);
                _activityLog.LogAction($"Task added: '{title}' (Reminder set for {reminderTime}).");
                return $"Reminder set for '{title}' ({reminderTime}). Task added successfully and stored in the database.";
            }

            // 4. Default Prompt: Force user to choose between the Quiz and Questions/Tasks
            _activityLog.LogAction("Presented user choice between quiz and tasks.");
            return "Please choose what you would like to do:\r\n1. Take the Cybersecurity Quiz (Type 'quiz' or '1')\r\n2. Manage Cybersecurity Tasks & Questions (Type 'tasks' or '2')";
        }

        private void AppendChat(string sender, string message)
        {
            txtChatLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {sender}: {message}\r\n\r\n");
        }
    }
}