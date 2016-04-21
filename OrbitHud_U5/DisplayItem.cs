using UnityEngine;

namespace OrbitHud_U5
{
    class DisplayItem
    {
        public string label;
        public string value;

        public GUIStyle valueStyle = new GUIStyle();
        public GUIStyle labelStyle = new GUIStyle();

        public DisplayItem(string label)
        {
            this.label = label;

            this.valueStyle.normal.textColor = Color.green;
            this.valueStyle.alignment = TextAnchor.MiddleLeft;
            this.valueStyle.padding.left = 10;

            this.labelStyle.normal.textColor = Color.green;
            this.labelStyle.alignment = TextAnchor.MiddleRight;
        }
    }
}
