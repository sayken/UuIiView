using UnityEngine.U2D;
using UnityEditor;

namespace UuIiView
{
    [CustomEditor(typeof(UISetterImage))]
    public class UISetterImageEditor : InspectorEditor
    {
        private void OnEnable()
        {
            Add(
                nameof(UISetterImage.spriteHolder),
                nameof(UISetterImage.atlas)
            );
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UISetterImage setter = target as UISetterImage;

            serializedObject.Update();

            if (setter.loadFrom == UISetterImage.LoadFrom.SpriteHolder)
            {
                prop["spriteHolder"].objectReferenceValue = (SpriteHolder)EditorGUILayout.ObjectField("Sprite Holder", setter.spriteHolder, typeof(SpriteHolder), true);
            }
            else if ( setter.loadFrom == UISetterImage.LoadFrom.SpriteAtlas)
            {
                prop["atlas"].objectReferenceValue = (SpriteAtlas)EditorGUILayout.ObjectField("Sprite Atlas", setter.atlas, typeof(SpriteAtlas), true);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}