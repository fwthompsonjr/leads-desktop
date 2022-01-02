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

        private void BindGrid()
        {
            var culture = CultureInfo.CurrentCulture;
            grid.DataSource = null;
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
                        new HccDetailItem { 
                            Id = s.Index.GetValueOrDefault(),
                            Key = s.Name,
                            Value = HccDetailItem.GetValue(s.Index.GetValueOrDefault())
                        }).ToList();

                    var bList = new BindingList<HccDetailItem>(listb);
                    return new BindingSource(bList, null);
                default:
                    return null;
            }
        }

        private void GetSettings(string bindingName)
        {
            var defaultCellFont = new DataGridViewCellStyle
            {
                Font = new Font("Tahoma", 11)
            };
            var enumerator = grid.Columns.GetEnumerator();
            switch (bindingName)
            {
                case "general":
                    grid.Columns[0].Width = 20;
                    grid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                    while (enumerator.MoveNext())
                    {
                        var column = enumerator.Current as DataGridViewColumn;
                        column.DefaultCellStyle = defaultCellFont;
                        column.HeaderCell.Style = defaultCellFont;
                        column.Visible = column.Index != 0;
                    }
                    break;
                case "details":
                    grid.Columns[0].Width = 30;
                    grid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                    grid.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                    while (enumerator.MoveNext())
                    {
                        var column = enumerator.Current as DataGridViewColumn;
                        column.DefaultCellStyle = defaultCellFont;
                        column.HeaderCell.Style = defaultCellFont;
                        column.Visible = column.Index != 0;
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


        private void TabStrip_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}
