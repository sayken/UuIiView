using System.Collections.Generic;
using UnityEditor;

namespace UuIiView
{
    public class InspectorEditor : Editor
    {
        protected Dictionary<string, SerializedProperty> prop = new Dictionary<string, SerializedProperty>();

        protected void Add(params string[] names)
        {
            foreach (var name in names)
            {
                prop.Add(name, serializedObject.FindProperty(name));
            }
        }
    }
}