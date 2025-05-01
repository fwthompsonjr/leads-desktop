using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public class FormFileViewer : Form
    {
        private TableLayoutPanel tableLayoutPanel;
        private TableLayoutPanel headerLayoutPanel;
        private SplitContainer splitContainer;
        private DataGridView dataGridFiles;
        private DataGridView dataGridSummary;
        private Button button;
        private Button showFilterButton;
        private ComboBox cboFilterContext;
        private TextBox tbxFilterContext;
        private List<FileInfo> fileCollection;

        public FormFileViewer(List<FileInfo> collection = null)
        {
            InitializeComponents();
            dataGridFiles.DataSource = new List<FileView>();
            dataGridFiles.Refresh();
            dataGridFiles.Enabled = false;
            dataGridSummary.DataSource = new List<QueryDbResult>();
            dataGridSummary.Refresh();
            splitContainer.Panel2Collapsed = false;
            if (collection != null)
            {
                fileCollection = collection;
            }

            _ = LoadFileNamesAsync().ContinueWith(_ =>
            {
                // Ensure this runs on the UI thread
                Invoke(() =>
                {
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
            headerLayoutPanel = new TableLayoutPanel
            {
                Name = "headerLayoutPanel",
                ColumnCount = 4,
                RowCount = 1,
                Dock = DockStyle.Fill
            };
            splitContainer = new SplitContainer
            {
                Name = "splitContainer",
                Dock = DockStyle.Fill,
                Padding = new Padding(2)
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
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(2),
            };
            showFilterButton = new Button
            {
                Name = "showFilterButton",
                AutoSize = true,
                Text = "Find",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(2),
                Tag = true
            };
            cboFilterContext = new ComboBox
            {
                Name = "cboFilterContext",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Visible = false
            };
            tbxFilterContext = new TextBox
            {
                Name = "tbxFilterContext",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Visible = false
            };
            cboFilterContext.Items.Add("Date");
            cboFilterContext.Items.Add("FileName");
            cboFilterContext.Refresh();
            cboFilterContext.SelectedIndex = 1;
            cboFilterContext.SelectedIndexChanged += CboFilterContext_SelectedIndexChanged;
            button.Click += Button_Click;
            showFilterButton.Click += FilterButton_Click;
            tbxFilterContext.TextChanged += TextBoxFilter_TextChanged;
            dataGridFiles.RowHeaderMouseClick += DataGridFiles_RowHeaderMouseClick;
            dataGridFiles.RowEnter += DataGridFiles_RowEnter;
            dataGridFiles.CellContentClick += DataGridFiles_CellContentClick;
            // Set up TableLayoutPanel
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            // Set up headerLayoutPanel
            headerLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            headerLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            headerLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            headerLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            headerLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));

            // Set up SplitContainer
            splitContainer.Panel1Collapsed = false;
            splitContainer.Panel1MinSize = 375;
            splitContainer.Panel2Collapsed = true;

            // Add controls to headerLayoutPanel
            headerLayoutPanel.Controls.Add(button, 0, 0);
            headerLayoutPanel.Controls.Add(showFilterButton, 1, 0);
            headerLayoutPanel.Controls.Add(cboFilterContext, 2, 0);
            headerLayoutPanel.Controls.Add(tbxFilterContext, 3, 0);
            // Add controls to TableLayoutPanel
            tableLayoutPanel.Controls.Add(headerLayoutPanel, 0, 0);
            tableLayoutPanel.Controls.Add(splitContainer, 1, 1);
            tableLayoutPanel.SetRowSpan(splitContainer, 2);
            splitContainer.Panel1.Controls.Add(dataGridFiles);
            splitContainer.Panel2.Controls.Add(dataGridSummary);

            // Add TableLayoutPanel to Form
            Controls.Add(tableLayoutPanel);
        }

        private void CboFilterContext_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbxFilterContext.Text = string.Empty;
        }

        private async Task LoadFileNamesAsync()
        {
            // Populate fileCollection with files from CommonFolderHelper asynchronously
            fileCollection ??= await Task.Run(CommonFolderHelper.GetFiles);

            // Clear existing rows
            dataGridFiles.Columns.Clear();
            var dataSource = fileCollection.Select(x => new FileView
            {
                Date = $"{x.CreationTime:g}",
                FileName = x.Name
            }).ToList();
            dataGridFiles.DataSource = dataSource;
            dataGridFiles.MultiSelect = false;
            dataGridFiles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridFiles.ReadOnly = true;
            DataGridViewButtonColumn buttonColumn = new()
            {
                HeaderText = "Open File",
                Text = "Open",
                UseColumnTextForButtonValue = true
            };
            dataGridFiles.Columns.Add(buttonColumn);
            // Set the column properties
            dataGridFiles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridFiles.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridFiles.Columns[0].Width = 150; // Set a fixed width for the first column
            dataGridFiles.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridFiles.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridFiles.Columns[2].Width = 80; // Set a fixed width for the last column
            dataGridFiles.Refresh();
        }
        private void Button_Click(object sender, EventArgs e)
        {
            if (Program.mainForm == null) return;
            Program.mainForm.menuOpenFile.PerformClick();
        }
        private void FilterButton_Click(object sender, EventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.Tag is not bool isEnabled) return;
            cboFilterContext.Visible = isEnabled;
            tbxFilterContext.Visible = isEnabled;
            if (tbxFilterContext.Visible) tbxFilterContext.Text = "";
            btn.Tag = !isEnabled;
        }
        private void DataGridFiles_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            lock (harmony)
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
        }

        private void DataGridFiles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            lock (harmony)
            {
                const int buttonColumnId = 0;
                if (e == null) return;
                if (e.RowIndex < 0) return;
                if (e.ColumnIndex != buttonColumnId) return;
                int rowIndex = e.RowIndex;
                var requestedFile = fileCollection[rowIndex];
                if (!File.Exists(requestedFile.FullName)) return;

                // Disable the button cell
                if (dataGridFiles.Rows[rowIndex].Cells[buttonColumnId] is not DataGridViewButtonCell buttonCell) return;
                if (buttonCell.Tag is bool isWorking && isWorking) return;
                buttonCell.Tag = true;

                _ = Task.Run(() =>
                {
                    OpenFile(requestedFile);
                    buttonCell.Tag = false;
                });
            }
        }

        private void DataGridFiles_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            lock (harmony)
            {
                if (e == null) return;
                if (e.RowIndex < 0) { return; }
                var mouse = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);
                var args = new DataGridViewCellMouseEventArgs(e.ColumnIndex, e.RowIndex, 0, 0, mouse);
                DataGridFiles_RowHeaderMouseClick(sender, args);
            }
        }

        private void TextBoxFilter_TextChanged(object sender, EventArgs e)
        {
            if (!dataGridFiles.Enabled) return;
            dataGridFiles.Enabled = false;
            string filterText = tbxFilterContext.Text?.Trim().ToLower();
            var fieldId = cboFilterContext.SelectedIndex + 1;
            CurrencyManager currencyManager = (CurrencyManager)BindingContext[dataGridFiles.DataSource];
            currencyManager.SuspendBinding();
            foreach (DataGridViewRow row in dataGridFiles.Rows)
            {
                if (string.IsNullOrEmpty(filterText))
                {
                    row.Visible = true;
                }
                else
                {
                    string fileName = row.Cells[fieldId].Value.ToString().ToLower();
                    row.Visible = fileName.Contains(filterText);
                }
            }
            currencyManager.ResumeBinding();
            dataGridFiles.Enabled = true;
        }
        private static void OpenFile(FileInfo fileInfo)
        {
            using Process p = new();
            p.StartInfo = new ProcessStartInfo(fileInfo.FullName)
            {
                UseShellExecute = true
            };
            p.Start();
            // No need to wait for the process to exit
        }

        private class FileView
        {
            public string Date { get; set; }
            public string FileName { get; set; }
        }

        private static readonly object harmony = new object();
    }
}
