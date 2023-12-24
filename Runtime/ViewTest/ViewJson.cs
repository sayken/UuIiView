using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace UuIiView
{
    [RequireComponent(typeof(UIViewRoot))]
    public class ViewJson : MonoBehaviour
    {
        // テスト用のJsonをセットする
        public List<TextAsset> jsons = new List<TextAsset>();

        // =========================================================================
        // Json作成
        // =========================================================================

        Dictionary<UIType, string> keyTypeTable = new Dictionary<UIType, string>()
        {
            {UIType.Text,"\"texts\""},
            {UIType.Image,"\"path_to_image\""},
            {UIType.RawImage,"\"image_url\""},
            {UIType.GameObject,"true"},
            {UIType.CustomButton,"true"},
            {UIType.CustomToggle,"true"},
            {UIType.Button,"true"},
            {UIType.Toggle,"true"},
            {UIType.Slider,"0f"},
            {UIType.List,"[]"}

        };

        string indent = "";

        public string CreateJson()
        {
            indent = "";
            return CreateJson(null);
        }

        public string CreateJson(GameObject go)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(indent).AppendLine("{");
            AddIndent();

            if (go != null)
            {
                sb.Append(indent).AppendLine("\"Id\":\"Some unique Id\",");
            }
            go ??= gameObject;
            List<string> elements = new List<string>();
            var uiSetters = go.GetComponentsInChildren<UISetter>(true);
            foreach (var _ in uiSetters)
            {
                var str = Check(_);
                if (!string.IsNullOrEmpty(str)) elements.Add(str);
            };
            sb.AppendLine(string.Join(",\n", elements));

            AddIndent(false);
            sb.Append(indent).Append("}");
            return sb.ToString();
        }

        string Check(UISetter uiSetter)
        {
            StringBuilder sb = new StringBuilder();
            string valueSample = "";
            if ( uiSetter.GetType() == typeof(UISetterSimple) )
            {
                var uiSetterSimple = (UISetterSimple)uiSetter;
                if (keyTypeTable.ContainsKey(uiSetterSimple.uiType))
                {
                    valueSample = keyTypeTable[uiSetterSimple.uiType];
                }
            }

            sb.Append(indent).Append("\"").Append(uiSetter.name).Append("\":").Append(valueSample);
            return sb.ToString();
        }

        void AddIndent(bool isAdd = true)
        {
            if (isAdd)
            {
                indent += "\t";
            }
            else if (indent.Length > 0)
            {
                indent = indent.Substring(1);
            }
        }
    }
}