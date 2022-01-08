using Harris.Criminal.Db.Downloads;
using Harris.Criminal.Db.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Harris.Criminal.Db.Entities
{
    public class DataLoadDownloads : BaseDataLoad, IDataLoad<HarrisCountyListDto>
    {
        public override List<string> FileNames { get; set; }

        public List<HarrisCountyListDto> Load(bool reset = false)
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

        public async Task<List<HarrisCountyListDto>> LoadAsyc(IProgress<DataLoadDto> progress, bool reset = false)
        {
            // build list of files
            Read(reset);
            var dto = new DataLoadDto
            {
                Count = FileNames.Count,
                StartTime = DateTime.Now
            };

            var tables = new List<HarrisCountyListDto>();
            await Task.Run(() =>
            {
                Map(progress, dto, tables);
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
            const string extn = "*CrimFilingsWithFutureSettings*.txt";
            var directory = new DirectoryInfo(DataFolder);
            var files = directory.GetFiles(extn).ToList();
            FileNames = files.Select(f => f.FullName).ToList();
            FileNames.Sort((a, b) => b.CompareTo(a));
        }
    }
}
