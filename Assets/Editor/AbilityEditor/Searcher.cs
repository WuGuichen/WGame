namespace WGame.Ability.Editor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;

    internal static class SearcherStyles
    {
        public const float resultHeight = 20f;
        public const float resultsBorderWidth = 2f;
        public const float resultsMargin = 15f;
        public const float resultsLabelOffset = 2f;

        public static readonly GUIStyle entryEven;
        public static readonly GUIStyle entryOdd;
        public static readonly GUIStyle labelStyle;
        public static readonly GUIStyle resultsBorderStyle;

        public static readonly GUIStyle searchTextField;
        public static readonly GUIStyle searchCancelButton;

        static SearcherStyles()
        {
            entryOdd = new GUIStyle("CN EntryBackOdd");
            entryEven = new GUIStyle("CN EntryBackEven");
            resultsBorderStyle = new GUIStyle("hostview");

            labelStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };

            searchTextField = GUI.skin.FindStyle("SearchTextField");
            searchCancelButton = GUI.skin.FindStyle("SearchCancelButton");
        }
    }

    public sealed class Searcher
    {
        [SerializeField] private List<string> results = new List<string>();
        [SerializeField] private int selectedIndex = -1;

        [System.NonSerialized] private SearchField searchField;
        [System.NonSerialized] private Vector2 previousMousePosition;
        [System.NonSerialized] private bool selectedIndexByMouse;
        [System.NonSerialized] private bool showResults;

        public int maxResults
        {
            get { return 15; }
        }

        public string searchString { get; set; }
        public Action<string> onInputChanged { get; set; }
        public Action<string> onConfirm { get; set; }

        public void AddResult(string result)
        {
            results.Add(result);
        }

        public void ClearResults()
        {
            results.Clear();
        }

        public void OnToolbarGUI()
        {
            Draw(asToolbar: true);
        }

        public void OnGUI()
        {
            Draw(asToolbar: false);
        }

        private void Draw(bool asToolbar)
        {
            var rect = GUILayoutUtility.GetRect(1, 1, 18, 18, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();
            DoSearchField(rect, asToolbar);
            GUILayout.EndHorizontal();
            rect.y += 18;
            DoResults(rect);
        }

        private void DoSearchField(Rect rect, bool asToolbar)
        {
            if (searchField == null)
            {
                searchField = new SearchField();
                searchField.downOrUpArrowKeyPressed += OnDownOrUpArrowKeyPressed;
            }

            var result = asToolbar
                ? searchField.OnToolbarGUI(rect, searchString)
                : searchField.OnGUI(rect, searchString, SearcherStyles.searchTextField,
                    SearcherStyles.searchCancelButton, SearcherStyles.searchCancelButton);

            if (result != searchString && onInputChanged != null)
            {
                onInputChanged(result);
                selectedIndex = -1;
                showResults = true;
            }

            searchString = result;

            if (HasSearchbarFocused())
            {
                RepaintFocusedWindow();
            }
        }

        private void OnDownOrUpArrowKeyPressed()
        {
            var current = Event.current;

            if (current.keyCode == KeyCode.UpArrow)
            {
                current.Use();
                selectedIndex--;
                selectedIndexByMouse = false;
            }
            else
            {
                current.Use();
                selectedIndex++;
                selectedIndexByMouse = false;
            }

            if (selectedIndex >= results.Count) selectedIndex = results.Count - 1;
            else if (selectedIndex < 0) selectedIndex = -1;
        }

        private void DoResults(Rect rect)
        {
            if (results.Count <= 0 || !showResults) return;

            var current = Event.current;
            rect.height = SearcherStyles.resultHeight * Mathf.Min(maxResults, results.Count);
            rect.x = SearcherStyles.resultsMargin;
            rect.width -= SearcherStyles.resultsMargin * 2;

            var elementRect = rect;

            rect.height += SearcherStyles.resultsBorderWidth;
            GUI.Label(rect, "", SearcherStyles.resultsBorderStyle);

            var mouseIsInResultsRect = rect.Contains(current.mousePosition);

            if (mouseIsInResultsRect)
            {
                RepaintFocusedWindow();
            }

            var movedMouseInRect = previousMousePosition != current.mousePosition;

            elementRect.x += SearcherStyles.resultsBorderWidth;
            elementRect.width -= SearcherStyles.resultsBorderWidth * 2;
            elementRect.height = SearcherStyles.resultHeight;

            var didJustSelectIndex = false;

            for (var i = 0; i < results.Count && i < maxResults; i++)
            {
                if (current.type == EventType.Repaint)
                {
                    var style = i % 2 == 0 ? SearcherStyles.entryOdd : SearcherStyles.entryEven;

                    style.Draw(elementRect, false, false, i == selectedIndex, false);

                    var labelRect = elementRect;
                    labelRect.x += SearcherStyles.resultsLabelOffset;
                    GUI.Label(labelRect, results[i], SearcherStyles.labelStyle);
                }

                if (elementRect.Contains(current.mousePosition))
                {
                    if (movedMouseInRect)
                    {
                        selectedIndex = i;
                        selectedIndexByMouse = true;
                        didJustSelectIndex = true;
                    }

                    if (current.type == EventType.MouseDown)
                    {
                        OnConfirm(results[i]);
                    }
                }

                elementRect.y += SearcherStyles.resultHeight;
            }

            if (current.type == EventType.Repaint && !didJustSelectIndex && !mouseIsInResultsRect &&
                selectedIndexByMouse)
            {
                selectedIndex = -1;
            }

            if ((GUIUtility.hotControl != searchField.searchFieldControlID && GUIUtility.hotControl > 0)
                || (current.rawType == EventType.MouseDown && !mouseIsInResultsRect))
            {
                showResults = false;
            }

            if (current.type == EventType.KeyUp && current.keyCode == KeyCode.Return && selectedIndex >= 0)
            {
                OnConfirm(results[selectedIndex]);
            }

            if (current.type == EventType.Repaint)
            {
                previousMousePosition = current.mousePosition;
            }
        }

        private void OnConfirm(string result)
        {
            searchString = result;
            if (onConfirm != null) onConfirm(result);
            if (onInputChanged != null) onInputChanged(result);
            RepaintFocusedWindow();
            GUIUtility.keyboardControl = 0;
        }

        public bool HasSearchbarFocused()
        {
            return GUIUtility.keyboardControl == searchField.searchFieldControlID;
        }

        private void RepaintFocusedWindow()
        {
            if (EditorWindow.focusedWindow != null)
            {
                EditorWindow.focusedWindow.Repaint();
            }
        }
    }
}