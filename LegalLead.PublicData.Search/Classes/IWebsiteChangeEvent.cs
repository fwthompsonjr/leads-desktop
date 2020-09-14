using System.Windows.Forms;

namespace LegalLead.PublicData.Search
{
    public interface IWebsiteChangeEvent
    {
        FormMain GetMain { get; set; }
        string Name { get; }

        void Change();

        void MapLabels(TableLayoutRowStyleCollection styles);

    }
}