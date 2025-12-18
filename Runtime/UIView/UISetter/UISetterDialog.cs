using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using TMPro;

namespace UuIiView
{
    public class UISetterDialog : UISetter
    {
        [SerializeField] GameObject positiveButtonPrefab;
        [SerializeField] GameObject negativeButtonPrefab;

        public override void Set(object obj)
        {
            if (obj == null) return;

            if (!(obj is IList datas))
            {
                Debug.LogWarning($"[UISetterDialog] {gameObject.name}: objがIListではありません。Type={obj.GetType()}");
                return;
            }

            foreach ( var data in datas )
            {
                var dic = JsonConvert.DeserializeObject<Dictionary<string,object>>(data.ToString());
                if (dic == null) continue;

                // IsPositiveの安全な取得
                bool isPositive = false;
                if (dic.TryGetValue("IsPositive", out var isPositiveObj))
                {
                    if (isPositiveObj is bool b)
                    {
                        isPositive = b;
                    }
                    else
                    {
                        bool.TryParse(isPositiveObj?.ToString(), out isPositive);
                    }
                }

                var prefab = isPositive ? positiveButtonPrefab : negativeButtonPrefab;
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
    }
}
