using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UuIiView
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UISetterText : UISetter
    {
        public const double TimeSpanLimit = -9999; // TimeSpan選択時に、この秒数の時はunderLimitFormatの書式が表示される
        public enum FormatType
        {
            None,
            DateTime,
            Numeric,
            Custom,
            TimeSpan,
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
        public string underLimitFormat = default;
        public string overLimitFormat = default;
        public int max;
        public int min;
        private TextMeshProUGUI textUI;


        public override void Set(object obj)
        {
            if (obj == null) return;

            if (textUI == null) textUI = GetComponent<TextMeshProUGUI>();

            string textStr = string.Empty;
            Color color = Color.white;

            if ( obj.ToString().StartsWith("{") )
            {
                // 2つ以上のパラメータがある
                try
                {
                    var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(obj.ToString());
                    if (dict == null) return;

                    if (dict.TryGetValue("color", out var colorStr) && ColorUtility.TryParseHtmlString(colorStr, out color))
                    {
                        textUI.color = color;
                    }
                    if (dict.TryGetValue("text", out var textValue))
                    {
                        textStr = textValue;
                    }
                }
                catch (Newtonsoft.Json.JsonException e)
                {
                    Debug.LogError($"[UISetterText] {gameObject.name}: JSONのパースに失敗しました。\n{e.Message}");
                    return;
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
            else if ( formatType == FormatType.TimeSpan)
            {
                if ( obj.GetType() == typeof(double) )
                {
                    var remainSec = (double)obj;
                    if ( remainSec <= TimeSpanLimit )
                    {
                        textUI.text = string.Format(underLimitFormat, remainSec);
                    }
                    else
                    {
                        var ts = TimeSpan.FromSeconds(remainSec);
                        textUI.text = ts.ToString(@"mm\:ss");
                    }
                }
            }
            else if (formatType == FormatType.Numeric)
            {
                if (int.TryParse(textStr, out int val))
                {
                    // 表示上限、表示下限があるかどうか
                    bool underLimit = false;
                    bool overLimit = false;
                    if (hasLimit)
                    {
                        underLimit = ( min > val );
                        overLimit = ( max < val );
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
                    string fmt = format;
                    if ( hasLimit )
                    {
                        if ( overLimit )
                        {
                            fmt = overLimitFormat;
                        }
                        else if ( underLimit )
                        {
                            fmt = underLimitFormat;
                        }
                    }
                    
                    if ( !string.IsNullOrEmpty(fmt) )
                    {
                        // それ以外の時のFormat
                        textUI.text = string.Format(fmt, textUI.text);
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
