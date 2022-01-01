using LegalLead.PublicData.Search.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            grid.DataSource = null;
            var bindingType = tabStrip.SelectedTab.Text;
            var bindingSource = GetBindingSource(bindingType.ToLower());
            grid.DataSource = bindingSource;
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
                        var heading = s.Description;
                        var labels = s.Labels;
                        labels.Insert(0, heading);
                        return labels;
                    }).SelectMany(i => i)
                    .Select(itm => new { Id = 0, Data = itm });
                    return new BindingSource(lista, "Id");
                default:
                    return null;
            }
        }

        private List<T> GetTag<T>()
        {
            if (Tag == null) return null;
            return Tag as List<T>;
        }
    }
}
