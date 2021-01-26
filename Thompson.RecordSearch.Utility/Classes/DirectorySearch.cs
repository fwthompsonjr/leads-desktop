using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thompson.RecordSearch.Utility.Classes
{
    public class DirectorySearch
    {
        public DirectorySearch(DirectoryInfo directoryInfo) : this(directoryInfo, null)
        {

        }
        public DirectorySearch(DirectoryInfo directoryInfo, string search, int searchType = 0)
        {
            Info = directoryInfo ?? throw new ArgumentNullException(nameof(directoryInfo));
            FileList = new List<string>();
            ProgramList = new List<string>();
            Search = search;
            if(searchType == 0)
            {
                SearchDirectory(Info, FileList);
                return;
            }
            
            foreach (var item in ProgramLocations)
            {
                SearchProgram(new DirectoryInfo(item), ProgramList);
            }
            foreach (var item in ProgramList)
            {
                if (FileList.Contains(item)) continue;
                FileList.Add(item);
            }
        }

        public DirectoryInfo Info { get; }
        public List<string> FileList { get; }

        public List<string> ProgramList { get; }

        public string Search { get; }

        private void SearchDirectory(DirectoryInfo dir_info, List<string> file_list)
        {
            try
            {
                foreach (DirectoryInfo subdir_info in dir_info.EnumerateDirectories())
                {
                    SearchDirectory(subdir_info, file_list);
                }
            }
            catch
            {
            }
            try
            {
                var files = (string.IsNullOrEmpty(Search) ?
                    dir_info.EnumerateFiles() :
                    dir_info.EnumerateFiles(Search))
                    .Select(f => f.FullName).ToList();
                foreach (var file_info in files)
                {
                    file_list.Add(file_info);
                }
            }
            catch
            {
            }
        }

        private List<string> ProgramLocations
        {
            get
            {
                return new List<string>
                    {
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
                    }.Distinct().ToList();
            }
        }

        private void SearchProgram(DirectoryInfo dir_info, List<string> file_list)
        {
            try
            {
                foreach (DirectoryInfo subdir_info in dir_info.EnumerateDirectories())
                {
                    SearchProgram(subdir_info, file_list);
                }
            }
            catch
            {
            }
            try
            {
                var files = (string.IsNullOrEmpty(Search) ?
                    dir_info.EnumerateFiles() :
                    dir_info.EnumerateFiles(Search))
                    .Select(f => f.FullName).ToList();
                foreach (var file_info in files)
                {
                    file_list.Add(file_info);
                }
            }
            catch
            {
            }
        }
    }
}
