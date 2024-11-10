using Newtonsoft.Json;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Tools
{
    public static class UserPermissionHelper
    {
        public static PermissionMapDto GetPermissions(int userId)
        {
            var item = DtoMap.Find(x => x.Id == userId) ?? new PermissionMapDto();
            return item;
        }
        private static List<PermissionMapDto> DtoMap
        {
            get
            {
                if (_map != null) return _map;
                var tmp = JsonConvert.DeserializeObject<List<PermissionMapDto>>(permissionsJs) ?? new List<PermissionMapDto>();
                _map = tmp;
                return _map;
            }
        }
        private static List<PermissionMapDto> _map = null;
        private static readonly string permissionsJs = Properties.Resources.user_permissions_list;
    }
}
