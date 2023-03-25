using UnityEditor;

namespace UuIiView
{
    [CustomEditor(typeof(CustomToggle))]
    public class CustomToggleEditor : InspectorEditor
    {
        private void OnEnable()
        {
            Add(
                nameof(CustomToggle.interactable),
                nameof(CustomToggle.selected),
                nameof(CustomToggle.disabled),
                nameof(CustomToggle.isOn)
            );
        }

        public override void OnInspectorGUI()
        {
            var ts = target as CustomToggle;

            DrawDefaultInspector();

            serializedObject.Update();
            prop["isOn"].boolValue = EditorGUILayout.Toggle("isOn", prop["isOn"].boolValue);
            prop["interactable"].boolValue = EditorGUILayout.Toggle("interactable", prop["interactable"].boolValue);
            bool modified = serializedObject.hasModifiedProperties;
            serializedObject.ApplyModifiedProperties();

            if ( modified )
            {
                ts.Interactable = prop["interactable"].boolValue;
                ts.IsOn = prop["isOn"].boolValue;
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("selected", prop["selected"].boolValue);
            EditorGUILayout.Toggle("disabled", prop["disabled"].boolValue);
            EditorGUI.EndDisabledGroup();

        }
    }
}