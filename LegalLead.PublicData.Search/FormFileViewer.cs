using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public class FormFileViewer : Form
    {
        private TableLayoutPanel tableLayoutPanel;
        private SplitContainer splitContainer;
        private DataGridView dataGridFiles;
        private DataGridView dataGridSummary;
        private Button button;
        private List<FileInfo> fileCollection;

        public FormFileViewer()
        {
            InitializeComponents();
            dataGridFiles.DataSource = new List<FileView>();
            dataGridFiles.Refresh();
            dataGridFiles.Enabled = false;
            dataGridSummary.DataSource = new List<QueryDbResult>();
            dataGridSummary.Refresh();
            splitContainer.Panel2Collapsed = false;
            _ = LoadFileNamesAsync().ContinueWith(_ =>
            {
                // Ensure this runs on the UI thread
                Invoke(() => {
                    dataGridFiles.Enabled = true;
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void InitializeComponents()
        {
            // Initialize controls
            tableLayoutPanel = new TableLayoutPanel
            {
                Name = "tableLayoutPanel",
                ColumnCount = 1,
                RowCount = 2,
                Dock = DockStyle.Fill,
                Padding = new Padding(2)
            };
            splitContainer = new SplitContainer
            {
                Name = "splitContainer",
                Dock = DockStyle.Fill
            };
            dataGridFiles = new DataGridView
            {
                Name = "dataGridFiles",
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Padding = new Padding(2),
            };
            dataGridSummary = new DataGridView
            {
                Name = "dataGridSummary",
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Padding = new Padding(2)
            };

            button = new Button
            {
                Name = "logReturnButton",
                AutoSize = true,
                Text = "Return",
                Padding = new Padding(2),
                Dock = DockStyle.Fill
            };
            button.Click += Button_Click;
            dataGridFiles.RowHeaderMouseClick += DataGridFiles_RowHeaderMouseClick;
            // Set up TableLayoutPanel
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // Set up SplitContainer
            splitContainer.Panel1Collapsed = false;
            splitContainer.Panel2Collapsed = true;

            // Add controls to TableLayoutPanel
            tableLayoutPanel.Controls.Add(button, 0, 0);
            tableLayoutPanel.Controls.Add(splitContainer, 1, 1);
            tableLayoutPanel.SetRowSpan(splitContainer, 2);
            splitContainer.Panel1.Controls.Add(dataGridFiles);
            splitContainer.Panel2.Controls.Add(dataGridSummary);

            // Add TableLayoutPanel to Form
            Controls.Add(tableLayoutPanel);
        }


        private async Task LoadFileNamesAsync()
        {
            // Populate fileCollection with files from CommonFolderHelper asynchronously
            fileCollection = await Task.Run(CommonFolderHelper.GetFiles);

            // Clear existing rows
            dataGridFiles.Columns.Clear();
            var dataSource = fileCollection.Select(x => new FileView { 
                Date = $"{x.CreationTime:g}",
                FileName = x.Name }).ToList();
            dataGridFiles.DataSource = dataSource;
            dataGridFiles.Refresh();
        }
        private void Button_Click(object sender, EventArgs e)
        {
            if (Program.mainForm == null) return;
            Program.mainForm.menuOpenFile.PerformClick();
        }
        private void DataGridFiles_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Get the selected row index
            int rowIndex = e.RowIndex;
            // check if process can execute
            if (rowIndex < 0 || !dataGridFiles.Enabled) return;
            // check the requested row is not currently the bound row
            if (dataGridSummary.Tag is int currentId && currentId == rowIndex) return;
            dataGridFiles.Enabled = false;
            var requestedFile = fileCollection[rowIndex];
            var selectedRow = dataGridFiles.Rows[rowIndex];
            if (selectedRow.Tag is List<QueryDbResult> results)
            {
                dataGridSummary.Columns.Clear();
                dataGridSummary.DataSource = results;
                dataGridSummary.Refresh();
                dataGridSummary.Tag = rowIndex;
                dataGridFiles.Enabled = true;
                return;
            }
            var collection = requestedFile.GetDataSource() ?? [];
            selectedRow.Tag = collection;
            dataGridSummary.Columns.Clear();
            dataGridSummary.DataSource = collection;
            dataGridSummary.Refresh();
            dataGridSummary.Tag = rowIndex;
            dataGridFiles.Enabled = true;
        }

        private class FileView
        {
            public string Date { get; set; }
            public string FileName { get; set; }
        }
    }
}
