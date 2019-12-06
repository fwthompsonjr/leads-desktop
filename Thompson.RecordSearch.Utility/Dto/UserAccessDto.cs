using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Thompson.RecordSearch.Utility.Classes;
using JConn = Newtonsoft.Json.JsonConvert;
namespace Thompson.RecordSearch.Utility.Dto
{
    public class UserAccessDtoCollection
    {
        public List<UserAccessDto> AccessDtos { get; set; }

    }


    public class UserAccessDto
    {
        public string CreateDate { get; set; }
        public string UserGuid { get; set; }
        public string UserKey { get; set; }
        

        public DateTime? CreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(CreateDate)) return null;
                if (!DateTime.TryParse(CreateDate, out DateTime createdDate)) return null;
                return createdDate;
            }
        }

        public static List<UserAccessDto> GetListDto(string fileSuffix)
        {

            const string dataFormat = @"{0}\xml\{1}.json";
            var appDirectory = ContextManagment.AppDirectory;
            var dataFile = string.Format(dataFormat,
                appDirectory,
                fileSuffix);
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException("Unable to find user access json");
            }
            var data = File.ReadAllText(dataFile);
            var colUsers = JConn.DeserializeObject<List<UserAccessDto>>(data);
            if (colUsers == null) return null;
            return colUsers;
        }

        public static UserAccessDto GetDto(string fileSuffix)
        {

            const string dataFormat = @"{0}\xml\{1}.json";
            var appDirectory = ContextManagment.AppDirectory;
            var dataFile = string.Format(dataFormat,
                appDirectory,
                fileSuffix);
            if (!File.Exists(dataFile))
            {
                throw new FileNotFoundException("Unable to find user access json");
            }
            var data = File.ReadAllText(dataFile);
            var colUsers = JConn.DeserializeObject<List<UserAccessDto>>(data);
            if (colUsers == null) return null;
            if (!colUsers.Any()) return new UserAccessDto();
            return colUsers.Last();
        }

        public static UserAccessDto CreateCredential(string cleared, string userKey, string targetFile)
        {

            var decoded = CryptoEngine.Encrypt(cleared, userKey);
            var dto = new UserAccessDto
            {
                UserKey = userKey,
                UserGuid = decoded,
                CreateDate = DateTime.Now.ToLongDateString()
            };
            var list = GetListDto(targetFile);
            list.Add(dto);

            const string dataFormat = @"{0}\xml\{1}.json";
            var appDirectory = ContextManagment.AppDirectory;
            var dataFile = string.Format(dataFormat,
                appDirectory,
                targetFile);
            if (File.Exists(dataFile)) File.Delete(dataFile);

            using (StreamWriter sw = new StreamWriter(dataFile))
            {
                sw.Write(JConn.SerializeObject(list, Newtonsoft.Json.Formatting.Indented));
            }
            return dto;
        }

        public static List<string> GetCredential(UserAccessDto dto)
        {
            if (dto == null) return null;
            if (string.IsNullOrEmpty(dto.UserGuid)) return null;
            if (string.IsNullOrEmpty(dto.UserKey)) return null;
            var decoded = CryptoEngine.Decrypt(dto.UserGuid, dto.UserKey);
            return decoded.Split('|').ToList();
        }
    }
}
