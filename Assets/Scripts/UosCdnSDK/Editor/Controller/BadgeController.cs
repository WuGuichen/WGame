using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace UosCdn
{
    public class BadgeController
    {
        public static ParametersBucket pb = ParametersBucket.GetParametersBucket();
        public static ParametersBadge pba = ParametersBadge.GetParametersBadge();
        public static ParametersRelease pr = ParametersRelease.GetParametersRelease();

        public static void UpdateBadge(string badgeName)
        {
            if (!Util.checkUosAuth() || string.IsNullOrEmpty(pb.selectedBucketUuid) ||
                string.IsNullOrEmpty(pr.selectedReleaseId) || string.IsNullOrEmpty(badgeName))
            {
                return;
            }

            if (badgeName.Equals("latest"))
            {
                EditorUtility.DisplayDialog("Update Badge Error", "Can not change linked release for badge latest", "OK");
                return;
            }

            string url = string.Format("{0}api/v1/buckets/{1}/badges/", Parameters.apiHost, pb.selectedBucketUuid);
            string requestBody = JsonUtility.ToJson(new UpdateBadgeParams(badgeName, pr.selectedReleaseId));

            try
            {
                string responseBody = HttpUtil.getHttpResponse(url, "PUT", requestBody);
                Debug.Log(string.Format("Update Badge : {0}", responseBody));
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Update Badge Error", e.Message, "OK");
                // Debug.LogError(string.Format("Create release error : {0}", e.Message));
            }
        }

        public static void LoadBadges(int page = 1)
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
                pba.badgePreviousButton = false;
            }
            else
            {
                pba.badgePreviousButton = true;
            }

            string url = string.Format("{0}api/v1/buckets/{1}/badges/?page={2}&per_page={3}", Parameters.apiHost, pb.selectedBucketUuid, page, Parameters.countPerpage);

            try
            {
                HttpResponse resp = HttpUtil.getHttpResponseWithHeaders(url, "GET");
                string badgesJson = resp.responseBody;
                string pagesPattern = Util.getHeader(resp.headers, "Content-Range");
                if (!string.IsNullOrEmpty(pagesPattern))
                {
                    pba.totalBadgeCounts = int.Parse(pagesPattern.Split('/')[1]);
                    pba.totalBadgePages = ( pba.totalBadgeCounts - 1 ) / 10 + 1;
                    if (page == pba.totalBadgePages)
                    {
                        pba.badgeNextButton = false;
                    }
                    else
                    {
                        pba.badgeNextButton = true;
                    }
                }

                pba.badgeList = JsonUtility.FromJson<RootBadges>("{\"Badges\":" + badgesJson + "}").Badges;

                pba.badgeNameList = new String[pba.badgeList.Length];

                for (int i = 0; i < pba.badgeList.Length; i++)
                {
                    pba.badgeNameList[i] = pba.badgeList[i].name;
                }

                if (pba.badgeList.Length > 0)
                {
                    Debug.Log("Total " + pba.totalBadgeCounts + " Badges");
                }
                else
                {
                    Debug.Log("No Badge for this bucket");
                }

                pba.currentBadgePage = page;
                pba.selectedBadgeIndex = 0;
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("List Badge Error", e.Message, "OK");
                // Debug.LogError(string.Format("List release error : {0}", e.Message));
            }
        }

        public static void DeleteBadge()
        {
            if (!Util.checkUosAuth() || string.IsNullOrEmpty(pb.selectedBucketUuid) || string.IsNullOrEmpty(pba.selectedBadgeName))
            {
                return;
            }

            string url = string.Format("{0}api/v1/buckets/{1}/badges/{2}/", Parameters.apiHost, pb.selectedBucketUuid, pba.selectedBadgeName);

            try
            {
                string responseBody = HttpUtil.getHttpResponse(url, "DELETE");
                Debug.Log(string.Format("Delete Badge : {0}", pba.selectedBadgeName));
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Delete Badge Error", e.Message, "OK");
                // Debug.LogError(string.Format("Create release error : {0}", e.Message));
            }
        }
    }
}