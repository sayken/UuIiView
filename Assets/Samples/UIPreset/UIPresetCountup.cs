using UnityEngine;
using TMPro;
using DG.Tweening;
using UuIiView;

namespace UuIiView.Sample
{
    [RequireComponent(typeof(UISetterText))]
    public class UIPresetCountup : MonoBehaviour, IUIPreset
    {

        [SerializeField] float duration = 5f;
        [SerializeField] Ease ease;

        UISetterText uiSetterText;
        int nowNumber;

        private void Awake()
        {
            uiSetterText = GetComponent<UISetterText>();
            var textMesh = GetComponent<TextMeshProUGUI>();
            if (!int.TryParse(textMesh.text, out nowNumber))
            {
                Debug.LogError($"数値ではありません [obj={textMesh.text}]");
                return;
            }

        }

        public void Preset(object obj)
        {
            int targetNumber;
            if (!int.TryParse(obj.ToString(), out targetNumber))
            {
                Debug.LogError($"数値ではありません [obj={obj}]");
                return;
            }


            DOTween
                .To(() => nowNumber, (n) => nowNumber = n, targetNumber, duration)
                .SetEase(ease)
                .OnUpdate(() => uiSetterText.Set(nowNumber));
        }

    }
}
