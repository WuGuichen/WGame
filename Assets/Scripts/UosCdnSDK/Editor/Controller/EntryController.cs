using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UosCdn
{
    public class EntryController
    {
        public static ParametersEntry pe = ParametersEntry.GetParametersEntry();
        public static ParametersBucket pb = ParametersBucket.GetParametersBucket();

        public static void LoadEntries(int page = 1)
        {
            if (!Util.checkUosAuth() || string.IsNullOrEmpty(pb.selectedBucketUuid))
            {
                return;
            }

            if (page < 1)
            {
                return;
            }

            /*
            if (page > Parameters.totalEntryPages)
            {
                return;
            }
            */

            if (page == 1)
            {
                pe.entryPreviousButton = false;
            }
            else
            {
                pe.entryPreviousButton = true;
            }

            string url = string.Format("{0}api/v1/buckets/{1}/entries/?page={2}&per_page={3}", Parameters.apiHost, pb.selectedBucketUuid, page, Parameters.countPerpage);
            try
            {
                HttpResponse resp = HttpUtil.getHttpResponseWithHeaders(url, "GET");
                string entriesJson = resp.responseBody;
                string pagesPattern = Util.getHeader(resp.headers, "Content-Range"); // 1-10/14
                if (!string.IsNullOrEmpty(pagesPattern))
                {
                    pe.totalEntryCounts = int.Parse(pagesPattern.Split('/')[1]);
                    pe.totalEntryPages = ( pe.totalEntryCounts - 1 ) / 10 + 1;
                    if (page == pe.totalEntryPages)
                    {
                        pe.entryNextButton = false;
                    }
                    else
                    {
                        pe.entryNextButton = true;
                    }
                }

                pe.entryList = JsonUtility.FromJson<RootEntries>("{\"Entries\":" + entriesJson + "}").Entries;
                pe.entryNameList = new String[pe.entryList.Length];

                for (int i = 0; i < pe.entryList.Length; i++)
                {
                    pe.entryNameList[i] = pe.entryList[i].path;
                }

                if (pe.entryList.Length > 0)
                {
                    Debug.Log("Total " + pe.totalEntryCounts + " Entries");
                }
                else
                {
                    Debug.Log("No Entry for this bucket");
                }

                pe.currentEntryPage = page;
                pe.selectedEntryIndex = 0;
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Load entries error : {0}.", e.Message));
            }
        }
        
        public static void ViewEntry()
        {
            if (pe.entryList.Length > 0)
            {
                EditorUtility.DisplayDialog("Entry Info", pe.entryList[pe.selectedEntryIndex].ToMessage(), "OK");
            }
        }

        public static void UploadFileManual(object pathObj)
        {
            if (string.IsNullOrEmpty(pb.selectedBucketUuid))
            {
                return;
            }

            ResetUploadParameters();
            ParametersUpload.unfinishedUploadList = LoadOrCreateUnfinishedUploadFile();

            string filepath = (string) pathObj;

            Entry[] remoteEntries = null;
            try
            {
                remoteEntries = GetRemoteEntries();
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Get remote entries error : {0}", e.Message));
                ParametersUpload.syncFinished = true;
                return;
            }

            EntryInfo ei = Util.getEntryInfoFromLocalFile(filepath);

            bool isUpdate = false;

            foreach (Entry entry in remoteEntries)
            {
                string entryPath = entry.path.StartsWith("/", StringComparison.Ordinal) ? entry.path.Substring(1) : entry.path;
                entryPath = entryPath.Replace("\\", "/");
                if (entryPath.Equals(ei.path))
                {
                    if (ei.content_hash.Equals(entry.content_hash))
                    {
                        Debug.Log("Current file already exist in remote bucket.");
                        ParametersUpload.syncFinished = true;
                        return;
                    }
                    else
                    {
                        Debug.Log(string.Format("Update Entry {0}", entry.path));
                        try
                        {
                            isUpdate = true;
                            ParametersUpload.totalUploadFiles = 1;
                            ParametersUpload.totalUploadSize = ei.content_size;

                            ContentController.UploadContent(ei);
                            UpdateEntry(entry.entryid, ei);
                            DeleteEntryInUnfinished(ei.objectKey);
                            LoadEntries(pe.currentEntryPage);

                            ParametersUpload.alreadyUploadFiles += 1;
                            ParametersUpload.alreadyUploadSize += ei.content_size;

                            Debug.Log("Upload successfully");
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(string.Format("Update entry for {0} failed - {1}", entry.path, e.Message));
                        }
                    }
                    break;
                }
            }

            if (!isUpdate)
            {
                Debug.Log(string.Format("Create Entry {0}", ei.path));
                try
                {
                    ParametersUpload.totalUploadFiles = 1;
                    ParametersUpload.totalUploadSize = ei.content_size;

                    ContentController.UploadContent(ei);
                    CreateEntry(ei);
                    DeleteEntryInUnfinished(ei.objectKey);
                    LoadEntries();

                    ParametersUpload.alreadyUploadFiles += 1;
                    ParametersUpload.alreadyUploadSize += ei.content_size;

                    Debug.Log("Upload successfully");
                }
                catch (Exception e)
                {
                    Debug.LogError(string.Format("Update entry for {0} failed - {1}", ei.path, e.Message));
                }
                
            }

            ParametersUpload.syncFinished = true;
        }

        public static void SyncEntries(object pathObj)
        {
            if (string.IsNullOrEmpty(pb.selectedBucketUuid))
            {
                return;
            }
  
            ResetUploadParameters();
            string path = (string) pathObj;
            Debug.Log("Sync Entries in path : " + path);

            Entry[] remoteEntries = null;
            try
            {
                remoteEntries = GetRemoteEntries();
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Get remote entries error : {0}", e.Message));
                ParametersUpload.syncFinished = true;
                return;
            }

            ParametersUpload.unfinishedUploadList = LoadOrCreateUnfinishedUploadFile();

            Dictionary<string, EntryInfo> localFiles = new Dictionary<string, EntryInfo>();
            try
            {
                localFiles = Util.getLocalFiles(path);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Get Local files error : {0}", e.Message));
                ParametersUpload.syncFinished = true;
                return;
            }
            
            List<EntryInfo> createEntries = new List<EntryInfo>();
            Dictionary<string, EntryInfo> updateEntries = new Dictionary<string, EntryInfo>();
            List<Entry> deleteEntries = new List<Entry>();

            foreach (Entry entry in remoteEntries)
            {
                string entryPath = entry.path.StartsWith("/", StringComparison.Ordinal) ? entry.path.Substring(1) : entry.path;
                entryPath = entryPath.Replace("\\", "/");
                if (localFiles.ContainsKey(entryPath))
                {
                    EntryInfo ei = localFiles[entryPath];
                    if (!ei.content_hash.Equals(entry.content_hash))
                    {
                        updateEntries.Add(entry.entryid, ei);
                        ParametersUpload.totalUploadSize += ei.content_size;
                    }
                    localFiles.Remove(entryPath);
                }
                else
                {
                    deleteEntries.Add(entry);
                }
            }

            foreach (KeyValuePair<string, EntryInfo> kvp in localFiles)
            {
                createEntries.Add(kvp.Value);
                ParametersUpload.totalUploadSize += kvp.Value.content_size;
            }

            ParametersUpload.totalUploadFiles = updateEntries.Count + createEntries.Count;

            var failedEntriesCount = 0;
            if (createEntries.Count > 0)
            {
                Debug.Log("Creating entries ...");
            }
            foreach (EntryInfo entry in createEntries)
            {
                try
                {
                    ContentController.UploadContent(entry);
                    CreateEntry(entry);
                    DeleteEntryInUnfinished(entry.objectKey);
                    ParametersUpload.alreadyUploadFiles += 1;
                    ParametersUpload.alreadyUploadSize += entry.content_size;
                }
                catch (Exception e)
                {
                    Debug.LogError(string.Format("Create entry failed - {0}", e.Message));
                    failedEntriesCount++;
                }
            }

            if (updateEntries.Count > 0)
            {
                Debug.Log("Updating entries ...");
            }
            foreach (KeyValuePair<string, EntryInfo> kvp in updateEntries)
            {
                try
                {
                    ContentController.UploadContent(kvp.Value);
                    UpdateEntry(kvp.Key, kvp.Value);
                    DeleteEntryInUnfinished(kvp.Value.objectKey);

                    ParametersUpload.alreadyUploadFiles += 1;
                    ParametersUpload.alreadyUploadSize += kvp.Value.content_size;
                }
                catch (Exception e)
                {
                    Debug.LogError(string.Format("Update entry for {0} failed - {1}", kvp.Value.path, e.Message));
                    ParametersUpload.failedFiles++;
                }
            }

            if (UOSCdnSettings.Settings.syncWithDelete)
            {
                if (deleteEntries.Count > 0)
                {
                    Debug.Log("Deleting entries ...");
                }

                List<Entry> entries = new List<Entry>();

                foreach (Entry entry in deleteEntries)
                {
                    entries.Add(entry);
                    if (entries.Count == Parameters.deleteMultiCount)
                    {
                        DeleteMultipleEntries(entries);
                        entries.Clear();
                    }
                }

                if (entries.Count > 0)
                {
                    DeleteMultipleEntries(entries);
                    entries.Clear();
                }
            }

            Debug.Log("Sync Finished.");

            if (failedEntriesCount > 0)
            {
                Debug.LogError(string.Format("Fail to create {0} entries, please try later.", failedEntriesCount));
            }

            if (ParametersUpload.failedFiles > 0)
            {
                Debug.LogError(string.Format("Fail to update {0} entries, please try later.", ParametersUpload.failedFiles));
            }

            LoadEntries();
            ParametersUpload.syncFinished = true;
        }


        public static Entry[] GetRemoteEntries()
        {
            Entry[] remoteEntries = null;
            var page = 1;
            var per_page = 1000;
            string url = string.Format("{0}api/v1/buckets/{1}/entries/?page={2}&per_page={3}", Parameters.apiHost, pb.selectedBucketUuid, page, per_page);

            HttpResponse resp = HttpUtil.getHttpResponseWithHeaders(url, "GET");
            string entriesJson = resp.responseBody;
            string pagesPattern = Util.getHeader(resp.headers, "Content-Range"); // 1-10/14
            if (!string.IsNullOrEmpty(pagesPattern))
            {
                var totalCounts = int.Parse(pagesPattern.Split('/')[1]);
                remoteEntries = new Entry[totalCounts];
                var start = (page - 1) * per_page;
                var end = page * per_page > totalCounts ? totalCounts : page * per_page;

                while (true)
                {
                    Array.Copy(JsonUtility.FromJson<RootEntries>("{\"Entries\":" + entriesJson + "}").Entries, 0, remoteEntries, start, end - start);
                    page++;
                    start = (page - 1) * per_page;
                    end = page * per_page > totalCounts ? totalCounts : page * per_page;
                    if (start >= totalCounts)
                    {
                        break;
                    }

                    url = string.Format("{0}api/v1/buckets/{1}/entries/?page={2}&per_page={3}", Parameters.apiHost, pb.selectedBucketUuid, page, per_page);
                    entriesJson = HttpUtil.getHttpResponse(url, "GET");
                }
            }
            else
            {
                remoteEntries = JsonUtility.FromJson<RootEntries>("{\"Entries\":" + entriesJson + "}").Entries;
            }

            return remoteEntries;
        }
       

        public static Entry CreateEntry(EntryInfo entryInfo)
        {
            string url = string.Format("{0}api/v1/buckets/{1}/entries/", Parameters.apiHost, pb.selectedBucketUuid);
            string requestBody = JsonUtility.ToJson(new EntryParams(entryInfo));

            string responseBody = HttpUtil.getHttpResponse(url, "POST", requestBody);
            return JsonUtility.FromJson<Entry>(responseBody);
        }

        public static Entry[] CreateMultipleEntries(List<EntryInfo> entryInfos)
        {
            string url = string.Format("{0}api/v1/buckets/{1}/multientries/", Parameters.apiHost, pb.selectedBucketUuid);
            string requestBody = "[";
            for (int i = 0; i < entryInfos.Count; i++)
            {
                string jsonStr = JsonUtility.ToJson(new EntryParams(entryInfos[i]));
                requestBody = requestBody + jsonStr + ",";
            }

            requestBody = requestBody.Substring(0, requestBody.Length - 1) + "]";
            string responseBody = HttpUtil.getHttpResponse(url, "POST", requestBody);

            Entry[] entries = JsonUtility.FromJson<RootEntries>("{\"Entries\":" + responseBody + "}").Entries;
            return entries;
        }

        public static void DeleteMultipleEntries(List<Entry> entries)
        {
            string url = string.Format("{0}api/v1/buckets/{1}/delmultientries/", Parameters.apiHost, pb.selectedBucketUuid);
            string requestBody = "[";
            for (int i = 0; i < entries.Count; i++)
            {
                string jsonStr = JsonUtility.ToJson(entries[i]);
                requestBody = requestBody + jsonStr + ",";
            }

            requestBody = requestBody.Substring(0, requestBody.Length - 1) + "]";
            try
            {
                string responseBody = HttpUtil.getHttpResponse(url, "POST", requestBody);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Delete bucket error : {0}", e.Message));
            }
        }

        public static Entry UpdateEntry(string entryId, EntryInfo entryInfo)
        {
            string url = string.Format("{0}api/v1/buckets/{1}/entries/{2}/", Parameters.apiHost, pb.selectedBucketUuid, entryId);
            string requestBody = JsonUtility.ToJson(new EntryParams(entryInfo));

            string responseBody = HttpUtil.getHttpResponse(url, "PUT", requestBody);
            return JsonUtility.FromJson<Entry>(responseBody);
        }

        public static void DeleteEntry(string entryId, string entryName = "")
        {
            if (string.IsNullOrEmpty(entryId) || string.IsNullOrEmpty(pb.selectedBucketUuid))
            {
                return;
            }
                   
            string url = string.Format("{0}api/v1/buckets/{1}/entries/{2}/", Parameters.apiHost, pb.selectedBucketUuid, entryId);

            try
            {
                string responseBody = HttpUtil.getHttpResponse(url, "DELETE");
                Debug.Log(string.Format("Delete Entry : {0}", entryName != "" ? entryName : entryId));
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Delete Entry Error", e.Message, "OK");
                // Debug.LogError(string.Format("Create bucket error : {0}", e.Message));
            }

        }

        public static void ResetUploadParameters()
        {
            ParametersUpload.syncFinished = false;
            ParametersUpload.unfinishedUploadList = new UploadPartsStatusList();
            ParametersUpload.unfinishedIndexObjetkeyMapping = new Dictionary<string, int>();

            ParametersUpload.alreadyUploadSize = 0;
            ParametersUpload.alreadyUploadFiles = 0;
            ParametersUpload.totalUploadSize = 0;
            ParametersUpload.totalUploadFiles = 0;
            ParametersUpload.failedFiles = 0;

            ParametersUpload.totalUploadSize4Current = 0;
            ParametersUpload.alreadyUploadSize4Current = 0;
            ParametersUpload.alreadyUploadPartsSize4Current = 0;

            if (false == Directory.Exists(Parameters.k_UploadPartStatusPathPrefix))
            {
                Directory.CreateDirectory(Parameters.k_UploadPartStatusPathPrefix);
            }
        }

        public static UploadPartsStatusList LoadOrCreateUnfinishedUploadFile()
        {
            if (File.Exists(Parameters.k_UploadPartStatusFile))
            {
                using (StreamReader sr = File.OpenText(Parameters.k_UploadPartStatusFile))
                {
                    string data = sr.ReadToEnd();

                    UploadPartsStatusList uploadPartsStatusList = JsonUtility.FromJson<UploadPartsStatusList>(data);
                    for (int i = 0; i < uploadPartsStatusList.unfinishedList.Count; i++)
                    {
                        ParametersUpload.unfinishedIndexObjetkeyMapping.Add(uploadPartsStatusList.unfinishedList[i].objectKey, i);
                    }

                    return JsonUtility.FromJson<UploadPartsStatusList>(data);
                }

            }
            else
            {
                using (FileStream fs = new FileStream(Parameters.k_UploadPartStatusFile, FileMode.CreateNew))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write("{}");
                    sw.Flush();
                }

                return new UploadPartsStatusList();
            }
        }

        public static void DeleteEntryInUnfinished(string objectKey)
        {
            if (ParametersUpload.unfinishedIndexObjetkeyMapping.ContainsKey(objectKey))
            {
                int index = ParametersUpload.unfinishedIndexObjetkeyMapping[objectKey];
                ParametersUpload.unfinishedUploadList.unfinishedList.RemoveAt(index);
                ParametersUpload.unfinishedIndexObjetkeyMapping.Remove(objectKey);

                WriteUnfinishedUploadsIntoFile();
            } 
        }

        public static void WriteUnfinishedUploadsIntoFile()
        {
            if (!File.Exists(Parameters.k_UploadPartStatusFile))
            {
                return;
            }

            File.WriteAllText(Parameters.k_UploadPartStatusFile, JsonUtility.ToJson(ParametersUpload.unfinishedUploadList));
        }
    }
}
