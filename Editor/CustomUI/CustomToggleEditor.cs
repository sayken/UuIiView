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
                nameof(CustomToggle.isOn),
                nameof(CustomToggle.targetPanelName),
                nameof(CustomToggle.closeGroupName),
                nameof(CustomToggle.parentName)
            );
        }

        public override void OnInspectorGUI()
        {
            var ts = target as CustomToggle;

            DrawDefaultInspector();

            serializedObject.Update();
            if ( ts.actionType == ActionType.Open || ts.actionType == ActionType.CloseAndOpen || ts.actionType == ActionType.CloseGroupAndOpen || ts.actionType == ActionType.ActionToPanel)
            {
                prop["targetPanelName"].stringValue = EditorGUILayout.TextField("Target Panel Name", prop["targetPanelName"].stringValue);
            }
            if ( ts.actionType == ActionType.CloseGroupAndOpen )
            {
                prop["closeGroupName"].stringValue = EditorGUILayout.TextField("Close Group Name", prop["closeGroupName"].stringValue);
            }

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
            EditorGUILayout.LabelField("Parent Name", prop["parentName"].stringValue);
            EditorGUI.EndDisabledGroup();

        }
    }
}