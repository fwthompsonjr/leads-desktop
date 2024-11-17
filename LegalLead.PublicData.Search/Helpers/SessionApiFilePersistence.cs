using System;
using System.Linq;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Extensions;
using Thompson.RecordSearch.Utility.Models;

namespace LegalLead.PublicData.Search.Helpers
{
    public class SessionApiFilePersistence : SessionFilePersistence
    {
        public override string GetAccountPermissions()
        {
            var bo = Read().ToInstance<LeadUserSecurityBo>();
            if (bo == null) return string.Empty;
            var index = bo.User.IndexData.ToInstance<List<LeadIndexesBo>>()?.FirstOrDefault();
            if (index == null || string.IsNullOrEmpty(index.CountyList)) return string.Empty;
            return index.CountyList;
        }
        public override string GetAccountCredential(string county = "")
        {
            var bo = Read().ToInstance<LeadUserSecurityBo>();
            if (bo == null) return string.Empty;
            var counties = bo.User.CountyData.ToInstance<List<LeadCountyTokenBo>>();
            var selected = counties?.Find(x => x.CountyName.Equals(county, Oic));
            if (selected == null || string.IsNullOrEmpty(selected.Model)) return string.Empty;
            return selected.Model;
        }
        protected override string SetupFile
        {
            get
            {
                if (setupFileName != null) return setupFileName;
                setupFileName = SessionUtil.GetFullFileName(datFileName);
                return setupFileName;
            }
        }
        private sealed class LeadCountyTokenBo
        {
            public string LeadUserId { get; set; } = string.Empty;
            public string CountyName { get; set; } = string.Empty;
            public string Model { get; set; } = string.Empty;
        }
        private sealed class LeadIndexesBo
        {
            public string LeadUserId { get; set; } = string.Empty;
            public string CountyList { get; set; } = string.Empty;
        }
        private static string setupFileName = null;
        private const string datFileName = "session.dtx";
        private static readonly StringComparison Oic = StringComparison.OrdinalIgnoreCase;
    }
}
