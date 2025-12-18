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
            Add(nameof(UISetterSimple.uiType));
        }

        /// <summary>
        /// Inspectorのカスタマイズ
        /// </summary>
        public override void OnInspectorGUI()
        {
            var setter = target as UISetterSimple;

            serializedObject.Update();

            prop["uiType"].enumValueIndex = (int)(UIType)EditorGUILayout.EnumPopup("UI Type", setter.uiType);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
