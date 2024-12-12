using System;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Dto;

namespace LegalLead.PublicData.Search.Interfaces
{
    public interface IHccReadingService
    {
        List<CaseItemDto> Find(DateTime date);
    }
}
