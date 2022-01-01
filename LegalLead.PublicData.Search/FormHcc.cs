using LegalLead.PublicData.Search.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            switch (bindingName)
            {
                case "general":
                    var general = data.FindAll(x => x.Type.Equals(bindingName, StringComparison.OrdinalIgnoreCase));
                    // get a list of string for the general desciption
                    var lista = general.Select(s =>
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
            switch (bindingName)
            {
                case "general":
                    grid.Columns[0].Width = 20;
                    grid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
                    var enumerator = grid.Columns.GetEnumerator();
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
            if (Tag == null) return null;
            return Tag as List<T>;
        }
    }
}
