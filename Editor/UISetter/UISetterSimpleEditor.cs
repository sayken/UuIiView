using UnityEditor;

namespace UuIiView
{
    [CustomEditor(typeof(UISetterSimple))]
    public class UISetterSimpleEditor : InspectorEditor
    {
        /// <summary>
        /// 保持する値を設定
        /// </summary>
        private void OnEnable()
        {
            Add(
                nameof(UISetterSimple.uiType),
                nameof(UISetterSimple.uiPanelRoot),
                nameof(UISetterSimple.cellPrefab),
                nameof(UISetterSimple.itemName)
            );
        }

        /// <summary>
        /// Inspectorのカスタマイズ
        /// </summary>
        public override void OnInspectorGUI()
        {
            var setter = target as UISetterSimple;

            serializedObject.Update();

            prop["uiType"].enumValueIndex = (int)(UIType)EditorGUILayout.EnumPopup("UI Type", setter.uiType);

            if (setter.uiType == UIType.List)
            {
                prop["uiPanelRoot"].objectReferenceValue = (UIPanel)EditorGUILayout.ObjectField("UI Panel Root", setter.uiPanelRoot, typeof(UIPanel), true); ;
                prop["cellPrefab"].objectReferenceValue = (UIViewRoot)EditorGUILayout.ObjectField("Cell Prefab", setter.cellPrefab, typeof(UIViewRoot), true);
                prop["itemName"].stringValue = EditorGUILayout.TextField("Item Name", setter.itemName);
            }
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}
