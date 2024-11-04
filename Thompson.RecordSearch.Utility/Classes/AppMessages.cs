using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Thompson.RecordSearch.Utility.Dto;

namespace Thompson.RecordSearch.Utility.Classes
{
    public static class AppMessages
    {
        public static string GetMessage(string name)
        {
            var item = Messages.Find(x => x.Key.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (item == null) return string.Empty;
            return item.Value ?? string.Empty;
        }

        public static List<AppMessageDto> Messages
        {
            get
            {
                if (_dtos != null) return _dtos;
                var tmp = JsonConvert.DeserializeObject<List<AppMessageDto>>(scriptContent) ?? new List<AppMessageDto>();
                _dtos = tmp;
                return _dtos;
            }
        }

        private static List<AppMessageDto> _dtos;
        private static readonly string scriptContent = Properties.Resources.app_messages;
    }
}
