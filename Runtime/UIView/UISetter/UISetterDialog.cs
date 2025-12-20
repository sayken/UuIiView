using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using TMPro;

namespace UuIiView
{
    /// <summary>
    /// ダイアログボタンを動的に生成するUISetter
    /// ポジティブ/ネガティブボタンをデータに基づいて生成する
    /// </summary>
    public class UISetterDialog : UISetter
    {
        /// <summary>ポジティブボタン用プレハブ</summary>
        [SerializeField]
        [Tooltip("OKや確認などのポジティブアクション用ボタンプレハブ")]
        private GameObject positiveButtonPrefab;

        /// <summary>ネガティブボタン用プレハブ</summary>
        [SerializeField]
        [Tooltip("キャンセルや閉じるなどのネガティブアクション用ボタンプレハブ")]
        private GameObject negativeButtonPrefab;

        /// <summary>
        /// ダイアログボタンを生成する
        /// </summary>
        /// <param name="obj">ボタン定義のIList（各要素はIsPositive/EventName/Name/TargetParentNameを持つ）</param>
        public override void Set(object obj)
        {
            if (obj == null) return;

            if (!(obj is IList datas))
            {
                Debug.LogWarning($"[UISetterDialog] {gameObject.name}: objがIListではありません。Type={obj.GetType()}");
                return;
            }

            foreach (var data in datas)
            {
                var dic = ParseToDictionary(data);
                if (dic == null) continue;

                // IsPositiveの安全な取得
                bool isPositive = GetBoolValue(dic, "IsPositive");

                var prefab = isPositive ? positiveButtonPrefab : negativeButtonPrefab;
                if (prefab == null)
                {
                    Debug.LogError($"[UISetterDialog] {gameObject.name}: {(isPositive ? "positiveButtonPrefab" : "negativeButtonPrefab")}が設定されていません。");
                    continue;
                }

                var go = Instantiate(prefab, transform);

                // EventNameの安全な取得
                if (dic.TryGetValue("EventName", out var eventNameObj))
                {
                    go.name = eventNameObj?.ToString() ?? string.Empty;
                }

                // Nameの安全な取得
                if (dic.TryGetValue("Name", out var nameObj))
                {
                    var textComponent = go.GetComponentInChildren<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        textComponent.text = nameObj?.ToString() ?? string.Empty;
                    }
                }

                CustomButton customButton = go.GetComponent<CustomButton>();
                if (customButton != null && dic.TryGetValue("TargetParentName", out var targetParentNameObj))
                {
                    customButton.actionType = ActionType.ActionToPanel;
                    customButton.TargetPanelName = targetParentNameObj?.ToString() ?? string.Empty;
                }
                else if (customButton != null)
                {
                    customButton.actionType = ActionType.Action;
                }
            }
        }

        /// <summary>
        /// オブジェクトをDictionaryに変換する
        /// </summary>
        /// <param name="data">変換元オブジェクト</param>
        /// <returns>変換されたDictionary（失敗時はnull）</returns>
        private Dictionary<string, object> ParseToDictionary(object data)
        {
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(data.ToString());
            }
            catch (JsonException e)
            {
                Debug.LogError($"[UISetterDialog] {gameObject.name}: JSONパースエラー: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// DictionaryからBool値を安全に取得する
        /// </summary>
        /// <param name="dic">取得元Dictionary</param>
        /// <param name="key">キー名</param>
        /// <returns>取得したbool値（取得失敗時はfalse）</returns>
        private bool GetBoolValue(Dictionary<string, object> dic, string key)
        {
            if (!dic.TryGetValue(key, out var value)) return false;

            if (value is bool b) return b;

            bool.TryParse(value?.ToString(), out bool result);
            return result;
        }
    }
}
