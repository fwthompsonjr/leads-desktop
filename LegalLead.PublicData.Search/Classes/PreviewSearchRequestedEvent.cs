using LegalLead.PublicData.Search.Classes;
using LegalLead.PublicData.Search.Extensions;
using LegalLead.PublicData.Search.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public class PreviewSearchRequestedEvent : WebsiteChangeEvent
    {
        public bool UseMaskedData { get; set; } = true;
        public override string Name => @"Preview";
        public override void Change()
        {
            UnloadPanel();
            var main = GetMain;
            var defaultDef = new DefaultStyleCollection(main);
            defaultDef.HideRows();
            var progres = main.Controls.Find("ButtonDentonSetting", true);
            if (progres == null || progres.Length == 0) return;
            progres[0].Visible = false;
        }
        protected void Reset()
        {
            UnloadPanel();
            base.Change();
            var main = GetMain;
            var styles = main.tableLayoutPanel1.RowStyles;
            var buttonRowStyle = new MenuRowStyleDefinition();
            buttonRowStyle.ApplyStyle(styles, RowsIndexes.ButtonRowId);
            HideProgress();
            ShowButtons();
        }

        public virtual void Toggle(bool isPreview, SearchResult context = null)
        {
            var manager = new PanelManager(GetPanel(), context, UseMaskedData);
            if (isPreview)
            {
                Change();
                manager.Populate();
            }
            else
            {
                Reset();
            }
        }

        private void ShowButtons()
        {
            const string dataButton = "button1";
            const string settingsButton = "ButtonDentonSetting";
            var names = new[] { dataButton, settingsButton };
            foreach (var name in names)
            {
                var controls = GetMain.Controls.Find(name, true).ToList();
                controls.ForEach(control => { control.Visible = name.Equals(dataButton); });
            }
        }

        private void UnloadPanel()
        {
            var manager = new PanelManager(GetPanel());
            manager.Unload();
        }

        protected Panel GetPanel()
        {
            var collection = GetMain.Controls.Find("viewPanel", true);
            if (collection == null || collection.Length == 0) return null;
            foreach (Control control in collection)
            {
                if (control is Panel panel) return panel;
            }
            return null;
        }


        protected class PanelManager(
            Panel source,
            SearchResult search = null,
            bool useMaskedData = true)
        {
            protected readonly Panel viewPanel = source;
            private readonly SearchResult context = search;
            private readonly bool isMasked = useMaskedData;
            public void Unload()
            {

                if (viewPanel == null) return;
                var collection = viewPanel.Controls.Cast<Control>().ToList();
                collection.ForEach(control =>
                {
                    if (control is Form form) form.Close();
                });
                // Remove all controls from the panel
                viewPanel.Controls.Clear();
            }

            public virtual void Populate()
            {
                Unload();
                if (viewPanel == null || context == null) return;
                // Create the TableLayoutPanel
                TableLayoutPanel viewPanelTableLayout = new()
                {
                    RowCount = 5,
                    ColumnCount = 3,
                    Dock = DockStyle.Fill
                };
                // Create button
                Button returnButton = new() { Text = "Return", AutoSize = true };
                Button toggleButton = new() { Text = "Group", Tag = "Off", AutoSize = true };
                returnButton.Click += ReturnButton_Click;
                toggleButton.Click += ToggleButton_Click;
                // Create labels 
                Label vwLabel01A = new() { Text = "Website", AutoSize = true };
                Label vwLabel01B = new() { Text = context.Website, AutoSize = true };
                Label vwLabel02A = new() { Text = "Search", AutoSize = true };
                Label vwLabel02B = new() { Text = context.Search, AutoSize = true };
                Label vwLabel03A = new() { Text = "File", AutoSize = true };
                Label vwLabel03B = new() { Text = Path.GetFileNameWithoutExtension(context.ResultFileName) ?? "-", AutoSize = true };
                Label vwLabel04A = new() { Text = "Records", AutoSize = true };
                Label vwLabel04B = new() { Text = $"{context.AddressList.Count}", AutoSize = true };
                var collection = new List<LabelPair>
                {
                    new(){ Caption = returnButton, Content = toggleButton },
                    new(){ Caption = vwLabel01A, Content = vwLabel01B },
                    new(){ Caption = vwLabel02A, Content = vwLabel02B },
                    new(){ Caption = vwLabel03A, Content = vwLabel03B },
                    new(){ Caption = vwLabel04A, Content = vwLabel04B },
                };
                // Add rows and columns styles
                for (int i = 0; i < collection.Count; i++)
                {
                    viewPanelTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
                }
                viewPanelTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                viewPanelTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
                viewPanelTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
                viewPanelTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                // Add labels to the TableLayoutPanel
                for (int i = 0; i < collection.Count; i++)
                {
                    var current = collection[i];
                    viewPanelTableLayout.Controls.Add(current.Caption, 0, i);
                    viewPanelTableLayout.Controls.Add(current.Content, 1, i);
                    if (i == 0) continue;
                    viewPanelTableLayout.SetColumnSpan(current.Content, 2);
                }
                var addresses = context.AddressList == null ? [] : context.AddressList;
                addresses.RemoveAll(x => string.IsNullOrWhiteSpace(x.DateFiled) || string.IsNullOrWhiteSpace(x.Court));
                if (addresses != null && addresses.Count > 0)
                {
                    // Create and add the DataGridView
                    DataGridView viewPanelDataGrid = new()
                    {
                        Name = "viewPanelDataGrid",
                        Dock = DockStyle.Fill,
                        Padding = new Padding(5),
                        Tag = addresses
                    };
                    addresses.BindGrid(viewPanelDataGrid, isMasked);
                    // Add the DataGridView to the TableLayoutPanel with ColumnSpan = 2
                    viewPanelTableLayout.Controls.Add(viewPanelDataGrid, 0, collection.Count);
                    viewPanelTableLayout.SetColumnSpan(viewPanelDataGrid, 3);
                }
                // Add the TableLayoutPanel to the viewPanel
                viewPanel.Controls.Add(viewPanelTableLayout);
            }

            private static void ReturnButton_Click(object sender, System.EventArgs e)
            {
                if (sender is not Button button) return;
                if (button.FindForm() is not FormMain main) return;
                var items = main.mnuView.DropDownItems;
                if (items.Count == 0) return;
                foreach (var item in items)
                {
                    if (item is not ToolStripMenuItem itm) continue;
                    if (!itm.Checked) continue;
                    itm.PerformClick();
                    break;
                }
            }

            private static void ToggleButton_Click(object sender, System.EventArgs e)
            {
                if (sender is not Button button) return;
                if (button.Tag is not string toggleMode) return;
                if (button.FindForm() is not FormMain main) return;
                var grids = main.Controls.Find("viewPanelDataGrid", true);
                if (grids == null || grids.Length == 0) return;
                var src = grids[0];
                if (src is not DataGridView grid) return;
                if (grid.Tag is not List<QueryDbResponse> query) return;
                var isGrouped = toggleMode != "Off";
                ToggleGrouping(grid, query, isGrouped);
                button.Tag = isGrouped ? "Off" : "On";
            }

            private static void ToggleGrouping(DataGridView grid, List<QueryDbResponse> query, bool isGrouped)
            {
                grid.Columns.Clear();
                if (isGrouped)
                {
                    grid.DataSource = query;
                    grid.Refresh();
                    return;
                }

                var groupedData = query.GroupBy(c => new { c.Court, c.CaseType, c.DateFiled })
                    .Select(g => new SummaryDto
                    {
                        Court = g.Key.Court,
                        CaseType = g.Key.CaseType,
                        DateFiled = g.Key.DateFiled,
                        Count = g.Count()
                    }).ToList();

                var subtotals = groupedData.GroupBy(c => c.Court)
                    .Select(g => new SummaryDto
                    {
                        Court = g.Key,
                        CaseType = "Sub Total:",
                        Count = g.Sum(x => x.Count)
                    }).ToList();
                var sum = groupedData.Sum(x => x.Count);

                groupedData.Sort((a, b) =>
                {
                    var aa = a.Court.CompareTo(b.Court);
                    if (aa != 0) return aa;
                    var bb = a.CaseType.CompareTo(b.CaseType);
                    if (bb != 0) return bb;
                    return a.DateFiled.CompareTo(b.DateFiled);
                });

                var courts = groupedData.Select(x => x.Court).Distinct().ToList();
                var finalList = new List<SummaryDto>();
                foreach (var court in courts)
                {
                    finalList.AddRange(groupedData.FindAll(x => x.Court == court));
                    var subitem = subtotals.Find(x => x.Court == court);
                    if (subitem != null)
                    {
                        subitem.Court = $" + {court} - Subtotal";
                        subitem.CaseType = "";
                        finalList.Add(subitem);
                    }
                }
                finalList.Add(new() { Court = "Grand Total", Count = sum });
                grid.DataSource = finalList;

                // Set the column properties
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                grid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                for (int i = 1; i < 4; i++)
                {
                    grid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    grid.Columns[i].Width = i == 1 ? 300 : 100; // Set a fixed width for the first (3) column
                    if (i == 1) continue;
                    grid.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                grid.Refresh();
            }
            private class LabelPair
            {
                public Control Caption { get; set; }
                public Control Content { get; set; }
            }
            private class SummaryDto
            {
                public string Court { get; set; }
                public string CaseType { get; set; }
                public string DateFiled { get; set; }
                public int Count { get; set; }
            }
        }
    }
}
