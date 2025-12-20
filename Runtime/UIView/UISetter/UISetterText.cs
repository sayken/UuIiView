using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UuIiView
{
    /// <summary>
    /// テキスト表示を行うUISetter
    /// 数値フォーマット（カンマ区切り、上限/下限）、日付フォーマット、カスタムフォーマットに対応
    /// JSON形式で色とテキストを同時に指定可能
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UISetterText : UISetter
    {
        /// <summary>TimeSpan表示時の下限値（この値以下でunderLimitFormatを使用）</summary>
        public const double TimeSpanLimit = -9999;

        /// <summary>
        /// テキストフォーマットの種類
        /// </summary>
        public enum FormatType
        {
            /// <summary>フォーマットなし（そのまま表示）</summary>
            None,
            /// <summary>日付フォーマット</summary>
            DateTime,
            /// <summary>数値フォーマット（カンマ区切り、上限/下限対応）</summary>
            Numeric,
            /// <summary>カスタムフォーマット（format文字列を使用）</summary>
            Custom,
            /// <summary>時間間隔フォーマット（mm:ss形式）</summary>
            TimeSpan,
        }

        /// <summary>日付フォーマットの選択肢</summary>
        public static readonly string[] dateFormat = new string[]
        {
            "yyyy-MM-dd",
            "yyyy-MM-dd HH:mm",
            "yyyy-MM-dd HH:mm:ss",
            "HH:mm",
            "mm:ss",
        };

        /// <summary>使用するフォーマットの種類</summary>
        [Tooltip("テキストのフォーマット種類")]
        public FormatType formatType = default;

        /// <summary>日付フォーマットのインデックス（dateFormat配列から選択）</summary>
        [Tooltip("日付フォーマットのインデックス（0-4）")]
        public int dateTimeFormat = 0;

        /// <summary>カスタムフォーマット文字列</summary>
        [Tooltip("カスタムフォーマット文字列（{0}にテキストが入る）")]
        public string format = default;

        /// <summary>数値にカンマ区切りを使用するか</summary>
        [Tooltip("数値をカンマ区切りで表示する")]
        public bool useComma = true;

        /// <summary>数値の上限/下限チェックを有効にするか</summary>
        [Tooltip("数値の上限/下限チェックを有効にする")]
        public bool hasLimit = false;

        /// <summary>下限値を下回った場合のフォーマット</summary>
        [Tooltip("下限値を下回った場合に使用するフォーマット")]
        public string underLimitFormat = default;

        /// <summary>上限値を超えた場合のフォーマット</summary>
        [Tooltip("上限値を超えた場合に使用するフォーマット")]
        public string overLimitFormat = default;

        /// <summary>数値の上限値</summary>
        [Tooltip("数値の上限値")]
        public int max;

        /// <summary>数値の下限値</summary>
        [Tooltip("数値の下限値")]
        public int min;

        /// <summary>TextMeshProUGUIコンポーネント参照</summary>
        private TextMeshProUGUI textUI;

        /// <summary>
        /// テキストデータを設定する
        /// JSON形式（{"color":"#FFFFFF","text":"..."}）または文字列を受け付ける
        /// formatTypeに応じてフォーマット処理を適用
        /// </summary>
        /// <param name="obj">表示するテキストデータ（文字列/JSON/DateTime/数値）</param>
        public override void Set(object obj)
        {
            if (obj == null) return;

            if (textUI == null) textUI = GetComponent<TextMeshProUGUI>();

            string textStr = string.Empty;
            Color color = Color.white;

            if (obj.ToString().StartsWith("{"))
            {
                // JSON形式：色とテキストを同時に指定
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
                textStr = obj.ToString();
            }

            ApplyFormat(obj, textStr);
        }

        /// <summary>
        /// formatTypeに応じてフォーマットを適用する
        /// </summary>
        /// <param name="obj">元のオブジェクト（型判定用）</param>
        /// <param name="textStr">表示する文字列</param>
        private void ApplyFormat(object obj, string textStr)
        {
            switch (formatType)
            {
                case FormatType.None:
                    textUI.text = textStr;
                    break;

                case FormatType.DateTime:
                    ApplyDateTimeFormat(obj);
                    break;

                case FormatType.TimeSpan:
                    ApplyTimeSpanFormat(obj);
                    break;

                case FormatType.Numeric:
                    ApplyNumericFormat(textStr);
                    break;

                case FormatType.Custom:
                    textUI.text = string.Format(format, textStr);
                    break;
            }
        }

        /// <summary>
        /// 日付フォーマットを適用する
        /// </summary>
        /// <param name="obj">DateTime型またはlong型（Ticks）のオブジェクト</param>
        private void ApplyDateTimeFormat(object obj)
        {
            if (obj is DateTime dt)
            {
                textUI.text = dt.ToString(dateFormat[dateTimeFormat]);
            }
            else if (obj is long ticks)
            {
                var datetime = new DateTime(ticks);
                textUI.text = datetime.ToString(dateFormat[dateTimeFormat]);
            }
        }

        /// <summary>
        /// 時間間隔フォーマットを適用する
        /// </summary>
        /// <param name="obj">double型（秒数）のオブジェクト</param>
        private void ApplyTimeSpanFormat(object obj)
        {
            if (obj is double remainSec)
            {
                if (remainSec <= TimeSpanLimit)
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

        /// <summary>
        /// 数値フォーマットを適用する（カンマ区切り、上限/下限対応）
        /// </summary>
        /// <param name="textStr">数値文字列</param>
        private void ApplyNumericFormat(string textStr)
        {
            if (!int.TryParse(textStr, out int val)) return;

            bool underLimit = false;
            bool overLimit = false;

            if (hasLimit)
            {
                underLimit = min > val;
                overLimit = max < val;
                val = Math.Clamp(val, min, max);
            }

            textUI.text = useComma ? string.Format("{0:#,0}", val) : val.ToString();

            string fmt = format;
            if (hasLimit)
            {
                if (overLimit)
                {
                    fmt = overLimitFormat;
                }
                else if (underLimit)
                {
                    fmt = underLimitFormat;
                }
            }

            if (!string.IsNullOrEmpty(fmt))
            {
                textUI.text = string.Format(fmt, textUI.text);
            }
        }
    }
}
