using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace UosCdn
{
    public class ReleaseController
    {
        public static ParametersBucket pb = ParametersBucket.GetParametersBucket();
        public static ParametersRelease pr = ParametersRelease.GetParametersRelease();

        public static void CreateRelease()
        {
            if (!Util.checkUosAuth() || string.IsNullOrEmpty(pb.selectedBucketUuid))
            {
                return;
            }

            string url = string.Format("{0}api/v1/buckets/{1}/releases/", Parameters.apiHost, pb.selectedBucketUuid);
            string requestBody = "{}";

            try
            {
                string responseBody = HttpUtil.getHttpResponse(url, "POST", requestBody);
                Debug.Log(string.Format("Create release : {0}", responseBody));
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Create Release Error", e.Message, "OK");
                // Debug.LogError(string.Format("Create release error : {0}", e.Message));
            }
        }
        
        public static void LoadReleases(int page = 1)
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
            if (page > Parameters.totalReleasePages)
            {
                return;
            }
            */

            if (page == 1)
            {
                pr.releasePreviousButton = false;
            }
            else
            {
                pr.releasePreviousButton = true;
            }

            string url = string.Format("{0}api/v1/buckets/{1}/releases/?page={2}&per_page={3}", Parameters.apiHost, pb.selectedBucketUuid, page, Parameters.countPerpage);

            try
            {
                HttpResponse resp = HttpUtil.getHttpResponseWithHeaders(url, "GET");
                string releasesJson = resp.responseBody;
                string pagesPattern = Util.getHeader(resp.headers, "Content-Range");
                if (!string.IsNullOrEmpty(pagesPattern))
                {
                    pr.totalReleaseCounts = int.Parse(pagesPattern.Split('/')[1]);
                    pr.totalReleasePages = ( pr.totalReleaseCounts - 1 ) / 10 + 1;
                    if (page == pr.totalReleasePages)
                    {
                        pr.releaseNextButton = false;
                    }
                    else
                    {
                        pr.releaseNextButton = true;
                    }
                }

                pr.releaseList = JsonUtility.FromJson<RootReleases>("{\"Releases\":" + releasesJson + "}").Releases;
                pr.releaseNameList = new String[pr.releaseList.Length];

                for (int i = 0; i < pr.releaseList.Length; i++)
                {
                    pr.releaseNameList[i] = pr.releaseList[i].releasenum.ToString();
                }

                if (pr.releaseList.Length > 0)
                {
                    Debug.Log("Total " + pr.totalReleaseCounts + " Releases");
                }
                else
                {
                    Debug.Log("No Release for this bucket");
                }

                pr.currentReleasePage = page;
                pr.selectedReleaseIndex = 0;

            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("List Release Error", e.Message, "OK");
                // Debug.LogError(string.Format("List release error : {0}", e.Message));
            }
        }

        public static void PromoteRelease(string toBucket, string notes = "")
        {
            if(!Util.checkUosAuth() || string.IsNullOrEmpty(pb.selectedBucketUuid))
            {
                return;
            }
            
            if (toBucket.Equals(pb.selectedBucketUuid)) {
                EditorUtility.DisplayDialog("Promote Release Warn", "Target bucket can not be the same as selected bucket.", "OK");
                return;
            }

            string url = string.Format("{0}api/v1/buckets/{1}/promote/", Parameters.apiHost, pb.selectedBucketUuid);
            string requestBody = JsonUtility.ToJson(new PromoteReleaseParams(pr.selectedReleaseId, toBucket, notes));

            try
            {
                string responseBody = HttpUtil.getHttpResponse(url, "POST", requestBody);
                Debug.Log(string.Format("Promote release : {0}", responseBody));
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Promote Release Error", e.Message, "OK");
                // Debug.LogError(string.Format("Create release error : {0}", e.Message));
            }
        }

        public static void ViewRelease()
        {
            if (pr.releaseList.Length > 0)
            {
                EditorUtility.DisplayDialog("Release Info", pr.releaseList[pr.selectedReleaseIndex].ToMessage(), "OK");
            }
        }
    }
}