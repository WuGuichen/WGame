using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UosCdn
{
    public class BucketController
    {
        public static ParametersBucket pb = ParametersBucket.GetParametersBucket();

        public static void LoadBuckets(int page = 1)
        {
            if (!Util.checkUosAuth())
            {
                return;
            }

            if (page < 1)
            {
                return;
            }

            /*
            if (page > Parameters.totalBucketPages)
            {
                return;
            }
            */

            if (page == 1)
            {
                pb.bucketPreviousButton = false;
            }
            else
            {
                pb.bucketPreviousButton = true;
            }

            string url = string.Format("{0}api/v1/buckets/?page={1}&per_page={2}", Parameters.apiHost, page, Parameters.countPerpage);

            try
            {
                HttpResponse resp = HttpUtil.getHttpResponseWithHeaders(url, "GET");
                string bucketsJson = resp.responseBody;
                string pagesPattern = Util.getHeader(resp.headers, "Content-Range"); // 1-10/14
                if (!string.IsNullOrEmpty(pagesPattern))
                {
                    pb.totalBucketCounts = int.Parse(pagesPattern.Split('/')[1]);
                    pb.totalBucketPages = ( pb.totalBucketCounts - 1 ) / 10 + 1;

                    if (page == pb.totalBucketPages)
                    {
                        pb.bucketNextButton = false;
                    }
                    else
                    {
                        pb.bucketNextButton = true;
                    }
                }

                pb.bucketList = JsonUtility.FromJson<RootBuckets>("{\"Buckets\":" + bucketsJson + "}").Buckets;
                pb.bucketNameList = new String[pb.bucketList.Length];

                for (int i = 0; i < pb.bucketList.Length; i++)
                {
                    pb.bucketNameList[i] = pb.bucketList[i].name;
                }
                
                if (pb.bucketList.Length > 0)
                {
                    Debug.Log("Total " + pb.totalBucketCounts + " Buckets");
                }
                else
                {
                    Debug.Log("No Bucket for this project");
                }

                pb.currentBucketPage = page;
                pb.selectedBucketIndex = 0;
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Load Bucket Error", e.Message, "OK");
                // Debug.LogError(string.Format("Load buckets error : {0}", e.Message));
            }
        }

        public static void CreateBucket(string bucketName, string bucketDescription = "")
        {
            if (!Util.checkUosAuth() || string.IsNullOrEmpty(bucketName))
            {
                return;
            }

            string url = string.Format("{0}api/v1/buckets/", Parameters.apiHost);
            string requestBody = "{\"name\": \"" + bucketName + "\"}";
            if (!string.IsNullOrEmpty(bucketName))
            {
                requestBody = "{\"name\": \"" + bucketName + "\", \"description\":\"" + bucketDescription + "\"}";
            }

            try
            {
                string responseBody = HttpUtil.getHttpResponse(url, "POST", requestBody);
                Debug.Log(string.Format("Create bucket : {0}", responseBody));

            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Create Bucket Error", e.Message, "OK");
                // Debug.LogError(string.Format("Create bucket error : {0}", e.Message));
            }
        }

        public static void DeleteBucket()
        {
            if (!Util.checkUosAuth() || string.IsNullOrEmpty(pb.selectedBucketUuid))
            {
                return;
            }

            string url = string.Format("{0}api/v1/buckets/{1}/", Parameters.apiHost, pb.selectedBucketUuid);

            try
            {
                string responseBody = HttpUtil.getHttpResponse(url, "DELETE");
                Debug.Log(string.Format("Delete bucket : {0}", pb.selectedBucketName));
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Delete Bucket Error", e.Message, "OK");
                // Debug.LogError(string.Format("Create bucket error : {0}", e.Message));
            }
        }

        public static void ViewBucket()
        {
            if (pb.bucketList.Length > 0)
            {
                EditorUtility.DisplayDialog("Bucket Info", pb.bucketList[pb.selectedBucketIndex].ToMessage(), "OK");
            }       
        }
    }
}