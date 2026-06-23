using UnityEditor;
using UnityEngine;

namespace UuIiView
{
    [InitializeOnLoad]
    internal static class UIHierarchyCustom
    {
        private static readonly Color COLOR = new Color(0f, 0f, 0f, 0.1f);
        private static Texture icon;
        private static int margin = 4;

        static UIHierarchyCustom()
        {
            icon = AssetDatabase.LoadAssetAtPath<Texture>("Packages/com.sayken.uuiiview/Editor/Icons/icon2.png");
#if UNITY_6000_5_OR_NEWER
            EditorApplication.hierarchyWindowItemByEntityIdOnGUI += OnGUI;
#else
            EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
#endif
        }

#if UNITY_6000_5_OR_NEWER
        private static void OnGUI(EntityId entityID, Rect rect)
        {
            bool line = false;
            GameObject go = EditorUtility.EntityIdToObject(entityID) as GameObject;
#else
        private static void OnGUI(int instanceID, Rect rect)
        {
            bool line = false;
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
#endif
            if (go != null)
            {
                var ts = go.GetComponent<UISetter>();
                if (ts != null)
                {
                    line = true;
                }
            }

            if (!line) return;

            Rect r = rect;

            var xMax = rect.xMax;

            rect.x = 32;
            rect.xMax = xMax + 16;

            EditorGUI.DrawRect(rect, COLOR);


            if (icon != null)
            {
                r.width = r.height;
                r.x += EditorStyles.label.CalcSize(go.name).x + 16 + margin;
                GUI.DrawTexture(r, icon);
            }

        }

        public static Vector2 CalcSize(this GUIStyle self, string text)
        {
            var content = new GUIContent(text);
            var size = self.CalcSize(content);
            return size;
        }
    }
}
