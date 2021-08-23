using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalLead.Changed
{
    public static class JsReader
    {
        public static T Read<T>(string sourceFileName) where T : class
        {
            var content = GetFileContent(sourceFileName);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
        }

        public static void Write(object obj, string sourceFileName)
        {

            var content = Newtonsoft.Json.JsonConvert.SerializeObject(obj, new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            using (var sw = new StreamWriter(sourceFileName, false))
            {
                sw.Write(content);
                sw.Close();
            }
        }
        private static string GetFileContent(string sourceFileName)
        {
            using (var reader = new StreamReader(sourceFileName))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
