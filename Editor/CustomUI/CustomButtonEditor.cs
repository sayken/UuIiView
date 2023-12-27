using UnityEditor;

namespace UuIiView
{
    [CustomEditor(typeof(CustomButton))]
    public class CustomButtonEditor : InspectorEditor
    {
        private void OnEnable()
        {
            Add(
                nameof(CustomButton.interactable),
                nameof(CustomButton.selected),
                nameof(CustomButton.disabled),
                nameof(CustomButton.targetPanelName)
            );
        }

        public override void OnInspectorGUI()
        {
            var ts = target as CustomButton;

            DrawDefaultInspector();

            serializedObject.Update();
            if ( ts.eventType == EventType.Open )
            {
                prop["targetPanelName"].stringValue = EditorGUILayout.TextField("Target Panel Name", prop["targetPanelName"].stringValue);
            }

            EditorGUILayout.Space();

            prop["interactable"].boolValue = EditorGUILayout.Toggle("interactable", prop["interactable"].boolValue);
            bool modified = serializedObject.hasModifiedProperties;
            serializedObject.ApplyModifiedProperties();

            if ( modified )
            {
                ts.Interactable = prop["interactable"].boolValue;
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("selected", prop["selected"].boolValue);
            EditorGUILayout.Toggle("disabled", prop["disabled"].boolValue);
            EditorGUILayout.LabelField("Target Panel Name", (ts.eventType == EventType.Open) ? prop["targetPanelName"].stringValue : ts.gameObject.name);
            EditorGUI.EndDisabledGroup();

        }
    }
}
