using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// LegalLead.PublicData.Search.Classes.ComboBoxEx
namespace LegalLead.PublicData.Search.Classes
{
    public class ComboBoxEx : ComboBox
    {
        public ComboBoxEx()
        {
            base.DropDownStyle = ComboBoxStyle.DropDownList;
            base.DrawMode = DrawMode.OwnerDrawFixed;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e == null)
            {
                base.OnDrawItem(e);
                return;
            }
            e.DrawBackground();
            if (e.State == DrawItemState.Focus)
                e.DrawFocusRectangle();
            var index = e.Index;
            if (index < 0 || index >= Items.Count) return;
            var item = Items[index];
            string displayValue = GetItemText(item);
            using (var brush = new SolidBrush(e.ForeColor))
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                e.Graphics.DrawString(displayValue, e.Font, brush, e.Bounds);
            }
        }
    }
}
