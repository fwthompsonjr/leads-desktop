using Newtonsoft.Json;
using System;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Models
{
    public class InvoiceHistoryModel
    {

        [JsonProperty("countyName")]
        public string CountyName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("recordCount")]
        public int RecordCount { get; set; }

        [JsonProperty("invoiceDate")]
        public DateTime InvoiceDate { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        public InvoiceHeaderModel Model { get; set; }
    }
}
