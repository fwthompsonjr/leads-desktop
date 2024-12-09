using LegalLead.PublicData.Search.Models;
using System.Collections.Generic;

namespace LegalLead.PublicData.Search.Interfaces
{
    public interface IRemoteDbHelper
    {
        FindDbResponse Begin(FindDbRequest findDb);
        FindDbResponse Complete(FindDbRequest findDb);
        List<QueryDbResponse> Query(QueryDbRequest queryDb);
        KeyValuePair<bool, string> Upload(UploadDbRequest uploadDb);
    }
}
