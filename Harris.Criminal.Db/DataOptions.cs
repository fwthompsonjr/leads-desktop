using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harris.Criminal.Db
{
    public static class DataOptions
    {
        private static string _text;
        private static string _dataFile;
        private static string Text => _text ?? (_text = GetText());
        private static string DataFile => _dataFile ?? (_dataFile = GetFileName());

        private static string GetText()
        {
            var dataFile = DataFile;
            if (string.IsNullOrEmpty(dataFile))
            {
                return string.Empty;
            }
            return File.ReadAllText(dataFile);
        }

        private static string GetFileName()
        {
            var appFolder = Startup.AppFolder;
            var dataFile = Path.Combine(appFolder, "_db", "hccc.options.json");
            if (!File.Exists(dataFile))
            {
                return null;
            }
            return dataFile;
        }

        public static string Read()
        {
            return Text;
        }

        public static void Write(string data)
        {
            if (string.IsNullOrEmpty(data)) return;
            var dataFile = DataFile;
            if (string.IsNullOrEmpty(dataFile)) return;
            using (var swriter = new StreamWriter(dataFile))
            {
                swriter.Write(data);
                swriter.Close();
            }
            _text = data;
        }
    }
}
