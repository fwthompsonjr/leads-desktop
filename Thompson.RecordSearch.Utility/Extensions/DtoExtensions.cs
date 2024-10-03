using System.Security.AccessControl;
using Thompson.RecordSearch.Utility.Dto;
using Thompson.RecordSearch.Utility.Models;

namespace Thompson.RecordSearch.Utility.Extensions
{
    public static class DtoExtensions
    {
        public static PersonAddress FromDto(this DallasCaseItemDto dto)
        {
            if (dto == null) return null;
            return new PersonAddress
            {
                Court = dto.Court,
                CaseNumber = dto.CaseNumber,
                CaseStyle = dto.CaseStyle,
                CaseType = dto.CaseType,
                DateFiled = dto.FileDate,
                Status = dto.CaseStatus,
                Name = dto.PartyName,
                Plantiff = dto.Plaintiff,
                Zip = "00000",
                Address1 = "000 No Street",
                Address3 = "Not, NA"
            };
        }
    }
}
