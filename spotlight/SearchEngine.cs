﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using spotlight.ListItem;
using System.Configuration;

namespace spotlight
{

    public struct SearchItemStruct
    {
        public EFileType Type;
        public SearchItem Item;
    }

    public struct GroupSearchItems
    {
        public EFileType Type;
        public string TypeName;
        public List<FileInformation> Items;

        public override string ToString()
        {
            return $"Type = {Type}, Count = {Items.Count}";
        }
    }

    public class SearchEngine
    {
        public List<string> FileList { get; }
        public List<SearchItemStruct> SearchItems = new List<SearchItemStruct>();
        public List<GroupSearchItems> Groups = new List<GroupSearchItems>();
        public static FileTypesList FileTypesList = new FileTypesList();
        public SearchRangeManager SearchRangeManager = new SearchRangeManager();

        public List<string> FileListExluded { get; set; }

        public int DeepFileSearch { get; private set; }

        string drives = ConfigurationManager.AppSettings["indexedDrives"];


        public SearchEngine()
        {
            DeepFileSearch = 4;

            // Проверка конфигурации

            // Пути поиска
            List<string> paths = new List<string>()
            {
                Environment.GetFolderPath(Environment.SpecialFolder.Favorites),
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
            };
            //var d = new List<string>(ConfigurationManager.AppSettings["indexedDrives"].Split(new char[] { ';' }));
            var dr = ApplicationSettings.AppSet.IndexedDrives.Where(x=>x.isIndexed == true);
            foreach (var item in dr)
            {
                paths.Add(item.Name);
            }
            //paths.AddRange(dr);

            FileList = new List<string>();

            foreach (var path in paths)
            {
                if(path != "")
                {
                    FileList.AddRange(GetFileListDeep(path, "*", DeepFileSearch));
                }                
            }

            GenereateData();
        }

        private void GenereateData()
        {
            List<string> fileList = new List<string>(FileList);

            foreach (FileTypeName fileType in FileTypesList)
            {
                List<string> other = new List<string>();
                List<FileInformation> items = new List<FileInformation>();
                foreach (string path in fileList)
                {
                    if (fileType.Regex == null)
                        items.Add(new FileInformation(path));
                    else
                    {
                        if (fileType.Regex.IsMatch(path))
                            items.Add(new FileInformation(path));
                        else
                            other.Add(path);
                    }
                }
                Groups.Add(new GroupSearchItems()
                {
                    Items = items,
                    Type = fileType.Type,
                    TypeName = fileType.TypeName
                });
                fileList = other;
            }
        }

        public List<GroupSearchItems> FilterData(string filter)
        {
            return FilterData(filter, EFileType.All, 3);
        }

        private class RangeSearchResult
        {
            private struct FileRangeStruct
            {
                public readonly List<FileInformation> Files;
                public readonly int Range;

                public override string ToString()
                {
                    return $"Range = {Range}, Count = {Files.Count}";
                }

                public FileRangeStruct(List<FileInformation> files, int range)
                {
                    Files = files;
                    Range = range;
                }
            }

            private List<FileRangeStruct> data = new List<FileRangeStruct>();

            public void Add(int range, FileInformation file)
            {
                FileRangeStruct rangedFiles = data.Find(item => item.Range == range);
                if (rangedFiles.Files == null)
                {
                    data.Add(new FileRangeStruct(new List<FileInformation>() {file}, range));
                    data.Sort((a, b) => a.Range.CompareTo(b.Range));
                }
                else
                    rangedFiles.Files.Add(file);
            }

            public List<FileInformation> Get(int range)
            {
                FileRangeStruct rangedFiles = data.Find(item => item.Range == range);
                return rangedFiles.Files ?? new List<FileInformation>();
            }

            public List<FileInformation> GetNum(int size)
            {
                if (size == 0)
                {
                    List<FileInformation> all = new List<FileInformation>();
                    data.ForEach(range => all.AddRange(range.Files));
                    return all;
                }
                int left = size;
                List<FileInformation> result = new List<FileInformation>();
                foreach (FileRangeStruct range in data)
                {
                    List<FileInformation> files = range.Files;
                    result.AddRange(files.GetRange(0, Math.Min(files.Count, left)));
                    left -= result.Count;
                    if (left < 0)
                        break;
                }
                return result;
            }

            public int Total()
            {
                return data.Aggregate(0, (last, item) => last + item.Files.Count);
            }

            public int TotalFirstRange()
            {
                return data[0].Files.Count;
            }
        }

