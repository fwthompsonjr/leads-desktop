using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegalLead.PublicData.Search.Classes
{
    internal interface IRowStyleChanged
    {
        void ApplyStyles(TableLayoutRowStyleCollection styles, int websiteId);
    }

    internal abstract class RowStyleChangeBase : IRowStyleChanged
    {
        public abstract int WebsiteIndex { get; }
        public abstract List<int> HiddenRows { get; }


        public void ApplyStyles(TableLayoutRowStyleCollection styles, int websiteId)
        {
            if (websiteId != WebsiteIndex) return;
            if (styles == null) return;
            if (HiddenRows == null) return;
            if (!HiddenRows.Any()) return;
            HiddenRows.ForEach(r => styles[r].Height = 0);
        }
    }

    internal class TarrantRowStyleChange : RowStyleChangeBase
    {
        public override int WebsiteIndex => (int)SourceType.TarrantCounty;

        public override List<int> HiddenRows => new List<int> { 3, 4 };

    }

    internal class CollinRowStyleChange : RowStyleChangeBase
    {
        public override int WebsiteIndex => (int)SourceType.CollinCounty;

        public override List<int> HiddenRows => new List<int> { 4 };

    }

    internal class DentonRowStyleChange : RowStyleChangeBase
    {
        public override int WebsiteIndex => (int)SourceType.DentonCounty;

        public override List<int> HiddenRows => new List<int> { 3, 4 };

    }

    internal static class RowStyleChangeProvider
    {
        private static List<IRowStyleChanged> _providers;
        internal static List<IRowStyleChanged> RowChangeProviders
        {
            get { return _providers ?? (_providers = GetProviders()); }
        }

        private static List<IRowStyleChanged> GetProviders()
        {
            return new List<IRowStyleChanged>
            {
                new DentonRowStyleChange(),
                new CollinRowStyleChange(),
                new TarrantRowStyleChange()
            };
        }
    }
}
