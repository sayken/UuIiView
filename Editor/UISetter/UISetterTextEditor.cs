using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace UuIiView
{
    [CustomEditor(typeof(UISetterText))]
    public class UISetterTextEditor : InspectorEditor
    {
        /// <summary>
        /// 保持する値を設定
        /// </summary>
        private void OnEnable()
        {
            Add(
                nameof(UISetterText.formatType),
                nameof(UISetterText.dateTimeFormat),
                nameof(UISetterText.format),
                nameof(UISetterText.useComma),
                nameof(UISetterText.hasLimit),
                nameof(UISetterText.underLimitFormat),
                nameof(UISetterText.overLimitFormat),
                nameof(UISetterText.max),
                nameof(UISetterText.min)
            );
        }

        /// <summary>
        /// Inspectorのカスタマイズ
        /// </summary>
        public override void OnInspectorGUI()
        {
            var setter = target as UISetterText;

            serializedObject.Update();

        
            prop["formatType"].enumValueIndex = (int)(UISetterText.FormatType)EditorGUILayout.EnumPopup("FormatType", setter.formatType);

            switch (setter.formatType)
            {
                case UISetterText.FormatType.None:
                    // 渡された値をそのまま表示
                    {
                        prop["format"].stringValue = string.Empty;
                    }
                    break;
                case UISetterText.FormatType.DateTime:
                    // 日付のフォーマットで表示
                    {
                        prop["dateTimeFormat"].intValue = EditorGUILayout.Popup("Format", setter.dateTimeFormat, UISetterText.dateFormat);
                    }
                    break;
                case UISetterText.FormatType.Numeric:
                    // 数値として表示
                    {
                        prop["useComma"].boolValue = EditorGUILayout.Toggle("UseComma", setter.useComma);
                        prop["hasLimit"].boolValue = EditorGUILayout.Toggle("HasLimit", setter.hasLimit);

                        if (setter.hasLimit)
                        {
                            prop["min"].intValue = EditorGUILayout.IntField("Min", setter.min);
                            prop["max"].intValue = EditorGUILayout.IntField("Max", setter.max);
                            prop["underLimitFormat"].stringValue = EditorGUILayout.TextField("UnderLimitFormat", setter.underLimitFormat);
                            prop["overLimitFormat"].stringValue = EditorGUILayout.TextField("OverLimitFormat", setter.overLimitFormat);
                        }
                        else
                        {
                            prop["format"].stringValue = EditorGUILayout.TextField("Format", setter.format);
                        }
                    }
                    break;
                case UISetterText.FormatType.Custom:
                    // フォーマットを指定して表示（玄人向け？）
                    {
                        prop["format"].stringValue = EditorGUILayout.TextField("Format", setter.format);
                    }
                    break;
            }
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}
