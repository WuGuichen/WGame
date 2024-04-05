namespace WGame.Ability.Editor
{
    using UnityEngine;

    // Layout
    // +--------+----------------+---------------------+
    // |                                               |
    // +--------+----------------+---------------------+
    // |        |                |                     |
    // |        |                |                     |
    // |        |                |                     |
    // | header |   time area    |      inspector      |
    // |        |                |                     |
    // |        |                |                     |
    // |        |                |                     |
    // +--------+----------------+----------+----------+
    internal sealed partial class AbilityEditWindow
    {
        public float toobarHeight = 21f;
        public float timeRulerHeight = 22f;

        private const float minInspectorWidth = 300f;
        private const float maxInspectorWidth = 800f;
        private float _inspectorWidth = 680f;
        public float inspectorWidth
        {
            get => _inspectorWidth;
            set => _inspectorWidth = Mathf.Clamp(value, minInspectorWidth, maxInspectorWidth);
        }

        public float horizontalScrollbarHeight = 18f;
        public float verticalScrollbarWidth = 18f;
        private const float timelineOffsetX = 6f;

        public float propertyHeight = 22f;

        private const float minPropertyWidth = 100f;
        private const float maxPropertyWidth = 450f;
        private float _propertyWidth = 180f;
        public float propertyWidth
        {
            get => _propertyWidth;
            set => _propertyWidth = Mathf.Clamp(value, minPropertyWidth, maxPropertyWidth);
        }

        // header
        private float minHeaderWidth = 195f;
        private float maxHeaderWidth = 500f;

        private float _headerWidth = 225f;
        public float headerWidth
        {
            get => _headerWidth;
            set => _headerWidth = Mathf.Clamp(value, minHeaderWidth, maxHeaderWidth);
        }

        public Rect rectWindow;
        public Rect rectBody;
        public Rect rectTimeArea;
        public Rect rectTimeRuler;
        public Rect rectTimeline;
        public Rect rectContent;
        public Rect rectHeader;

        public Rect rectClient;
        public Rect rectClientView;
        public Rect rectRangebar;

        public Rect rectInspector;
        public Rect rectInspectorLeft;
        public Rect rectInspectorRight;

        private void InitLayout()
        {
            rectWindow = position;
            rectBody = new Rect(0, toobarHeight, position.width - inspectorWidth, position.height - toobarHeight);
            rectClient = new Rect(0, rectBody.y + timeRulerHeight, rectBody.width, rectBody.height - timeRulerHeight);
            rectHeader = new Rect(rectBody.x, rectBody.y + timeRulerHeight, headerWidth, rectBody.height);
            rectTimeRuler = new Rect(rectBody.x + headerWidth + timelineOffsetX, rectBody.y, position.width - headerWidth - inspectorWidth - timelineOffsetX, timeRulerHeight);
            rectTimeline = new Rect(rectTimeRuler.x, rectTimeRuler.y, rectTimeRuler.width, rectBody.height);
            rectTimeArea = new Rect(rectTimeline.x, rectTimeline.y + timeRulerHeight, rectTimeline.width, rectTimeline.height - timeRulerHeight);
            rectContent = new Rect(rectBody.x + headerWidth, rectBody.y + timeRulerHeight, position.width - headerWidth - inspectorWidth, rectBody.height - timeRulerHeight);
            rectClientView = new Rect(0, 0, rectClient.width, rectClient.height);
            rectRangebar = new Rect(headerWidth, position.height - horizontalScrollbarHeight - 2, rectContent.width - 2 * verticalScrollbarWidth, horizontalScrollbarHeight);
            rectInspector = new Rect(position.width - inspectorWidth, toobarHeight, inspectorWidth, position.height - toobarHeight);
            rectInspectorLeft = new Rect(rectInspector.x, rectInspector.y + timeRulerHeight, propertyWidth, rectInspector.height - timeRulerHeight);
            rectInspectorRight = new Rect(rectInspector.x + rectInspectorLeft.width + 2, rectInspectorLeft.y, inspectorWidth - rectInspectorLeft.width - 2, rectInspectorLeft.height);
        }
    }
}