using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

namespace Motion
{
    class Tooltip
    {
        public GUIStyle style { get; set; }
        public string text { get; set; }

        private GUIStyle _font;

        public GUIStyle font
        {
            get
            {
                if (_font != null)
                    return _font;

                if (style != null)
                    return style;

                _font = new GUIStyle();
                _font.font = EditorStyles.label.font;

                return _font;
            }
            set
            {
                _font = value;
            }
        }

        private float _pad = 4.0f;

        public float Pad
        {
            get => _pad;
            set => _pad = value;
        }

        private GUIContent _textContent;

        GUIContent TextContent
        {
            get
            {
                if (_textContent == null)
                {
                    _textContent = new GUIContent();
                }

                _textContent.text = text;

                return _textContent;
            }
        }

        private Color _foreColor = Color.white;

        public Color ForeColor
        {
            get => _foreColor;
            set => _foreColor = value;
        }

        private Rect _bounds;

        public Rect bounds
        {
            get
            {
                var size = font.CalcSize(TextContent);
                _bounds.width = size.x + (2.0f * Pad);
                _bounds.y -= 2.0f;
                _bounds.height = size.y + 2.0f;
                return _bounds;
            }

            set { _bounds = value; }
        }

        public Tooltip(GUIStyle style, GUIStyle font)
        {
            this.style = style;
            this._font = font;
        }

        public Tooltip()
        {
            style = null;
            _font = null;
        }

        public void Draw()
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (style != null)
            {
                using(new GUIColorOverride(Color.black))
                    GUI.Label(bounds, GUIContent.none, style);
            }

            var textBounds = bounds;
            textBounds.x += Pad;
            textBounds.width -= Pad;

            using (new GUIColorOverride(ForeColor))
            {
                GUI.Label(textBounds, TextContent, font);
            }
        }
    }
}