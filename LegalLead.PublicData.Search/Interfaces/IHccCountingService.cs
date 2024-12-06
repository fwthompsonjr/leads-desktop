using System;

namespace LegalLead.PublicData.Search.Interfaces
{
    public interface IHccCountingService
    {
        int Count(DateTime date);
    }
}