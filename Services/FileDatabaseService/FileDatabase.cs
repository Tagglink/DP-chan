using System.IO;
using System.Collections.Generic;
using System.Linq;
using DP_chan.Services.JsonService;
using DP_chan.Services.WebFetchService;
using DP_chan.Extensions;
using System;

namespace DP_chan.Services.FileDatabaseService {
    public class FileDatabase {
        private Random randomizer;
        private Json json;
        private WebFetcher webFetcher;
        private string databasePath;
        private string dataPath;
        private string dataFilename;
        private Dictionary<string, List<string>> files;

        public FileDatabase(Json json, WebFetcher webFetcher, string databasePath, string dataPath)
        {
            randomizer = new Random();
            this.json = json;
            this.webFetcher = webFetcher;
            this.databasePath = databasePath;
            this.dataPath = dataPath;
            dataFilename = "file_database.json";

            files = Load();
            if (files == null)
            {
                files = new Dictionary<string, List<string>>();
            }
 
            if (!Directory.Exists(databasePath))
            {
                Directory.CreateDirectory(databasePath);
            }
        }

        public string AddFile(string group, string uri) {
            string groupPath = GetGroupPath(group);
            string filepath = StringExtensions.GetFilepath(uri, groupPath);

            if (files.ContainsKey(group)) {
                if (files[group].Contains(filepath)){
                    throw new FileAlreadyExistsException();
                }
            } else {
                files.Add(group, new List<string>());
                if (!Directory.Exists(groupPath)) {
                    Directory.CreateDirectory(groupPath);
                }
            }

            webFetcher.DownloadFile(uri, filepath);
            files[group].Add(filepath);
            Save();

            return Path.GetFileName(uri);
        }

        public string GetFile(string group) {
            List<string> fileGroup = files[group];
            if (fileGroup.Count > 0) {
                int r = randomizer.Next(0, fileGroup.Count);
                return fileGroup[r];
            } else {
                throw new FileGroupEmptyException();
            }
        }

        public string GetFile(string group, int index) {
            return files[group][index];
        }

        public string GetFile(string group, string filename) {
            string filepath = GetGroupPath(group) + Path.ChangeExtension(filename, null);
            return files[group].Find((s) => (Path.ChangeExtension(s, null) == filepath));
        }

        public void RemoveGroup(string group) { 
            string groupPath = GetGroupPath(group);
            if (files.Remove(group)) {
                Directory.Delete(groupPath, true);
            } else {
                throw new KeyNotFoundException();
            }
            Save();
        }

        public void RemoveFile(string group, string filename) {
            string filepath = GetGroupPath(group) + filename;
            if (files[group].Remove(filepath)) {
                File.Delete(filepath);
            } else {
                throw new KeyNotFoundException();
            }
            Save();
        }

        private string GetGroupPath(string group){
            return databasePath + group + "/";
        }

        private void Save() {
            Dictionary<string, string[]> jsonFormat = FilesToJsonFormat();
            json.SaveProperly(jsonFormat, dataPath + dataFilename);
        }

        private Dictionary<string, List<string>> Load() {
            Dictionary<string, string[]> jsonFormat = Json.Open<Dictionary<string, string[]>>(dataPath + dataFilename);
            if (jsonFormat == null){
                return null;
            } else {
                return JsonFormatToFiles(jsonFormat);
            }
        }

        private Dictionary<string, string[]> FilesToJsonFormat() {
            Dictionary<string, string[]> ret = new Dictionary<string, string[]>();
            foreach (var pair in files) {
                ret.Add(pair.Key, pair.Value.ToArray());
            }

            return ret;
        }

        private Dictionary<string, List<string>> JsonFormatToFiles(Dictionary<string, string[]> jsonFormat) {
            Dictionary<string, List<string>> ret = new Dictionary<string, List<string>>();
            foreach (var pair in jsonFormat) {
                ret.Add(pair.Key, new List<string>(pair.Value));
            }

            return ret;
        }

        public string[] GetFileGroups() {
            string[] ret = new string[files.Count];
            
            for (int i = 0; i < files.Count; i++) {
                ret[i] = files.ElementAt(i).Key;
            }

            return ret;
        }

        public string[] GetFiles(string group) {
            if (files.ContainsKey(group)) {
                string[] ret = files[group].ToArray();

                for (int i = 0; i < ret.Length; i++) {
                    ret[i] = Path.GetFileName(ret[i]);
                }
                
                return ret;
            }
            else {
                return new string[0];
            }
        }
    }
}