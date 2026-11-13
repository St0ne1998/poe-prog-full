using System;
using System.Drawing;
using System.Windows.Forms;

namespace CybersecurityBot
{
    public partial class QuizForm : Form
    {
        private QuizEngine _quizEngine;
        private Label lblQuestion;
        private RadioButton[] rbOptions;
        private Button btnSubmit;
        private Label lblFeedback;
        public Action<int, int> OnQuizCompleted;

        public QuizForm(QuizEngine engine)
        {
            _quizEngine = engine;
            _quizEngine.Reset();
            InitializeComponentCustom();
            LoadNextQuestion();
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Cybersecurity Mini-Game Quiz";
            this.Size = new Size(620, 460);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);

            lblQuestion = new Label
            {
                Location = new Point(20, 20),
                Size = new Size(560, 70),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30)
            };
            this.Controls.Add(lblQuestion);

            rbOptions = new RadioButton[4];
            int startY = 100;
            for (int i = 0; i < 4; i++)
            {
                rbOptions[i] = new RadioButton
                {
                    Location = new Point(30, startY + (i * 42)),
                    Size = new Size(540, 35),
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.FromArgb(50, 50, 50)
                };
                this.Controls.Add(rbOptions[i]);
            }

            lblFeedback = new Label
            {
                Location = new Point(20, 290),
                Size = new Size(560, 50),
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.DarkGray
            };
            this.Controls.Add(lblFeedback);

            btnSubmit = new Button
            {
                Text = "Submit Answer",
                Location = new Point(440, 360),
                Size = new Size(140, 38),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Click += BtnSubmit_Click;
            this.Controls.Add(btnSubmit);
        }

        private void LoadNextQuestion()
        {
            if (_quizEngine.HasMoreQuestions)
            {
                var q = _quizEngine.GetCurrentQuestion();
                lblQuestion.Text = q.QuestionText;
                for (int i = 0; i < 4; i++)
                {
                    rbOptions[i].Checked = false;
                    rbOptions[i].Visible = i < q.Options.Count;
                    if (i < q.Options.Count)
                    {
                        rbOptions[i].Text = q.Options[i];
                    }
                }
                lblFeedback.Text = "";
            }
            else
            {
                string summary = _quizEngine.GetPerformanceFeedback();
                MessageBox.Show(summary, "Quiz Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OnQuizCompleted?.Invoke(_quizEngine.GetScore(), _quizEngine.GetTotalQuestions());
                this.Close();
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            int selected = -1;
            for (int i = 0; i < 4; i++)
            {
                if (rbOptions[i].Checked) { selected = i; break; }
            }

            if (selected == -1)
            {
                MessageBox.Show("Please select an option before submitting.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool correct = _quizEngine.AnswerQuestion(selected, out string explanation);

            if (correct)
            {
                lblFeedback.ForeColor = Color.SeaGreen;
                lblFeedback.Text = "✔ Correct! " + explanation;
            }
            else
            {
                lblFeedback.ForeColor = Color.Crimson;
                lblFeedback.Text = "✖ Incorrect. " + explanation;
            }

            btnSubmit.Text = "Next Question";
            btnSubmit.Click -= BtnSubmit_Click;
            btnSubmit.Click += ProceedToNext;
        }

        private void ProceedToNext(object sender, EventArgs e)
        {
            btnSubmit.Text = "Submit Answer";
            btnSubmit.Click -= ProceedToNext;
            btnSubmit.Click += BtnSubmit_Click;
            LoadNextQuestion();
        }
    }
}