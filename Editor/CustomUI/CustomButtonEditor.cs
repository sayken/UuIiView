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
                nameof(CustomButton.targetPanelName),
                nameof(CustomButton.parentName),
                nameof(CustomButton.closeGroupName)
            );
        }

        public override void OnInspectorGUI()
        {
            var ts = target as CustomButton;

            DrawDefaultInspector();

            serializedObject.Update();
            if ( ts.actionType == ActionType.Open || ts.actionType == ActionType.CloseAndOpen || ts.actionType == ActionType.CloseGroupAndOpen)
            {
                prop["targetPanelName"].stringValue = EditorGUILayout.TextField("Target Panel Name", prop["targetPanelName"].stringValue);
            }
            if ( ts.actionType == ActionType.CloseGroupAndOpen )
            {
                prop["closeGroupName"].stringValue = EditorGUILayout.TextField("Close Group Name", prop["closeGroupName"].stringValue);
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
            EditorGUILayout.LabelField("Target Panel Name", (ts.actionType == ActionType.Open) ? prop["targetPanelName"].stringValue : ts.gameObject.name);
            EditorGUILayout.LabelField("Close Group Name", (ts.actionType == ActionType.CloseGroupAndOpen) ? prop["closeGroupName"].stringValue : ts.gameObject.name);
            EditorGUILayout.LabelField("Parent Name", prop["parentName"].stringValue);
            EditorGUI.EndDisabledGroup();

        }
    }
}
