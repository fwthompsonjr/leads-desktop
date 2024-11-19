using System;

namespace LegalLead.PublicData.Search.Extensions
{
    internal static class SourceTypeExtensions
    {
        public static string GetCountyName(this SourceType source)
        {
            var name = Enum.GetName(source);
            name = name.Replace("County", string.Empty);
            name = name.Replace("Civil", string.Empty);
            name = name.Replace("Criminal", string.Empty);
            name = name.Replace("ElPaso", "El Paso");
            name = name.Replace("FortBend", "Fort Bend");
            return name;
        }
    }
}
