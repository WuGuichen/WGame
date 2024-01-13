using System;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace UosCdn
{
    [Serializable]
    public class UosCdnManager : EditorWindow
    {
        public int LabelWidth = 120;
        public int IndentSize = 20;

        public Vector2 scrollPosition;

        public ParametersBucket pb = ParametersBucket.GetParametersBucket();
        public ParametersRelease pr = ParametersRelease.GetParametersRelease();
        public ParametersEntry pe = ParametersEntry.GetParametersEntry();
        public ParametersBadge pba = ParametersBadge.GetParametersBadge();
        public ParametersAddressableConfig pac = ParametersAddressableConfig.GetParametersAddressableConfig();

        void OnGUI()
        {
            
            EditorGUIUtility.labelWidth = LabelWidth;
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            
            CheckInitInfo();

            BucketArea();
            EntryArea();
            ReleaseArea();
            BadgeArea();
            UosCdnUrlArea();

            GUILayout.Label("");
            GUILayout.Label("  ( You can go to Window/Unity Online Services/CDN/Settings to personalize your configuration. )");
            
            GUILayout.EndScrollView();

        }

        void OnEnable()
        {

        }

        void CheckInitInfo()
        {
            if (string.IsNullOrEmpty(Parameters.uosAppId) || string.IsNullOrEmpty(Parameters.uosAppServiceSecret))
            {
                GUILayout.BeginHorizontal();
                GUIStyle s = new GUIStyle(EditorStyles.label);
                s.normal.textColor = Color.red;
                GUILayout.Label("Plesse set uos app id & secret before using. (Edit -> Project Settings... -> Unity Online Services)", s);
                GUILayout.Label("If you have not got uos app id & secret, please go to uos dashboard first. (Window -> Unity Online Services -> CDN -> Go to Dashboard)", s);
                GUILayout.EndHorizontal();
            }
            
            if (UOSCdnSettings.Settings == null)
            {
                Debug.LogError("Something went wrong when init Unity Online Services settings.");
            }
            
        }


        void BucketArea()
        {
            pb.showBucketArea = EditorGUILayout.Foldout(pb.showBucketArea, pb.showBucketAreaText);

            if (pb.showBucketArea)
            {
                pb.showBucketAreaText = "Bucket";
            }
            else
            {
                pb.showBucketAreaText = string.Format("Bucket ( current bucket : {0})", pb.selectedBucketName);
            }

            if (pb.showBucketArea)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(IndentSize);

                pb.selectedBucketIndex = EditorGUILayout.Popup("Bucket", pb.selectedBucketIndex, pb.bucketNameList);

                if (pb.bucketList.Length > pb.selectedBucketIndex)
                {
                    pb.selectedBucketName = pb.bucketList[pb.selectedBucketIndex].name;
                    pb.selectedBucketUuid = pb.bucketList[pb.selectedBucketIndex].id;
                }
                else
                {
                    pb.selectedBucketName = "";
                    pb.selectedBucketUuid = "";
                }

                GUI.enabled = pb.bucketPreviousButton;
                if (GUILayout.Button(new GUIContent("-", "load previous page"), GUILayout.Width(30)))
                {
                    BucketController.LoadBuckets(pb.currentBucketPage - 1);
                }

                GUI.enabled = true;
                GUILayout.Label(pb.currentBucketPage.ToString(), GUILayout.Width(15));

                GUI.enabled = pb.bucketNextButton;
                if (GUILayout.Button(new GUIContent("+", "load next page"), GUILayout.Width(30)))
                {
                    BucketController.LoadBuckets(pb.currentBucketPage + 1);
                }
                GUI.enabled = true;

                if (GUILayout.Button(new GUIContent("Load", "Load all buckets by page"), GUILayout.Width(50)))
                {
                    BucketController.LoadBuckets();
                }

                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Space(LabelWidth + IndentSize + 2);

                if (GUILayout.Button("New"))
                {
                    Rect r = GUILayoutUtility.GetLastRect();
                    r.x = r.x + position.width / 2 - 100;
                    r.y = r.y + 20;
                    PopupWindow.Show(r, new CreateBucketPopup());
                }

                if (GUILayout.Button("Delete"))
                {
                    int option = EditorUtility.DisplayDialogComplex("Delete Bucket",
                        string.Format("Do you really want to delete bucket - {0}", pb.selectedBucketName),
                        "Yes", "Cancel", "No");

                    if (option == 0)
                    {
                        BucketController.DeleteBucket();
                        BucketController.LoadBuckets();
                    }                    
                }

                if (GUILayout.Button(new GUIContent("Info", "Show detailed bucket information")))
                {
                    BucketController.ViewBucket();
                }

                GUILayout.EndHorizontal();                       
            }
        }

        void EntryArea()
        {
            pe.showEntryAreaText = "Entry";
            pe.showEntryArea = EditorGUILayout.Foldout(pe.showEntryArea, pe.showEntryAreaText);

            if (pe.showEntryArea)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(IndentSize);

                pe.refreshSyncPath();

                EditorGUILayout.LabelField("Data Path", pe.syncPath);
                if (GUILayout.Button(new GUIContent("Choose", "Choose folder to sync with remote server"), GUILayout.Width(84)))
                {
                    pe.syncPath = EditorUtility.OpenFolderPanel("Sync Folder", pe.syncPath, "");
                }

                if (GUILayout.Button(new GUIContent("Sync", "Upload new or updated files in selected folder."), GUILayout.Width(50)))
                {
                    if (ParametersUpload.syncFinished)
                    {
                        UploadWindow.uploadWindow();
                        Thread thread = new Thread(new ParameterizedThreadStart(EntryController.SyncEntries));
                        thread.Start(pe.syncPath);
                    }
                    else
                    {
                        UploadWindow.uploadWindow();
                    }
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(IndentSize);

                pe.selectedEntryIndex = EditorGUILayout.Popup("Entry", pe.selectedEntryIndex, pe.entryNameList);

                if (pe.entryList.Length > pe.selectedEntryIndex)
                {
                    pe.selectedEntryName = pe.entryList[pe.selectedEntryIndex].path;
                    pe.selectedEntryUuid = pe.entryList[pe.selectedEntryIndex].entryid;
                }
                else
                {
                    pe.selectedEntryName = "";
                    pe.selectedEntryUuid = "";
                }

                GUI.enabled = pe.entryPreviousButton;
                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    EntryController.LoadEntries(pe.currentEntryPage - 1);
                }

                GUI.enabled = true;
                GUILayout.Label(pe.currentEntryPage.ToString(), GUILayout.Width(15));

                GUI.enabled = pe.entryNextButton;
                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    EntryController.LoadEntries(pe.currentEntryPage + 1);
                }
                GUI.enabled = true;

                if (GUILayout.Button(new GUIContent("Load", "Load all entries by page."), GUILayout.Width(50)))
                {
                    EntryController.LoadEntries();
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(LabelWidth + IndentSize + 2);

                if (GUILayout.Button(new GUIContent("Upload", "Create or update a single entry, depending on whether existing in server")))
                {
                    if (!ParametersUpload.syncFinished)
                    {
                        UploadWindow.uploadWindow();
                    }
                    else
                    {
                        string fileToUpload = EditorUtility.OpenFilePanel("Select file to upload", "", "");
                        if (string.IsNullOrEmpty(fileToUpload))
                        {
                            return;
                        }

                        UploadWindow.uploadWindow();
                        Thread thread = new Thread(new ParameterizedThreadStart(EntryController.UploadFileManual));
                        thread.Start(fileToUpload);
                    }
                }

                if (GUILayout.Button("Delete"))
                {
                    int option = EditorUtility.DisplayDialogComplex("Delete Entry",
                        string.Format("Do you really want to delete entry - {0}", pe.selectedEntryName),
                        "Yes", "Cancel", "No");

                    if (option == 0)
                    {
                        EntryController.DeleteEntry(pe.selectedEntryUuid, pe.selectedEntryName);
                        EntryController.LoadEntries();
                    }

                }

                if (GUILayout.Button(new GUIContent("Info", "Show detailed entry information")))
                {
                    EntryController.ViewEntry();
                }

                GUILayout.EndHorizontal();
            }
        }

        void ReleaseArea()
        {
            pr.showReleaseArea = EditorGUILayout.Foldout(pr.showReleaseArea, pr.showReleaseAreaText);

            if (pr.showReleaseArea)
            {
                pr.showReleaseAreaText = "Release";
            }
            else
            {
                pr.showReleaseAreaText = string.Format("Release ( current release : {0})", pr.selectedReleaseName);
            }

            if (pr.showReleaseArea)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(IndentSize);

                pr.selectedReleaseIndex = EditorGUILayout.Popup("Release", pr.selectedReleaseIndex, pr.releaseNameList);

                if (pr.releaseList.Length > pr.selectedReleaseIndex)
                {
                    pr.selectedReleaseId = pr.releaseList[pr.selectedReleaseIndex].releaseid;
                    pr.selectedReleaseName = pr.releaseList[pr.selectedReleaseIndex].releasenum.ToString();
                }
                else
                {
                    pr.selectedReleaseId = "";
                    pr.selectedReleaseName = "";
                }

                GUI.enabled = pr.releasePreviousButton;
                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    ReleaseController.LoadReleases(pr.currentReleasePage - 1);
                }

                GUI.enabled = true;
                GUILayout.Label(pr.currentReleasePage.ToString(), GUILayout.Width(15));

                GUI.enabled = pr.releaseNextButton;
                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    ReleaseController.LoadReleases(pr.currentReleasePage + 1);
                }
                GUI.enabled = true;

                if (GUILayout.Button(new GUIContent("Load", "Load all releases by page."), GUILayout.Width(50)))
                {
                    ReleaseController.LoadReleases();
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(LabelWidth + IndentSize + 2);

                if (GUILayout.Button("New"))
                {
                    ReleaseController.CreateRelease();
                    ReleaseController.LoadReleases();
                }

                if (GUILayout.Button(new GUIContent("Promote", "Promote this release to a specified bucket")))
                {
                    Rect r = GUILayoutUtility.GetLastRect();
                    r.x = r.x + position.width / 2 - 100;
                    r.y = r.y + 20;
                    PopupWindow.Show(r, new PromoteReleasePopup());
                }

                if (GUILayout.Button(new GUIContent("Info", "Show detailed release information")))
                {
                    ReleaseController.ViewRelease();
                }

                GUILayout.EndHorizontal();
            }
        }

        void BadgeArea()
        {
            pba.showBadgeArea = EditorGUILayout.Foldout(pba.showBadgeArea, pba.showBadgeAreaText);

            if (pba.showBadgeArea)
            {
                pba.showBadgeAreaText = "Badge";
            }
            else
            {
                pba.showBadgeAreaText = string.Format("Badge ( current badge : {0})", pba.selectedBadgeName);
            }

            if (pba.showBadgeArea)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(IndentSize);

                pba.selectedBadgeIndex = EditorGUILayout.Popup("Badge", pba.selectedBadgeIndex, pba.badgeNameList);

                if (pba.badgeList.Length > pba.selectedBadgeIndex)
                {
                    pba.selectedBadgeName = pba.badgeList[pba.selectedBadgeIndex].name;
                    pba.selectedBadgeLinkedRelease = string.Format("{0} ({1})", pba.badgeList[pba.selectedBadgeIndex].releasenum, pba.badgeList[pba.selectedBadgeIndex].releaseid);
                }
                else
                {
                    pba.selectedBadgeName = "";
                    pba.selectedBadgeLinkedRelease = "";
                }

                GUI.enabled = pba.badgePreviousButton;
                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    BadgeController.LoadBadges(pba.currentBadgePage - 1);
                }

                GUI.enabled = true;
                GUILayout.Label(pba.currentBadgePage.ToString(), GUILayout.Width(15));

                GUI.enabled = pba.badgeNextButton;
                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    BadgeController.LoadBadges(pba.currentBadgePage + 1);
                }
                GUI.enabled = true;

                if (GUILayout.Button(new GUIContent("Load", "Load all badges by page."), GUILayout.Width(50)))
                {
                    BadgeController.LoadBadges();
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(IndentSize);
                EditorGUILayout.LabelField("Linked Release", pba.selectedBadgeLinkedRelease);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(LabelWidth + IndentSize + 2);

                if (GUILayout.Button(new GUIContent("New", "create new badge for current release")))
                {
                    Rect r = GUILayoutUtility.GetLastRect();
                    r.x = r.x + position.width / 2 - 100;
                    r.y = r.y + 20;
                    PopupWindow.Show(r, new CreateBadgePopup());
                }

                if (GUILayout.Button("Delete"))
                {
                    if (pba.selectedBadgeName.Equals("latest"))
                    {
                        EditorUtility.DisplayDialog("Delete Badge Error", "Can not delete badge latest", "OK");
                    }
                    else
                    {
                        int option = EditorUtility.DisplayDialogComplex("Delete Badge",
                        string.Format("Do you really want to delete badge - {0}", pba.selectedBadgeName),
                        "Yes", "Cancel", "No");

                        if (option == 0)
                        {
                            BadgeController.DeleteBadge();
                            BadgeController.LoadBadges();
                        }
                    }
                }

                if (GUILayout.Button(new GUIContent("Update", "assign this badge to current release")))
                {
                    BadgeController.UpdateBadge(pba.selectedBadgeName);
                    BadgeController.LoadBadges(pba.currentBadgePage);
                }

                GUILayout.EndHorizontal();
            }
        }

        void UosCdnUrlArea()
        {
            GUILayout.BeginHorizontal();
            // GUILayout.Space(IndentSize);
            if (pba.badgeList.Length > pba.selectedBadgeIndex)
            {
                pac.remoteLoadUrl = AddressableConfigController.GetRemoteLoadPath(pba.badgeList[pba.selectedBadgeIndex].name);
            }
            GUILayout.Label("Load Url (Badge)", GUILayout.Width(120));
            EditorGUILayout.SelectableLabel(pac.remoteLoadUrl, GUILayout.Height(20));
            GUILayout.EndHorizontal();
#if ADDRESSABLES_EXISTS
            GUILayout.BeginHorizontal();
            GUILayout.Space(LabelWidth + IndentSize + 2);
            
            var text = new GUIContent("Set Addressables Profile", "set remote load path in addressables profile. only works for active profile.");
            if (GUILayout.Button(text))
            {
                if (string.IsNullOrEmpty(pb.selectedBucketUuid))
                {
                    EditorUtility.DisplayDialog("Empty Bucket", "Please select bucket first.", "OK");
                }
                else if (pba.badgeList.Length > pba.selectedBadgeIndex)
                {
                    AddressableConfigController.SetAddressablesProfile(pba.badgeList[pba.selectedBadgeIndex].name);
                }
                else
                {
                    AddressableConfigController.SetAddressablesProfile("");
                }
            }
            GUILayout.EndHorizontal();
#endif
        }

        void DrawLine()
        {
            Rect rr = EditorGUILayout.GetControlRect(GUILayout.Height(10 + 2));
            rr.height = 2;
            rr.y += 10 / 2;
            rr.x -= 3;
            rr.width += 6;
            EditorGUI.DrawRect(rr, Color.grey);
        }
    }

    
}
