using System;
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
        };


        public FormatType formatType = default;
        public int dateTimeFormat = 0;
        public string format = default;
        public bool useComma = true;
        public bool hasLimit = false;
        public int max;
        public int min;
        private TextMeshProUGUI text;


        public override void Set(object obj)
        {
            if (text == null) text = GetComponent<TextMeshProUGUI>();


            if (formatType == FormatType.None)
            {
                text.text = obj.ToString();
            }
            else if (formatType == FormatType.DateTime)
            {
                if (obj.GetType() == typeof(DateTime))
                {
                    text.text = string.Format(dateFormat[dateTimeFormat], obj.ToString());
                }
            }
            else if (formatType == FormatType.Numeric)
            {
                if (int.TryParse(obj.ToString(), out int val))
                {
                    // 表示上限、表示下限があるかどうか
                    if (hasLimit)
                    {
                        val = Math.Clamp(val, min, max);
                    }

                    // カンマ区切りにするかどうか
                    if (useComma)
                    {
                        text.text = string.Format("{0:#,0}", val);
                    }
                    else
                    {
                        text.text = val.ToString();
                    }

                    // 最後にフォーマットがあれば、それを使う
                    if ( !string.IsNullOrEmpty(format) )
                    {
                        text.text = string.Format(format, text.text);
                    }
                }
            }
            else if (formatType == FormatType.Custom)
            {
                text.text = string.Format(format, obj.ToString());
            }
        }

    }
}