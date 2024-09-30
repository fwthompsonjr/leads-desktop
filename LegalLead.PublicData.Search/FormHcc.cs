using LegalLead.PublicData.Search.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public partial class FormHcc : Form
    {
        public FormHcc()
        {
            InitializeComponent();
        }

        private void FormHcc_Load(object sender, EventArgs e)
        {
            // onload -- read the data from db folder into the tag propertie for binding...
            var data = HccOption.Read();
            Tag = data;
            BindGrid();
        }

        private void TabStrip_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            var culture = CultureInfo.CurrentCulture;
            grid.DataSource = null;
            grid.Columns.Clear();
            grid.MultiSelect = false;
            var bindingType = tabStrip.SelectedTab.Text;
            var bindingName = bindingType.ToLower(culture);
            var bindingSource = GetBindingSource(bindingName);
            grid.DataSource = bindingSource;
            GetSettings(bindingName);
        }


        private BindingSource GetBindingSource(string bindingName)
        {
            var data = GetTag<HccOption>();
            var datasource = data.FindAll(x => x.Type.Equals(bindingName, StringComparison.OrdinalIgnoreCase));
            switch (bindingName)
            {
                case "general":
                    // get a list of string for the general desciption
                    var lista = datasource.Select(s =>
                    {
                        var heading = s.Name;
                        var labels = s.Labels;
                        labels.Insert(0, heading);
                        return labels;
                    }).SelectMany(i => i)
                    .Select(itm => new HccItem { Id = 0, Data = itm })
                    .ToList();

                    foreach (var item in lista)
                    {
                        item.Id = lista.IndexOf(item);
                    }

                    var aList = new BindingList<HccItem>(lista);
                    return new BindingSource(aList, null);
                case "details":
                    var listb = datasource.Select(s =>
                        new HccDetailItem
                        {
                            Id = s.Index.GetValueOrDefault(),
                            Key = s.Name,
                            Value = HccDetailItem.GetValue(s.Index.GetValueOrDefault())
                        }).ToList();

                    var bList = new BindingList<HccDetailItem>(listb);
                    return new BindingSource(bList, null);
                case "settings":
                    var listc = datasource.Select(s =>
                        new HccSettingItem
                        {
                            Id = data.IndexOf(s),
                            Key = s.Name,
                            Value = s.Index.GetValueOrDefault()
                        }).ToList();

                    var cList = new BindingList<HccSettingItem>(listc);
                    return new BindingSource(cList, null);
                default:
                    return null;
            }
        }

        private void GetSettings(string bindingName)
        {
            var defaultCellFont = new DataGridViewCellStyle
            {
                Font = new Font("Tahoma", 10),
                Alignment = DataGridViewContentAlignment.TopLeft
            };
            var wordWrapStyle = new DataGridViewCellStyle
            {
                Font = new Font("Tahoma", 10),
                WrapMode = DataGridViewTriState.True,
                Alignment = DataGridViewContentAlignment.TopLeft
            };
            var enumerator = grid.Columns.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var column = enumerator.Current as DataGridViewColumn;
                column.DefaultCellStyle = defaultCellFont;
                column.HeaderCell.Style = defaultCellFont;
                column.Visible = column.Index != 0;
                column.ReadOnly = true;
            }
            switch (bindingName)
            {
                case "general":
                    grid.Columns[0].Width = 20;
                    grid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                    break;
                case "details":
                    grid.Columns[0].Width = 30;
                    // Add button for update
                    DataGridViewButtonColumn btnColumn = new DataGridViewButtonColumn
                    {
                        Name = "Update",
                        DefaultCellStyle = defaultCellFont,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
                    };
                    btnColumn.HeaderCell.Style = defaultCellFont;
                    grid.Columns.Add(btnColumn);
                    grid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                    grid.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    var bsrc = (BindingSource)grid.DataSource;
                    var bindList = (BindingList<HccDetailItem>)bsrc.DataSource;
                    var thelist = bindList?.ToList();
                    // apply word wrap to items where id > 300
                    for (int rw = 0; rw < grid.Rows.Count; rw++)
                    {
                        var item = thelist[rw];
                        if (item.Id <= 300)
                        {
                            // handle button ??
                            continue;
                        }
                        var cell = grid.Rows[rw].Cells[2];
                        cell.Style = wordWrapStyle;
                        grid.AutoResizeRow(rw, DataGridViewAutoSizeRowMode.AllCells);
                    }
                    break;
                case "settings":
                    var data = GetTag<HccOption>();
                    var source = (BindingSource)grid.DataSource;
                    var bindingList = (BindingList<HccSettingItem>)source.DataSource;
                    var list = bindingList?.ToList();
                    grid.Columns[0].Width = 30;
                    grid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                    grid.Columns[2].Visible = false;
                    // add new column for Value
                    DataGridViewComboBoxColumn newColumn = new DataGridViewComboBoxColumn
                    {
                        Name = "DataValue",
                        DefaultCellStyle = defaultCellFont,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                    };
                    newColumn.HeaderCell.Style = defaultCellFont;
                    grid.Columns.Add(newColumn);
                    for (int row = 0; row < grid.Rows.Count; row++)
                    {
                        var item = list[row];
                        var src = data[item.Id];
                        DataGridViewComboBoxCell cell =
                            (DataGridViewComboBoxCell)(grid.Rows[row].Cells["DataValue"]);
                        cell.DataSource = data[item.Id].Values;
                        cell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                        cell.Value = src.Values[item.Value];
                        cell.Tag = item;
                        // set the tool tip
                        DataGridViewCell box = grid.Rows[row].Cells[1];
                        box.ToolTipText = src.Description;
                    }
                    break;
                default:
                    break;
            }
        }

        private List<T> GetTag<T>()
        {
            if (Tag == null)
            {
                return null;
            }

            return Tag as List<T>;
        }

        private void Grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewComboBoxCell editted = (DataGridViewComboBoxCell)(grid.Rows[e.RowIndex].Cells[e.ColumnIndex]);
            if (editted.Tag == null) return;
            if (!(editted.Tag is HccSettingItem item)) return;
            var data = GetTag<HccOption>();
            var option = data[item.Id];
            var cellValue = Convert.ToString(editted.Value, CultureInfo.CurrentCulture);
            if (cellValue == null) return;
            var changed = option.Values.IndexOf(cellValue);
            if (changed < 0) return;
            option.Index = changed;
            var json = HccOption.Update(data);
            Tag = json;
        }

        private void Grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            var senderGrid = (DataGridView)sender;
            if (e.RowIndex < 0) return;
            if (!(senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn)) return;


            var bsrc = (BindingSource)grid.DataSource;
            var bindList = (BindingList<HccDetailItem>)bsrc.DataSource;
            var thelist = bindList?.ToList();
            var item = thelist[e.RowIndex];
            var headers = new List<int> { 100, 125, 400 };
            var details = new List<int> { 200, 300, 500 };

            if (headers.Contains(item.Id)) BeginDataLoad("header");
            if (details.Contains(item.Id)) BeginDataLoad("detail");
        }

        private void BeginDataLoad(string processName)
        {
            Console.WriteLine("Begin process for {0}", processName);
        }
    }
}
