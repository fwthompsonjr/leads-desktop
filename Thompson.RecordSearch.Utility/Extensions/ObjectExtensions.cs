using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Thompson.RecordSearch.Utility.Extensions
{
    internal static class ObjectExtensions
    {
        public static void AppendScripts(
            this Dictionary<string, string> collection,
            List<string> scripts)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            string nline = Environment.NewLine;
            var nl = nline.ToCharArray();
            collection.Clear();
            scripts.ForEach(s =>
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    var items = s.Split(nl, StringSplitOptions.RemoveEmptyEntries);
                    var name = items[0].Trim();
                    items[0] = $"/* {name} */";
                    collection.Add(name, string.Join(nline, items));
                }
            });
        }
    }
}
