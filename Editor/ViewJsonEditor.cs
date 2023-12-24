using UnityEngine;
using UnityEditor;
using System.IO;

namespace UuIiView
{
    [CustomEditor(typeof(ViewJson))]
    public class ViewJsonEditor : InspectorEditor
    {
        string savePath = "";

        private void OnEnable()
        {
            Add(nameof(ViewJson.jsons));
        }

        public override void OnInspectorGUI()
        {
            var vt = target as ViewJson;

            serializedObject.Update();

            if (!EditorApplication.isPlaying)
            {
                if (GUILayout.Button("Json生成", GUILayout.Width(200)))
                {
                    string json = vt.CreateJson();
                    var path = EditorUtility.SaveFilePanel("", savePath, "viewtest", "json");

                    savePath = Path.GetDirectoryName(path);

                    if (!string.IsNullOrEmpty(path))
                    {
                        File.WriteAllText(path, json);
                    }
                }

                GUILayout.Label("Editor再生中のみ、Testボタンが出ます");
                EditorGUILayout.Separator();
            }

            for (int i = 0; i < vt.jsons.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    prop["jsons"].DeleteArrayElementAtIndex(i);

                    break;
                }
                prop["jsons"].GetArrayElementAtIndex(i).objectReferenceValue = EditorGUILayout.ObjectField("Test Json " + i, prop["jsons"].GetArrayElementAtIndex(i).objectReferenceValue, typeof(TextAsset), false) as TextAsset;


                if (EditorApplication.isPlaying && GUILayout.Button("Test"))
                {
                    vt.GetComponent<UIViewRoot>().SetData(vt.jsons[i].text);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (!EditorApplication.isPlaying && GUILayout.Button("Add", GUILayout.Width(40)))
            {
                prop["jsons"].InsertArrayElementAtIndex(prop["jsons"].arraySize);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}