        public List<GroupSearchItems> FilterRangeData(string filterQuery, EFileType? filterType, int resultCount)
        {
            string filter;
            EFileType type;
            if (filterType == null)
            {
                SearchInputStruct searchInput = ParseSearchInput(filterQuery);
                filter = searchInput.Search;
                type = searchInput.Type;
                resultCount = 10;
            } else if (filterType == EFileType.App)
            {
                filter = GetSearchIgnoreFilter(filterQuery);
                type = EFileType.App;
            }
            else
            {
                filter = GetSearchIgnoreFilter(filterQuery);
                type = (EFileType) filterType;
            }

            var result = new List<GroupSearchItems>();
            foreach (GroupSearchItems group in Groups)
            {
                if (EFileType.All != type && group.Type != type)
                    continue;

                var rangeSearchResult = new RangeSearchResult();
                foreach (FileInformation file in group.Items)
                {
                    int range = Search(file.DisplayName, filter);
                    if (range > 0)
                    {
                        rangeSearchResult.Add(range, file);
                        if (resultCount > 0 && rangeSearchResult.TotalFirstRange() >= resultCount)
                            break;
                    }
                }
                result.Add(new GroupSearchItems()
                {
                    Items = rangeSearchResult.GetNum(resultCount),
                    Type = group.Type,
                    TypeName = group.TypeName
                });
            }
            return result;
        }

        public List<GroupSearchItems> FilterData(string filter, EFileType type, int resultCount)
        {
            List<GroupSearchItems> result = new List<GroupSearchItems>();
            foreach (GroupSearchItems group in Groups)
            {
                if (!EFileType.All.Equals(type) && group.Type != type)
                    continue;

                List<FileInformation> groupFiles = new List<FileInformation>();
                foreach (FileInformation file in group.Items)
                {
                    if (Search(file.DisplayName, filter) != 0)
                    {
                        /*string name = fileName.Aggregate((bas, substr) => $"{bas} {substr}"); todo Select find pattern*/
                        if (resultCount > 0)
                        {
                            if (groupFiles.Count < resultCount)
                                groupFiles.Add(file);
                            else
                                break;
                        }
                        else
                            groupFiles.Add(file);
                    }
                }
                result.Add(new GroupSearchItems()
                {
                    Items = groupFiles,
                    Type = group.Type,
                    TypeName = group.TypeName
                });
            }
            return result;
        }

        public List<string> GetFileList(string path)
        {
            return GetFileListDeep(path, "*", -1);
        }

        private struct SearchDeep
        {
            public int Deep;
            public int CurrentDeep;
        }

        public List<string> GetFileListDeep(string path, string filter, int deepSize)
        {
            SearchDeep deep = new SearchDeep
            {
                CurrentDeep = 0,
                Deep = deepSize
            };
            return GetFileListDeep(path, filter, deep, new List<string>());
        }

        private List<string> GetFileListDeep(string path, string filter, SearchDeep deep, List<string> cache)
        {
            if (deep.CurrentDeep < deep.Deep || deep.Deep == -1)
            {
                try
                {
                    //IEnumerable <string> dirs = Directory.GetDirectories(path).Where(x=>x.Contains("Recovery") == false).Where(x => x.Contains("System Volume Information") == false);
                    var dirs = new DirectoryInfo(path).GetDirectories().Where(x => (x.Attributes & FileAttributes.Directory) == FileAttributes.Directory).Where(x => (x.Attributes & FileAttributes.Hidden) == 0).Where(x => (x.Attributes & FileAttributes.ReadOnly) == 0);

                    SearchDeep searchDeep = deep;
                    searchDeep.CurrentDeep += 1;
                    foreach (var dir in dirs)
                    {
                        cache = GetFileListDeep(dir.FullName, filter, searchDeep, cache);
                    }              
                }
                catch (Exception e)
                {
                    string es = e.ToString();
                   // FileListExluded.Add(es);
                    Console.WriteLine(e);
                }
                
            }
            string[] files = Directory.GetFiles(path, filter);
            cache.AddRange(files);
            return cache;
        }

        public struct SearchInputStruct
        {
            public EFileType Type;
            public string Search;

            public SearchInputStruct(EFileType type, string search)
            {
                Type = type;
                Search = search;
            }
        }

        public static string GetSearchIgnoreFilter(string search)
        {
            var match = new Regex(@"(\w*?):\s?([\w\W]+)").Match(search);
            return match.Success ? match.Groups[2].Value : search;
        }

        private SearchInputStruct ParseSearchInput(string search)
        {
            Regex regex = new Regex(@"(\w+):\s?([\w\W]+)");
            Match match = regex.Match(search);
            if (match.Success)
            {
                EFileType? fileType = FileTypesList.GetTypeName(match.Groups[1].Value);
                if (fileType != null)
                    return new SearchInputStruct((EFileType) fileType, match.Groups[2].Value);
            }
            return new SearchInputStruct(EFileType.All, search);
        }

        private int Search(string source, string search)
        {
            Regex regex1 = new Regex($"^{search}", RegexOptions.IgnoreCase);
            Match match1 = regex1.Match(source);
            if (match1.Success)
                return 1;

            Regex regex = new Regex($"{search}", RegexOptions.IgnoreCase);
            Match match = regex.Match(source);
            if (match.Success == false)
                return 0;

            int beginMatch = match.Index;
            Regex spaces = new Regex(@"\w+(\W)");
            MatchCollection matchCollection = spaces.Matches(source.Substring(0, beginMatch));
            return matchCollection.Count + 1;
        }

        public void AddQuery(string query, FileInformation file)
        {
            SearchRangeManager.AddQuery(GetSearchIgnoreFilter(query), file.FileLocation);
        }

        

        
    }
}