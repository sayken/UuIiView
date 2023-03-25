using UnityEngine;
using TMPro;

namespace UuIiView
{
    public class UIDialog : UIPanel
    {
        [SerializeField] RectTransform buttonRoot = default;
        [SerializeField] GameObject positiveButtonPrefab = default;
        [SerializeField] GameObject negativeButtonPrefab = default;

        public void SetButton(string name, string str, bool isPositive)
        {
            var prefab = isPositive ? positiveButtonPrefab : negativeButtonPrefab;
            var go = Instantiate(prefab, buttonRoot);
            go.name = name;
            go.GetComponentInChildren<TextMeshProUGUI>().text = str;
        }
    }
}
