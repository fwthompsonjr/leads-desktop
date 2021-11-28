using Harris.Criminal.Db.Interfaces;
using Harris.Criminal.Db.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Harris.Criminal.Db.Entities
{
    public class DataLoadReferences : BaseDataLoad, IDataLoad<ReferenceTable>
    {

        public override List<string> FileNames { get; set; }

        public List<ReferenceTable> Load(bool reset = false)
        {
            var progressHandler = new Progress<DataLoadDto>(dto =>
            {
                if (dto.IsComplete)
                {
                    Console.WriteLine("Task has completed");
                }
            });
            var progress = progressHandler as IProgress<DataLoadDto>;
            return LoadAsyc(progress).Result;
        }

        public async Task<List<ReferenceTable>> LoadAsyc(IProgress<DataLoadDto> progress, bool reset = false)
        {
            // build list of files
            Read(reset);
            var dto = new DataLoadDto
            {
                Count = FileNames.Count,
                StartTime = DateTime.Now
            };

            var tables = new List<ReferenceTable>();
            await Task.Run(() =>
            {
                Fetch(progress, dto, tables);
            });
            return tables;
        }

        /// <summary>
        /// Reads Reference Data and Stores in Memory
        /// </summary>
        /// <param name="reset"></param>
        private void Read(bool reset = false)
        {
            if (reset == false && FileNames != null)
            {
                return;
            }
            const string extn = "*hcc.tables.*.json";
            var directory = new DirectoryInfo(DataFolder);
            var files = directory.GetFiles(extn).ToList();
            FileNames = files.Select(f => f.FullName).ToList();
        }
    }
}
