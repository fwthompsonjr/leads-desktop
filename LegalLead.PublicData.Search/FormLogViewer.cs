using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public class FormLogViewer : Form
    {
        private TableLayoutPanel tableLayoutPanel;
        private ComboBox comboBox;
        private DataGridView dataGridView;
        private DataGridView dataGridSummary;
        private TextBox textBox;
        private Button button;

        public FormLogViewer()
        {
            InitializeComponents();
            LoadLogFiles();
        }

        private void InitializeComponents()
        {
            // Initialize controls
            tableLayoutPanel = new TableLayoutPanel {
                Name = "logTableLayoutPanel",
                ColumnCount = 3,
                RowCount = 3,
                Dock = DockStyle.Fill,
                Padding = new Padding(2)
            };
            comboBox = new ComboBox {
                Name = "logComboBox",
                Dock = DockStyle.Fill
            };
            dataGridView = new DataGridView { 
                Name = "logGridView",
                Dock = DockStyle.Fill,
                ReadOnly = true 
            };
            dataGridSummary = new DataGridView
            {
                Name = "logSummaryView",
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Padding = new Padding(2)
            };
            textBox = new TextBox { 
                Name = "logTextBox",
                Dock = DockStyle.Fill,
                AutoSize = true,
                ReadOnly = true,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Padding = new Padding(2)
            };
            button = new Button { 
                Name = "logReturnButton", 
                AutoSize = true, 
                Text = "Return",
                Padding = new Padding(2),
                Dock = DockStyle.Fill
            };
            // Set up TableLayoutPanel
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));

            // Add controls to TableLayoutPanel
            tableLayoutPanel.Controls.Add(button, 0, 0);
            tableLayoutPanel.Controls.Add(comboBox, 1, 0);
            tableLayoutPanel.SetColumnSpan(comboBox, 2);
            tableLayoutPanel.Controls.Add(dataGridView, 0, 1);
            tableLayoutPanel.SetColumnSpan(dataGridView, 2);
            tableLayoutPanel.Controls.Add(textBox, 2, 1);
            tableLayoutPanel.SetRowSpan(textBox, 2);
            tableLayoutPanel.Controls.Add(dataGridSummary, 0, 2);
            tableLayoutPanel.SetColumnSpan(dataGridSummary, 2);

            // Add event handlers
            comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            dataGridView.RowEnter += DataGridView_SelectionChanged;
            button.Click += Button_Click;
            // Add TableLayoutPanel to Form
            Controls.Add(tableLayoutPanel);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (Program.mainForm == null) return;
            Program.mainForm.menuLogView.PerformClick();
        }

        private void LoadLogFiles()
        {
            var logFiles = Directory.GetFiles(AppBasePath, "*log*.txt")
                                    .Select(Path.GetFileName)
                                    .ToList();
            comboBox.DataSource = logFiles;
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedFile = comboBox.SelectedItem.ToString();
            var filePath = Path.Combine(AppBasePath, selectedFile);
            var fileContent = File.ReadAllText(filePath);

            var logEntries = FileSplitRegex.Split(fileContent)
                                  .Where(part => !string.IsNullOrWhiteSpace(part))
                                  .Select(part => {
                                      var dateEndIndex = part.IndexOf(": ");
                                      return new LogView
                                      {
                                          Date = part[..dateEndIndex],
                                          LogEntry = part[(dateEndIndex + 2)..]
                                      };
                                  })
                                  .ToList();

            logEntries.Sort((a, b) => {
                var aa = a.Date.CompareTo(b.Date);
                if (aa != 0) return aa;
                return a.LogEntry.CompareTo(b.LogEntry);
            });
            dataGridView.Columns.Clear();
            dataGridView.DataSource = logEntries;
            // Set the column properties
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView.Columns[0].Width = 150; // Set a fixed width for the first column
            dataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            var summary = CollectFetchRequestItems(logEntries);
            dataGridSummary.Columns.Clear();
            dataGridSummary.DataSource = summary;
            // Set the column properties
            dataGridSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            for (int i = 0; i < 3; i++)
            {
                dataGridSummary.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dataGridSummary.Columns[i].Width = i == 0 ? 50 : 150; // Set a fixed width for the first (3) column
                if (i == 0) continue;
                dataGridSummary.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            dataGridSummary.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridSummary.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void DataGridView_SelectionChanged(object sender, DataGridViewCellEventArgs e)
        {
            var isTextSet = false;
            try
            {
                var indx = e.RowIndex;
                if (dataGridView.DataSource is not List<LogView> list) return;
                if (indx < 0 || indx > list.Count - 1) return;

                textBox.Text = list[indx].LogEntry;
                isTextSet = true;
            }
            finally
            {
                if (!isTextSet) textBox.Text = string.Empty;
            }
        }

        private static List<FetchRequestItem> CollectFetchRequestItems(List<LogView> logViews)
        {
            const string startingFetchToken = "Starting fetch request: ";
            const string endingFetchToken = "Ending fetch request:";
            var fetchRequestItems = new List<FetchRequestItem>();
            FetchRequestItem currentItem = null;

            foreach (var logView in logViews)
            {
                if (logView.LogEntry.Contains(startingFetchToken))
                {
                    if (currentItem != null)
                    {
                        // If a new start request is found before ending the previous one, mark it as aborted
                        currentItem.EndTime = null;
                        fetchRequestItems.Add(currentItem);
                    }

                    currentItem = new FetchRequestItem
                    {
                        Id = logViews.IndexOf(logView),
                        StartTime = DateTime.ParseExact(logView.Date, "s", CultureInfo.CurrentCulture)
                    };
                }
                else if (logView.LogEntry.Contains(endingFetchToken) && currentItem != null)
                {
                    currentItem.EndTime = DateTime.ParseExact(logView.Date, "s", CultureInfo.CurrentCulture);
                    Assert.IsFalse(string.IsNullOrEmpty(currentItem.ElapsedTime));
                    fetchRequestItems.Add(currentItem);
                    currentItem = null;
                }
            }

            // If the last fetch request was not ended, mark it as aborted
            if (currentItem != null)
            {
                currentItem.EndTime = null;
                fetchRequestItems.Add(currentItem);
            }

            return fetchRequestItems;
        }

        private class LogView
        {
            public string Date { get; set; }
            public string LogEntry { get; set; }
        }
        private class FetchRequestItem
        {
            public int Id { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public string ElapsedTime
            {
                get
                {
                    if (!EndTime.HasValue) return string.Empty;
                    TimeSpan timeSpan = (EndTime.Value - StartTime);
                    return timeSpan.TotalMinutes.ToString("F2", CultureInfo.CurrentCulture.NumberFormat);
                }
            }
        }

        private readonly static string AppBasePath = AppDomain.CurrentDomain.BaseDirectory;
        private readonly static Regex FileSplitRegex = new(@"(?=\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2})");
    }
}
