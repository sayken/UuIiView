using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UuIiView
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UISetterText : UISetter
    {
        public enum FormatType
        {
            None,
            DateTime,
            Numeric,
            Custom,
        }

        // 日付フォーマット。スラッシュ区切りの日付フォーマットはここでは指定できないので、FormatType.Customを使う
        public static readonly string[] dateFormat = new string[]
        {
            "yyyy-MM-dd",
            "yyyy-MM-dd HH:mm",
            "yyyy-MM-dd HH:mm:ss",
            "HH:mm",
            "mm:ss",
        };


        public FormatType formatType = default;
        public int dateTimeFormat = 0;
        public string format = default;
        public bool useComma = true;
        public bool hasLimit = false;
        public int max;
        public int min;
        private TextMeshProUGUI textUI;


        public override void Set(object obj)
        {
            if (textUI == null) textUI = GetComponent<TextMeshProUGUI>();

            string textStr = string.Empty;
            Color color = Color.white;

            if ( obj.ToString().StartsWith("{") )
            {
                // 2つ以上のパラメータがある
                var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(obj.ToString());

                if (ColorUtility.TryParseHtmlString(dict["color"], out color))
                {
                    textUI.color = color;
                }
                if (dict.ContainsKey("text"))
                {
                    textStr = dict["text"];
                }
            }
            else
            {
                // テキスト
                textStr = obj.ToString();
            }


            if (formatType == FormatType.None)
            {
                textUI.text = textStr;
            }
            else if (formatType == FormatType.DateTime)
            {
                if (obj.GetType() == typeof(DateTime))
                {
                    textUI.text = string.Format(dateFormat[dateTimeFormat], obj.ToString());
                }
                else if ( obj.GetType() == typeof(long) )
                {
                    var datetime = new DateTime((long)obj);
                    textUI.text = datetime.ToString(dateFormat[dateTimeFormat]);
                }
            }
            else if (formatType == FormatType.Numeric)
            {
                if (int.TryParse(textStr, out int val))
                {
                    // 表示上限、表示下限があるかどうか
                    if (hasLimit)
                    {
                        val = Math.Clamp(val, min, max);
                    }

                    // カンマ区切りにするかどうか
                    if (useComma)
                    {
                        textUI.text = string.Format("{0:#,0}", val);
                    }
                    else
                    {
                        textUI.text = val.ToString();
                    }

                    // 最後にフォーマットがあれば、それを使う
                    if ( !string.IsNullOrEmpty(format) )
                    {
                        textUI.text = string.Format(format, textUI.text);
                    }
                }
            }
            else if (formatType == FormatType.Custom)
            {
                textUI.text = string.Format(format, textStr);
            }
        }

    }
}
