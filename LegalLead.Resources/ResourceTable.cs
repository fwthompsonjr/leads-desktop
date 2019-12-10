using LegalLead.Resources.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LegalLead.Resources
{
    public static class ResourceTable
    {
        private const string ResourceFile = @"resource-table.json";
        private const string ResourceFolder = @"text";

        private static string _appFolder;
        private static string _resourceFileName;
        private static ResourceMap _resourceMap;

        public static string ResourceFileName => _resourceFileName ?? (_resourceFileName 
            = Path.Combine(AppFolder, ResourceFolder, ResourceFile));

        public static string AppFolder => _appFolder ?? (_appFolder = GetAppFolderName());

        public static ResourceMap Map => _resourceMap ?? (_resourceMap = GetResourceMap());

        private static List<int> _resourceTypeList;

        private static List<int> ResourceTypeList => _resourceTypeList ?? (_resourceTypeList = GetResourceTypeIdList());

        /// <summary>
        /// Gets the name of the application directory.
        /// </summary>
        /// <returns></returns>
        private static string GetAppFolderName()
        {
            var execName = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            return Path.GetDirectoryName(execName);
        }

        public static IEnumerable<Resource> GetResources(ResourceType resourceType)
        {
            var typeId = (int)resourceType;
            return Map.Resources
                .Where(x => x.Id == typeId);
        }
        public static IEnumerable<Resource> GetResources(ResourceType resourceType, ResourceKeyIndex indexType)
        {
            var typeId = (int)resourceType;
            var keyId = (int)indexType;
            return Map.Resources
                .Where(x => x.Id == typeId)
                .Where(y => y.KeyIndex == keyId);
        }

        public static string GetText(ResourceType resourceType, ResourceKeyIndex indexType)
        {
            var item = GetResources(resourceType, indexType);
            if (item == null) return string.Empty;
            if (item.Count() == 0) return string.Empty;
            return item.First().Value ?? string.Empty;
        }


        public static string GetText(int resourceTypeId, ResourceKeyIndex indexType)
        {
            if (!ResourceTypeList.Contains(resourceTypeId))
            {
                throw new ArgumentOutOfRangeException(nameof(resourceTypeId));
            }
            return GetText((ResourceType)resourceTypeId, indexType);
        }

        private static ResourceMap GetResourceMap()
        {

            if (!File.Exists(ResourceFileName))
            {
                throw new FileNotFoundException();
            }
            using (var reader = new StreamReader(ResourceFileName))
            {
                var data = reader.ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ResourceMap>(data);
            }
        }

        private static List<int> GetResourceTypeIdList()
        {
            var indexes = Map.Resources
                .Select(x => x.Id)
                .Distinct<int>()
                .ToList();
            return indexes;
        }
    
    }
}
