using System;
using System.Drawing;
using System.Windows.Forms;

namespace CybersecurityBot
{
    public partial class TaskManagerForm : Form
    {
        private TaskDatabaseManager _dbManager;
        private ListBox lstTasks;
        private Button btnComplete;
        private Button btnDelete;
        private Button btnRefresh;

        public TaskManagerForm(TaskDatabaseManager dbManager)
        {
            _dbManager = dbManager;
            InitializeComponentCustom();
            LoadTasks();
        }

        private void InitializeComponentCustom()
        {
            this.Text = "Cybersecurity Task Manager";
            this.Size = new Size(550, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);

            Label lblTitle = new Label
            {
                Text = "Your Cybersecurity Tasks & Reminders",
                Location = new Point(20, 15),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            this.Controls.Add(lblTitle);

            lstTasks = new ListBox
            {
                Location = new Point(20, 50),
                Size = new Size(500, 230),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(lstTasks);

            btnComplete = new Button
            {
                Text = "Mark Completed",
                Location = new Point(20, 300),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnComplete.Click += (s, e) => {
                if (lstTasks.SelectedItem is CybersecurityTask task)
                {
                    _dbManager.MarkCompleted(task.Id);
                    LoadTasks();
                }
                else { MessageBox.Show("Please select a task from the list."); }
            };
            this.Controls.Add(btnComplete);

            btnDelete = new Button
            {
                Text = "Delete Task",
                Location = new Point(190, 300),
                Size = new Size(130, 35),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnDelete.Click += (s, e) => {
                if (lstTasks.SelectedItem is CybersecurityTask task)
                {
                    _dbManager.DeleteTask(task.Id);
                    LoadTasks();
                }
                else { MessageBox.Show("Please select a task from the list."); }
            };
            this.Controls.Add(btnDelete);

            btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(340, 300),
                Size = new Size(180, 35),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnRefresh.Click += (s, e) => LoadTasks();
            this.Controls.Add(btnRefresh);
        }

        private void LoadTasks()
        {
            lstTasks.Items.Clear();
            var tasks = _dbManager.GetAllTasks();
            foreach (var t in tasks)
            {
                lstTasks.Items.Add(t);
            }
        }
    }
}